using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 피스의 이동, 회전을 담당, 플레이어 조작도 여기서 받음
public class Piece : MonoBehaviour
{
    public Board board { get; private set; } // 현재 사용중인 보드
    public TriominoData data { get; private set; } // 현재 트리오미노의 데이터
    public Vector3Int[] cells { get; private set; } // 셀들의 위치 정보
    public Vector3Int position { get; private set; } // 피스의 위치 정보
    public Tile[] tiles { get; private set; } // 셀들의 색상 정보
    public int rotationIdx { get; private set; }

<<<<<<< HEAD
    public float stepDelay = 3f; 
    public float moveDelay = 0.1f;   
    public float lockDelay = 0.5f;   
=======
    private float[] stepDelayByDifficulty = { 1.25f, 0.8f, 0.3f };
>>>>>>> main

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

    private Vector2Int lateralNeg;   // holdDir=-1일 때 이동 벡터
    private Vector2Int lateralPos;   // holdDir=+1일 때 이동 벡터
    private KeyCode negKey;
    private KeyCode posKey;

    //빠른 낙하
    public float softDropArr = 0.03f; // 낙하 키 홀드 시 연사 간격(작을수록 빠름)
    private float softDropTimer = 0f;
    private KeyCode gravityKey;       // 스폰 방향에 따라 달라지는 '중력 키'
    private Vector2Int gravityVec;    // 스폰 방향에 따른 '중력 벡터'

    // Sound
    public AudioSource sound;              // Piece(또는 자식)에 붙인 AudioSource
    public AudioClip soundMove;            // 좌/우(측면) 이동
    public AudioClip soundSoftDropTick;    // 빠른 낙하 한 칸 '틱'
    public AudioClip soundRotate;          // 회전 성공
    public AudioClip soundLock;            // 고정
    public AudioClip soundClear;           // 블록 파괴(클리어)
    public float soundVolume = 0.8f;       // 기본 볼륨
    public Vector2 pitchJitter = new Vector2(0.95f, 1.05f); // 미세 피치 랜덤

    // 이동 Sound 과도한 스팸 방지용 쿨다운
    private float moveSoundCd = 0f;
    public float moveSoundInterval = 0.04f; // 이동음 최소 간격(초)

    private float soundMuteUntil = 0f; // 스폰 직후 음소거

    // piece가 처음 생성됐을 때 색을 결정함
    private void ColorSet(Piece piece, out Tile firstTile, out Tile secondTile, out Tile thirdTile)
    {
        int randomIdx = UnityEngine.Random.Range(0, 3);
        switch (randomIdx)
        {
            case 0:
                firstTile = piece.data.tiles[0];
                secondTile = piece.data.tiles[1];
                thirdTile = piece.data.tiles[1];
                break;
            case 1:
                firstTile = piece.data.tiles[1];
                secondTile = piece.data.tiles[0];
                thirdTile = piece.data.tiles[1];
                break;
            default:
                firstTile = piece.data.tiles[1];
                secondTile = piece.data.tiles[1];
                thirdTile = piece.data.tiles[0];
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
        soundMuteUntil = Time.time + 0.05f;

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
    }

    private void Update()
    {
<<<<<<< HEAD
        // 지우고 위치를 옮겨서 다시 그린다
        board.Clear(this);
=======
        if (board.gameManager.isOver)
        {
            return;
        }
        
        SetDifficulty();

        // 지우고 위치를 옮겨서 다시 그린다.
        this.board.Clear(this);

        lockTime += Time.deltaTime; 
>>>>>>> main

        lockTime += Time.deltaTime;
        
        // 회전 입력
        RotationInput();

        SetupDirections();   // 측면 + 중력 키/벡터 셋업
        GravityTap();        // 중력 키 '탭' → 한 칸
        HandleSoftDrop();    // 중력 키 '홀드' → 빠른 낙하
        HandleAutoShift();   // 측면 DAS/ARR

        // HardDrop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        
        if (Time.time > stepTime)
            Step();

        board.Set(this);
    }

    private void HardDrop()
    {
        switch (board.currentSpawnIdx)
        {
            case 0:
                while (Move(Vector2Int.down))
                {
                    continue;
                }
                Lock();
                break;
            case 1:
                while (Move(Vector2Int.left))
                {
                    continue;
                }
                Lock();
                break;
            case 2:
                while (Move(Vector2Int.up))
                {
                    continue;
                }
                Lock();
                break;
            case 3:
                while (Move(Vector2Int.right))
                {
                    continue;
                }
                Lock();
                break;
            default:
                break;
        }
    }

    // 레벨은 3레벨까지 존재 -> 5레벨 이상으로 늘어나면 반복문으로 변경 가능
    private void SetDifficulty()
    {
        if (board.score < board.difficultyLines[0])
        {
            stepDelay = stepDelayByDifficulty[0];
        }
        else if (board.score < board.difficultyLines[1])
        {
            stepDelay = stepDelayByDifficulty[1];
        }
        else
        {
            stepDelay = stepDelayByDifficulty[2];
        }

        // for (int idx = 0;idx < board.difficultyLines.Length;idx++)
        // {
        //     if (board.score < board.difficultyLines[idx])
        //     {
        //         stepDelay = stepDelayByDifficulty[idx];
        //         break;
        //     }
        // }
    }

    // 고정
    private void Lock()
    {
        if (board.IsGameover(this))
        {
            board.gameManager.GameOver();
<<<<<<< HEAD
            board.gameManager.SendGameData(board.score);
            UnityEngine.Debug.Log(board.currentSpawnIdx);
        }

        // 고정 Sound
        PlaySound(soundLock, 1.0f);

=======
        }

>>>>>>> main
        board.Set(this); // 고정하고
        board.NextSpawnIdx(); // 스폰 위치를 변경
        board.TryMatch(this); // 피스 제거 시도
        board.ChangeGray(this); // 가장자리 혹은 회색블록에 낙하시 회색 블록으로 변화
        board.SpawnPiece(); // 다른 피스 스폰
    }

<<<<<<< HEAD
=======
    // 정해진 시간마다 중심으로 한 칸씩 내려감
>>>>>>> main
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

    private bool Move(Vector2Int translation)
    {
        // 이동할 위치를 계산
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // 유효하다면 위치를 이동
        if (valid)
        {
            this.position = newPosition;
<<<<<<< HEAD
            moveTime = Time.time + moveDelay;
            lockTime = 0f;

            // 이동 Sound: 중력 이동과 측면 이동을 구분
            bool isGravityMove = (translation == gravityVec);

            if (isGravityMove)
            {
                // 빠른 낙하의 사운드
                PlaySound(soundSoftDropTick, 0.6f);
            }
            else
            {
                // 측면 이동 사운드 - 스팸 방지 쿨타임
                if (Time.time >= moveSoundCd)
                {
                    PlaySound(soundMove);
                    moveSoundCd = Time.time + moveSoundInterval;
                }
            }
=======
            moveTime = Time.time + moveDelay; // 다음 입력을 받을 수 있는 시기 계산
            lockTime = 0f; // lockTime 초기화
>>>>>>> main
        }

        return valid;
    }

    // 회전
    public void Rotate(int direction)
    {
        int originalRotation = rotationIdx;

        // 회전
        rotationIdx = Wrap(rotationIdx + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // 월킥 테스트에 실패하면 다시 원래대로 돌려놓음
        if (TestWallKicks(rotationIdx, direction) == false)
        {
            rotationIdx = originalRotation;
            ApplyRotationMatrix(-direction);    // 회전 실패음 재생
        }
        else
        {
            // 회전 성공 Sound
            PlaySound(soundRotate);
        }
    }

    // 실제적으로 회전시키는 메소드
    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;

            // 회전 행렬을 사용해서 회전 시킴
            switch (data.triomino)
            {
                case ETriomino.I:
                    cell.x -= 0.5f; cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
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

    // 인풋들
    // 회전 입력
    private void RotationInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Rotate(1);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Rotate(-1);
        }
    }

    // 스폰 방향에 맞춰 '측면'과 '중력' 키/벡터 정의
    private void SetupDirections()
    {
        switch (board.currentSpawnIdx)
        {
<<<<<<< HEAD
            // 중력 상/하 : 측면 = 좌/우
            case 0:
                gravityKey = KeyCode.DownArrow; gravityVec = Vector2Int.down;
                lateralNeg = Vector2Int.left; lateralPos = Vector2Int.right;
                negKey = KeyCode.LeftArrow; posKey = KeyCode.RightArrow;
                break;
            case 2:
                gravityKey = KeyCode.UpArrow; gravityVec = Vector2Int.up;
                lateralNeg = Vector2Int.left; lateralPos = Vector2Int.right;
                negKey = KeyCode.LeftArrow; posKey = KeyCode.RightArrow;
                break;

            // 중력 좌/우 : 측면 = 상/하
            case 1:
                gravityKey = KeyCode.LeftArrow; gravityVec = Vector2Int.left;
                lateralNeg = Vector2Int.down; lateralPos = Vector2Int.up;
                negKey = KeyCode.DownArrow; posKey = KeyCode.UpArrow;
                break;
            case 3:
                gravityKey = KeyCode.RightArrow; gravityVec = Vector2Int.right;
                lateralNeg = Vector2Int.down; lateralPos = Vector2Int.up;
                negKey = KeyCode.DownArrow; posKey = KeyCode.UpArrow;
                break;
=======
            if (Move(Vector2Int.down))
            {
                stepTime = Time.time + stepDelay;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
>>>>>>> main
        }
    }

    // 중력 방향으로 '한 칸' 탭
    private void GravityTap()
    {
        if (Input.GetKeyDown(gravityKey))
        {
            Move(gravityVec);
            softDropTimer = softDropArr;           // 같은 프레임 중복 방지
            stepTime = Time.time + stepDelay;      // 자연 낙하와 겹치지 않게 밀어둠
        }
    }

    // 낙하 키 홀드 → 빠른 낙하
    private void HandleSoftDrop()
    {
        if (Input.GetKey(gravityKey))
        {
            softDropTimer -= Time.unscaledDeltaTime;
            while (softDropTimer <= 0f)
            {
                softDropTimer += (softDropArr <= 0f) ? 0.0001f : softDropArr;
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
        bool negDown = Input.GetKeyDown(negKey);
        bool posDown = Input.GetKeyDown(posKey);
        bool negHeld = Input.GetKey(negKey);
        bool posHeld = Input.GetKey(posKey);
        bool negUp = Input.GetKeyUp(negKey);
        bool posUp = Input.GetKeyUp(posKey);

        if (negDown && !posHeld) StartHold(-1);
        else if (posDown && !negHeld) StartHold(+1);
        else if (negDown && posHeld) StartHold(-1);
        else if (posDown && negHeld) StartHold(+1);

        if (negDown && holdDir == +1) StartHold(-1);
        if (posDown && holdDir == -1) StartHold(+1);

        if ((negUp && holdDir == -1) || (posUp && holdDir == +1)) StopHold();

        if (holdDir != 0)
        {
            bool stillHolding = (holdDir == -1) ? negHeld : posHeld;
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
                    Move(holdDir == -1 ? lateralNeg : lateralPos);
                }
            }
        }
    }

    private void StartHold(int dir)
    {
        holdDir = dir;
        Move(holdDir == -1 ? lateralNeg : lateralPos);
        repeatTimer = das;
    }

    private void StopHold()
    {
        holdDir = 0;
        repeatTimer = 0f;
    }

    // Sound 헬퍼
    private void PlaySound(AudioClip clip, float vol = -1f, bool jitter = true)
    {
        if (sound == null || clip == null) return;

        float v = (vol < 0f) ? soundVolume : vol;

        // 피치 랜덤
        float oldPitch = sound.pitch;
        if (jitter) sound.pitch = UnityEngine.Random.Range(pitchJitter.x, pitchJitter.y);

        sound.PlayOneShot(clip, v);

        sound.pitch = oldPitch;
    }

    // 보드에서 라인/매치로 '파괴'가 일어났을 때 호출
    public void OnCleared()
    {
        PlaySound(soundClear, 1.0f, false);
    }
}