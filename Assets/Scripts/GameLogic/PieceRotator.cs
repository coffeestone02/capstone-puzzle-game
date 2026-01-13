using UnityEngine;

/// <summary>
/// 피스의 회전
/// </summary>
public class PieceRotator : MonoBehaviour
{
    private int rotationIndex;

    /// <summary>
    /// 피스를 회전
    /// </summary>
    /// <param name="direction">1은 오른쪽, -1은 왼쪽 회전</param>
    public void Rotate(Piece piece, int direction)
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
        PieceMover pieceMover = GetComponent<PieceMover>();
        TriominoData data = piece.data;
        int wallKickIndex = GetWallKickIndex(piece, rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = piece.data.wallKicks[wallKickIndex, i];

            if (pieceMover.Move(piece, translation))
                return true;
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
