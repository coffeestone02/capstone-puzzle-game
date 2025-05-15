using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 피스의 이동, 회전을 담당, 플레이어 조작도 여기서 받음
public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TriominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    public void Initialize(Board board, Vector3Int position, TriominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

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

        switch(board.currentSpawnIdx)
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
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if(valid)
        {
            this.position = newPosition;
        }

        return valid;
    }

    // 인풋들
    public void TopPosInput()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    public void RightPosInput()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
    }

    public void BottomPosInput()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    public void LeftPosInput()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Move(Vector2Int.up);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
    }

}
