using UnityEngine;

public class Util
{
    /// <summary>
    /// EpieceDir를 Vector2Int 형태로 변환
    /// </summary>
    public static Vector2Int GetMoveVector2Int(EPieceDir moveDir)
    {
        if (moveDir == EPieceDir.UP) return Vector2Int.up;
        if (moveDir == EPieceDir.DOWN) return Vector2Int.down;
        if (moveDir == EPieceDir.LEFT) return Vector2Int.left;
        if (moveDir == EPieceDir.RIGHT) return Vector2Int.right;

        return Vector2Int.zero;
    }

    /// <summary>
    /// Vector2Int를 EpieceDir로 변환
    /// </summary>
    public static EPieceDir GetEPieceDir(Vector2Int moveVec)
    {
        if (moveVec == Vector2Int.up) return EPieceDir.UP;
        if (moveVec == Vector2Int.down) return EPieceDir.DOWN;
        if (moveVec == Vector2Int.left) return EPieceDir.LEFT;
        if (moveVec == Vector2Int.right) return EPieceDir.RIGHT;

        return EPieceDir.NONE;
    }
}
