using UnityEngine;

/// <summary>
/// 피스의 회전
/// </summary>
public class PieceRotator : MonoBehaviour
{
    private int rotationIndex;

    private void Start()
    {
        Managers.Input.rotateAction -= OnRotate;
        Managers.Input.rotateAction += OnRotate;
    }

    /// <summary>
    /// InputManager.rotateAction에 바인딩하여 사용
    /// </summary>
    /// <param name="direction">-1은 왼쪽회전, 1은 오른쪽회전</param>
    public void OnRotate(int direction)
    {
        Piece activePiece = GetComponent<Piece>();
        Board board = GetComponent<Board>();

        board.Clear(activePiece);
        Rotate(activePiece, direction);
        board.Set(activePiece);
    }

    /// <summary>
    /// 조작중인 피스를 회전시킴
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
    /// <param name="direction">1은 오른쪽, -1은 왼쪽 회전</param>
    private void Rotate(Piece piece, int direction)
    {
        int originalRotation = rotationIndex;

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotation(piece, direction);

        if (TestWallKicks(piece, rotationIndex, direction) == false)
        {
            rotationIndex = originalRotation;
            ApplyRotation(piece, -direction);
        }
    }

    private void ApplyRotation(Piece piece, int direction)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            int x, y;
            if (direction > 0)
            {
                x = piece.cells[i].y;
                y = -piece.cells[i].x;
            }
            else
            {
                x = -piece.cells[i].y;
                y = piece.cells[i].x;
            }

            piece.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(Piece piece, int rotationIndex, int rotationDirection)
    {
        Board board = GetComponent<Board>();
        TriominoData data = piece.data;
        int wallKickIndex = GetWallKickIndex(piece, rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = piece.data.wallKicks[wallKickIndex, i];
            Vector3Int newPosition = piece.position;
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            if (board.IsValidPosition(piece, newPosition))
            {
                piece.position = newPosition;
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(Piece piece, int rotationIndex, int rotationDirection)
    {
        TriominoData data = piece.data;
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
            wallKickIndex--;

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
}
