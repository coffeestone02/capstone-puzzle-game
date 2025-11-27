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

    private Vector2 fingerStartPosition;
    private Vector2 fingerEndPosition;
    [SerializeField] private float swipeThreshold = 50f;
    private float swipeTime;
    private Vector2Int gravityDir;
    private Vector2 accumulatedDrag; // 누적된 드래그 거리
    [SerializeField] private float pixelsPerGridCell = 80f; // 한 칸 이동에 필요한 픽셀 거리
    private bool isMove;


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
            Move(Vector2Int.left);
    }

    private void Update()
    {
        if (board.gameManager.isOver || board.gameManager.isPause)
            return;

        board.Clear(this);
        SetGravityDirection();
        HandleInput();
        Step();
        board.Set(this);

        lockTime += Time.deltaTime;
    }

    private void HardDrop(Vector2Int dir)
    {
        while (Move(dir))
        {
            continue;
        }
        Lock();
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
        if (Time.time <= stepTime)
            return;

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

    private void SetGravityDirection()
    {
        switch (board.currentSpawnIdx)
        {
            case 0:
                gravityDir = Vector2Int.down;
                break;
            case 1:
                gravityDir = Vector2Int.left;
                break;
            case 2:
                gravityDir = Vector2Int.up;
                break;
            case 3:
                gravityDir = Vector2Int.right;
                break;
            default:
                gravityDir = Vector2Int.down;
                break;
        }

    }

    private void DragMove()
    {
        // 현재 터치 위치와 마지막으로 이동한 위치 사이의 델타 계산
        Vector2 currentDrag = fingerEndPosition - fingerStartPosition;

        accumulatedDrag += currentDrag; // 누적 드래그에 추가
        fingerStartPosition = fingerEndPosition; // 마지막 위치 업데이트

        // 좌우
        if (Mathf.Abs(accumulatedDrag.x) >= pixelsPerGridCell)
        {
            int moveSteps = (int)(accumulatedDrag.x / pixelsPerGridCell);

            if (moveSteps > 0 && board.currentSpawnIdx != 1) // 오른쪽
            {
                for (int i = 0; i < moveSteps; i++)
                {
                    Move(Vector2Int.right);
                }

                accumulatedDrag.x -= moveSteps * pixelsPerGridCell;
            }
            else if (moveSteps < 0 && board.currentSpawnIdx != 3) // 왼쪽
            {
                for (int i = 0; i < Mathf.Abs(moveSteps); i++)
                {
                    Move(Vector2Int.left);
                }

                accumulatedDrag.x -= moveSteps * pixelsPerGridCell;
            }

            isMove = true;
        }

        // 상하
        if (Mathf.Abs(accumulatedDrag.y) >= pixelsPerGridCell)
        {
            int moveSteps = (int)(accumulatedDrag.y / pixelsPerGridCell);

            if (moveSteps > 0 && board.currentSpawnIdx != 0) // 위
            {
                for (int i = 0; i < moveSteps; i++)
                {
                    Move(Vector2Int.up);
                }

                accumulatedDrag.y -= moveSteps * pixelsPerGridCell;
            }
            else if (moveSteps < 0 && board.currentSpawnIdx != 2) // 아래
            {
                for (int i = 0; i < Mathf.Abs(moveSteps); i++)
                {
                    Move(Vector2Int.down);
                }

                accumulatedDrag.y -= moveSteps * pixelsPerGridCell;
            }

            isMove = true;
        }
    }


    private void RotateAndHardDrop()
    {
        // 처음 터치 위치와 손을 뗀 위치 사이의 거리
        float swipeDistanceX = Mathf.Abs(fingerEndPosition.x - fingerStartPosition.x);
        float swipeDistanceY = Mathf.Abs(fingerEndPosition.y - fingerStartPosition.y);

        if (isMove == false && swipeDistanceX < swipeThreshold && swipeDistanceY < swipeThreshold) // 터치하고 끝(회전)
        {
            Rotate(1);
        }
        else if (swipeTime <= 0.3f && swipeDistanceX > swipeDistanceY)
        {
            if (fingerStartPosition.x - fingerEndPosition.x > 0 && gravityDir == Vector2Int.left)
            {
                HardDrop(Vector2Int.left); // 왼쪽으로 하드드랍
            }
            else if (gravityDir == Vector2Int.right)
            {
                HardDrop(Vector2Int.right); // 오른쪽으로 하드드랍
            }
        }
        else if (swipeTime <= 0.3f && swipeDistanceY > swipeDistanceX) // 상하 하드드랍
        {
            if (fingerStartPosition.y - fingerEndPosition.y > 0 && gravityDir == Vector2Int.down)
            {
                HardDrop(Vector2Int.down); // 아래로 하드드랍 
            }
            else if (gravityDir == Vector2Int.up)
            {
                HardDrop(Vector2Int.up); // 위로 하드드랍
            }
        }
    }

    // 입력
    private void HandleInput()
    {
        if (Input.touchCount <= 0)
            return;

        Touch touch = Input.GetTouch(0); // 터치 정보 가져옴
        if (touch.phase == TouchPhase.Began)
        {
            fingerStartPosition = touch.position;
            accumulatedDrag = Vector2.zero;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            fingerEndPosition = touch.position;
            swipeTime += Time.deltaTime;
            DragMove(); // 이동
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            fingerEndPosition = touch.position;
            RotateAndHardDrop(); // 회전과 하드드랍
            swipeTime = 0f;
            accumulatedDrag = Vector2.zero;
            isMove = false;
        }
    }

    // 움직임
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
                return true;
        }

        return false;
    }

    // 회전 상태와 방향에 맞는 월킥 인덱스 가져오기
    private int GetWallKickIdx(int rotationIdx, int rotationDirection)
    {
        int wallKickIdx = rotationIdx * 2;

        if (rotationDirection < 0) wallKickIdx--;

        return Wrap(wallKickIdx, 0, data.wallKicks.GetLength(0));
    }

    // 회전할 때 인덱스를 범위에 맞게 설정해줌
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);

    }
}
