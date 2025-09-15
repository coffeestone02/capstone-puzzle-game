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

    private float[] stepDelayByDifficulty = { 1.25f, 0.8f, 0.3f };

    public float stepDelay = 1.25f; // 중심으로 이동하는 속도. 자동으로 stepDelay의 시간만큼 중심으로 이동함
    public float moveDelay = 0.1f; // 플레이어의 입력 이동 속도를 정함. 값이 클수록 입력을 많이 못함
    public float lockDelay = 0.5f; // 이 시간만큼 못 움직이면 피스를 Lock함

    private float stepTime; // 중심으로 이동하는 시기
    private float moveTime; // 다음 입력을 받을 수 있는 시기
    private float lockTime; // 고정되는 시기(lockTime이 lockDelay를 넘기는 순간 고정됨)

    // piece가 처음 생성됐을 때 색을 결정함
    private void ColorSet(Piece piece, out Tile firstTile, out Tile secondTile, out Tile thirdTile)
    {
        int randomIdx = Random.Range(0, 3); // switch의 조건 개수
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
        if (board.gameManager.isOver)
        {
            return;
        }
        
        SetDifficulty();

        // 지우고 위치를 옮겨서 다시 그린다.
        this.board.Clear(this);

        lockTime += Time.deltaTime; 

        // 회전 입력
        RotationInput();

        if (Time.time > moveTime)
        {
            switch (board.currentSpawnIdx)
            {
                case 0:
                    TopPosInput();
                    break;
                case 1:
                    RightPosInput();
                    break;
                case 2:
                    BottomPosInput();
                    break;
                case 3:
                    LeftPosInput();
                    break;
                default:
                    break;
            }
        }

        // HardDrop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        
        if (Time.time > stepTime)
        {
            Step();
        }

        this.board.Set(this);
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
        }

        board.Set(this); // 고정하고
        board.NextSpawnIdx(); // 스폰 위치를 변경
        board.TryMatch(this); // 피스 제거 시도
        board.ChangeGray(this); // 가장자리 혹은 회색블록에 낙하시 회색 블록으로 변화
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

        bool valid = board.IsValidPosition(this, newPosition); // 유효한 위치인지 확인

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
    public void Rotate(int direction)
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
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
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

    // 위로 입력 불가
    private void TopPosInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
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
        }
    }

    // 오른쪽으로 입력 불가
    private void RightPosInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
    }

    // 아래쪽으로 입력 불가
    private void BottomPosInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
    }

    // 왼쪽으로 입력 불가
    private void LeftPosInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
    }

}
