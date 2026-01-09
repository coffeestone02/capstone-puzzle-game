using UnityEngine;

/// <summary>
/// 피스의 회전
/// </summary>
public class PieceRotator : MonoBehaviour
{
    private Piece activePiece;

    private void Awake()
    {
        activePiece = GetComponent<Piece>();
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        PieceMover pieceMover = GetComponent<PieceMover>();
        TriominoData data = activePiece.data;
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (pieceMover.Move(activePiece, translation))
                return true;
        }

        return false;
    }


    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        TriominoData data = activePiece.data;
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

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
