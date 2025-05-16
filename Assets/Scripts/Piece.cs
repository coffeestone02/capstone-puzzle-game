using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 피스의 이동, 회전을 담당, 플레이어 조작도 여기서 받음
public class Piece : MonoBehaviour
{
    public Board board { get; private set; } // 현재 사용중인 보드
    public TriominoData data { get; private set; } // 현재 트리오미노의 데이터
    public Vector3Int[] cells { get; private set; } // 셀들의 위치 정보
    public Vector3Int position { get; private set; } // 피스의 위치 정보

    // 초기화 함수
    public void Initialize(Board board, Vector3Int position, TriominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        // cells 배열이 null이면 배열을 만들어서 셀 데이터를 초기화 해준다
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        // 지우고 위치를 옮겨서 다시 그린다.
        this.board.Clear(this);

        // 스폰 위치에 따라 입력을 처리
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

        this.board.Set(this);
    }

    // 옆으로 움직임
    private bool Move(Vector2Int translation)
    {
        // 이동할 위치를 계산
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition); // 유효한 위치인지 확인

        // 유효하다면 위치를 이동
        if (valid)
        {
            this.position = newPosition;
        }

        return valid;
    }

    // 인풋들
    // 위로 입력 불가
    public void TopPosInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    // 오른쪽으로 입력 불가
    public void RightPosInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
    }

    // 아래쪽으로 입력 불가
    public void BottomPosInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    // 왼쪽으로 입력 불가
    public void LeftPosInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
    }

}
