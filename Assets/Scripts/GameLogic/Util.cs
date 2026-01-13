using UnityEngine;

public class Util
{
    /// <summary>
    /// EpieceDir를 Vector2Int 형태로 변환
    /// </summary>
    public static Vector2Int GetMoveVector2Int(EPieceDir moveDir)
    {
        Vector2Int moveVec = Vector2Int.up;
        switch (moveDir)
        {
            case EPieceDir.UP:
                moveVec = Vector2Int.up;
                break;
            case EPieceDir.RIGHT:
                moveVec = Vector2Int.right;
                break;
            case EPieceDir.DOWN:
                moveVec = Vector2Int.down;
                break;
            case EPieceDir.LEFT:
                moveVec = Vector2Int.left;
                break;
            case EPieceDir.NONE:
                moveVec = Vector2Int.zero;
                break;
        }

        return moveVec;
    }

    /// <summary>
    /// Vector2Int를 EpieceDir로 변환
    /// </summary>
    public static EPieceDir GetEPieceDir(Vector2Int moveVec)
    {
        if (moveVec == Vector2Int.up) return EPieceDir.UP;
        else if (moveVec == Vector2Int.down) return EPieceDir.DOWN;
        else if (moveVec == Vector2Int.left) return EPieceDir.LEFT;
        else if (moveVec == Vector2Int.right) return EPieceDir.RIGHT;
        else return EPieceDir.NONE;
    }
}
