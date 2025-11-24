using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 피스의 이동, 회전을 담당, 플레이어 조작도 여기서 받음
public class Piece : MonoBehaviour
{
    private Board board; // 현재 사용중인 보드
    private TriominoData data;// 현재 트리오미노의 데이터
    public Vector3Int[] cells { get; private set; } // 셀들의 위치 정보
    public Vector3Int position { get; private set; } // 피스의 위치 정보
    public Tile[] tiles { get; private set; } // 셀들의 색상 정보
    private int rotationIdx;

    public float stepDelay = 1.25f; // 중심으로 이동하는 속도. 자동으로 stepDelay의 시간만큼 중심으로 이동함
    public float moveDelay = 0.1f; // 플레이어의 입력 이동 속도를 정함. 값이 클수록 입력을 많이 못함
    public float lockDelay = 0.5f; // 이 시간만큼 못 움직이면 피스를 Lock함

    private float stepTime; // 중심으로 이동하는 시기
    private float moveTime; // 다음 입력을 받을 수 있는 시기
    private float lockTime; // 고정되는 시기(lockTime이 lockDelay를 넘기는 순간 고정됨)

    //DAS(딜레이), ARR(이동속도)
    public float das = 0.15f;    // 좌/우(측면) 연사 시작 지연
    public float arr = 0.03f;    // 좌/우(측면) 연사 간격
    private int holdDir = 0;         // -1=측면 음(-), +1=측면 양(+), 0=없음
    private float repeatTimer = 0f;

    private Vector2Int lateralNegative;   // holdDir=-1일 때 이동 벡터
    private Vector2Int lateralPositive;   // holdDir=+1일 때 이동 벡터
    private KeyCode negativeKeyCode;
    private KeyCode positiveKeyCode;

    //빠른 낙하
    public float softDropArr = 0.03f; // 낙하 키 홀드 시 연사 간격(작을수록 빠름)
    private float softDropTimer = 0f;
    private KeyCode inputKeyCode;       // 스폰 방향에 따라 달라지는 '중력 키'
    private Vector2Int gravityVec;    // 스폰 방향에 따른 '중력 벡터'


    // piece가 처음 생성됐을 때 색을 결정함
    private void ColorSet(Piece piece, out Tile firstTile, out Tile secondTile, out Tile thirdTile)
    {
        int randomIdx = Random.Range(0, 3); // switch의 조건 개수
        switch (randomIdx)
        {
            case 0:
                firstTile = piece.data.normalTiles[0];
                secondTile = piece.data.normalTiles[1];
                thirdTile = piece.data.normalTiles[1];
                break;
            case 1:
                firstTile = piece.data.normalTiles[1];
                secondTile = piece.data.normalTiles[0];
                thirdTile = piece.data.normalTiles[1];
                break;
            default:
                firstTile = piece.data.normalTiles[1];
                secondTile = piece.data.normalTiles[1];
                thirdTile = piece.data.normalTiles[0];
                break;
        }
    }

    // 초기화 함수
    public void Initialize(Board board, Vector3Int position, TriominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        rotationIdx = 0;

        stepTime = Time.time + stepDelay; // 중심으로 이동하는 시기 계산
        moveTime = Time.time + moveDelay; // 다음 입력을 받을 수 있는 시기 계산
        lockTime = 0f;

        // 타이머 상태 초기화
        holdDir = 0;
        repeatTimer = 0f;
        softDropTimer = 0f;

        // 타일 색 설정
        Tile firstTile;
        Tile secondTile;
        Tile thirdTile;
        ColorSet(this, out firstTile, out secondTile, out thirdTile);

        // cells 배열이 null이면 배열을 만들어서 셀 데이터를 초기화 해준다
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
            tiles = new Tile[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }

        // 색상 정보 추가
        tiles[0] = firstTile;
        tiles[1] = secondTile;
        tiles[2] = thirdTile;

        if (board.currentSpawnIdx == 2 && data.triomino == ETriomino.I)
        {
            Move(Vector2Int.left);
        }
    }

    private void Update()
    {
        if (board.gameManager.isOver || board.gameManager.isPause)
        {
            return;
        }

        // 지우고 위치를 옮겨서 다시 그린다.
        board.Clear(this);

        lockTime += Time.deltaTime;

        // 회전 입력
        RotationInput();

        SetupDirections();   // 측면 + 중력 키/벡터 셋업
        GravityTap();        // 중력 키 '탭' -> 한 칸
        HandleSoftDrop();    // 중력 키 '홀드' -> 빠른 낙하
        HandleAutoShift();   // 측면 DAS/ARR

        // HardDrop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(this);
    }

    private void HardDrop()
    {
        switch (board.currentSpawnIdx)
        {
            case 0:
                MoveToEnd(Vector2Int.down);
                Lock();
                break;
            case 1:
                MoveToEnd(Vector2Int.left);
                Lock();
                break;
            case 2:
                MoveToEnd(Vector2Int.up);
                Lock();
                break;
            case 3:
                MoveToEnd(Vector2Int.right);
                Lock();
                break;
            default:
                break;
        }
    }

    private void MoveToEnd(Vector2Int dir)
    {
        while (Move(dir))
        {
            continue;
        }
    }

    // 고정
    private void Lock()
    {
        if (Util.IsTouchEdge(board, this, this.position)) // 스폰 장소에서 바로 아래로 떨어진 경우
        {
            board.Clear(this);
        }
        else // 정상적으로 고정된 경우
        {
            board.Set(this); // 고정하고
            board.TryMatch(this); // 피스 제거 시도
            AudioManager.instance.PlayLockSound();
        }

        board.NextSpawnIdx(); // 스폰 위치를 변경
        board.SpawnPiece(); // 다른 피스 스폰
    }

    // 정해진 시간마다 중심으로 한 칸씩 내려감
    private void Step()
    {
        stepTime = Time.time + stepDelay; // 다음에 이동해야할 시기 계산

        switch (board.currentSpawnIdx)
        {
            case 0:
                Move(Vector2Int.down);
                break;
            case 1:
                Move(Vector2Int.left);
                break;
            case 2:
                Move(Vector2Int.up);
                break;
            case 3:
                Move(Vector2Int.right);
                break;
            default:
                break;
        }

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    // 옆으로 움직임
    private bool Move(Vector2Int translation)
    {
        // 이동할 위치를 계산
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = Util.IsValidPosition(board, this, newPosition); // 유효한 위치인지 확인

        // 유효하다면 위치를 이동
        if (valid)
        {
            this.position = newPosition;
            moveTime = Time.time + moveDelay; // 다음 입력을 받을 수 있는 시기 계산
            lockTime = 0f; // lockTime 초기화
        }

        return valid;
    }

    // 회전
    public void Rotate(int direction, bool isSpawn = false)
    {
        int originalRotation = rotationIdx; // 기존 방향

        // 회전
        rotationIdx = Wrap(rotationIdx + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // 월킥 테스트에 실패하면 다시 원래대로 돌려놓음
        if (TestWallKicks(rotationIdx, direction) == false)
        {
            rotationIdx = originalRotation;
            ApplyRotationMatrix(-direction);
        }
        else if (isSpawn == false)
        {
            // 회전 성공 Sound
            AudioManager.instance.PlayRotateSound();
        }
    }

    // 실제적으로 회전시키는 메소드
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            int x, y;
            if (direction > 0) // 시계방향
            {
                x = cells[i].y;
                y = -cells[i].x;
            }
            else // 반시계방향
            {
                x = -cells[i].y;
                y = cells[i].x;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // 월킥 테스트
    private bool TestWallKicks(int rotationIdx, int rotationDirection)
    {
        int wallKickIdx = GetWallKickIdx(rotationIdx, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIdx, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    // 회전 상태와 방향에 맞는 월킥 인덱스 가져오기
    private int GetWallKickIdx(int rotationIdx, int rotationDirection)
    {
        int wallKickIdx = rotationIdx * 2;

        if (rotationDirection < 0)
        {
            wallKickIdx--;
        }

        return Wrap(wallKickIdx, 0, data.wallKicks.GetLength(0));
    }

    // 회전할 때 인덱스를 범위에 맞게 설정해줌
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    // 회전 입력
    private void RotationInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Rotate(1);
        else if (Input.GetKeyDown(KeyCode.A))
            Rotate(-1);
    }

    // 스폰 방향에 맞춰 '측면'과 '중력' 키/벡터 정의
    private void SetupDirections()
    {
        switch (board.currentSpawnIdx)
        {
            // 중력 상/하 : 측면 = 좌/우
            case 0:
                inputKeyCode = KeyCode.DownArrow; gravityVec = Vector2Int.down;
                lateralNegative = Vector2Int.left; lateralPositive = Vector2Int.right;
                negativeKeyCode = KeyCode.LeftArrow; positiveKeyCode = KeyCode.RightArrow;
                break;
            case 2:
                inputKeyCode = KeyCode.UpArrow; gravityVec = Vector2Int.up;
                lateralNegative = Vector2Int.left; lateralPositive = Vector2Int.right;
                negativeKeyCode = KeyCode.LeftArrow; positiveKeyCode = KeyCode.RightArrow;
                break;

            // 중력 좌/우 : 측면 = 상/하
            case 1:
                inputKeyCode = KeyCode.LeftArrow; gravityVec = Vector2Int.left;
                lateralNegative = Vector2Int.down; lateralPositive = Vector2Int.up;
                negativeKeyCode = KeyCode.DownArrow; positiveKeyCode = KeyCode.UpArrow;
                break;
            case 3:
                inputKeyCode = KeyCode.RightArrow; gravityVec = Vector2Int.right;
                lateralNegative = Vector2Int.down; lateralPositive = Vector2Int.up;
                negativeKeyCode = KeyCode.DownArrow; positiveKeyCode = KeyCode.UpArrow;
                break;
        }
    }

    // 중력 방향으로 '한 칸' 탭
    private void GravityTap()
    {
        if (Input.GetKeyDown(inputKeyCode))
        {
            Move(gravityVec);
            softDropTimer = softDropArr;           // 같은 프레임 중복 방지
            stepTime = Time.time + stepDelay;      // 자연 낙하와 겹치지 않게 밀어둠
        }
    }

    // 낙하 키 홀드 -> 빠른 낙하
    private void HandleSoftDrop()
    {
        if (Input.GetKey(inputKeyCode))
        {
            softDropTimer -= Time.unscaledDeltaTime;
            while (softDropTimer <= 0f)
            {
                if (softDropArr <= 0f)
                    softDropTimer += 0.0001f;
                else
                    softDropTimer += softDropArr;

                Move(gravityVec);
                stepTime = Time.time + stepDelay;  // 자연 낙하와 중복 방지
            }
        }
        else
        {
            softDropTimer = 0f;
        }
    }

    // 좌/우(or 상/하) DAS/ARR
    private void HandleAutoShift()
    {
        bool negativeDown = Input.GetKeyDown(negativeKeyCode);
        bool positiveDown = Input.GetKeyDown(positiveKeyCode);
        bool negativeHeld = Input.GetKey(negativeKeyCode);
        bool positiveHeld = Input.GetKey(positiveKeyCode);
        bool negativeUp = Input.GetKeyUp(negativeKeyCode);
        bool positiveUp = Input.GetKeyUp(positiveKeyCode);

        if (negativeDown && !positiveHeld) StartHold(-1);
        else if (positiveDown && !negativeHeld) StartHold(1);
        else if (negativeDown && positiveHeld) StartHold(-1);
        else if (positiveDown && negativeHeld) StartHold(1);

        if (negativeDown && holdDir == 1) StartHold(-1);
        if (positiveDown && holdDir == -1) StartHold(1);

        if ((negativeUp && holdDir == -1) || (positiveUp && holdDir == 1)) StopHold();

        if (holdDir != 0)
        {
            bool stillHolding = (holdDir == -1) ? negativeHeld : positiveHeld;
            if (!stillHolding)
            {
                StopHold();
            }
            else
            {
                repeatTimer -= Time.unscaledDeltaTime;
                while (repeatTimer <= 0f)
                {
                    repeatTimer += (arr <= 0f) ? 0.0001f : arr;
                    Move(holdDir == -1 ? lateralNegative : lateralPositive);
                }
            }
        }
    }

    private void StartHold(int dir)
    {
        holdDir = dir;

        if (holdDir == -1)
            Move(lateralNegative);
        else
            Move(lateralPositive);

        repeatTimer = das;
    }

    private void StopHold()
    {
        holdDir = 0;
        repeatTimer = 0f;
    }
}
