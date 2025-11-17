using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 범위 확인
public class Util : MonoBehaviour
{
    // 방향 벡터
    public static Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    // 셀의 자리가 유효한지 검사하는 함수
    public static bool IsValidPosition(Board board, Piece piece, Vector3Int position)
    {
        RectInt bounds = board.Bounds;

        // 각 셀마다 검사
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // 보드 범위 안에 있는지 검사
            if (bounds.Contains((Vector2Int)tilePosition) == false)
                return false;

            // 이미 타일이 있는지 검사
            if (board.tilemap.HasTile(tilePosition))
                return false;

        }

        return true;
    }

    // 스폰 위치 반대편 모서리에 닿았는지 확인
    public static bool IsTouchEdge(Board board, Piece piece, Vector3Int position)
    {
        RectInt bounds = board.Bounds;
        int left = bounds.xMin + 1;
        int right = bounds.xMax - 2;
        int down = bounds.yMin + 1;
        int up = bounds.yMax - 2;
        int currentSpawnIdx = board.currentSpawnIdx;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int pos = piece.cells[i] + position;

            if (currentSpawnIdx == 0 && pos.y <= down) // 위에서 아래
                return true;
            else if (currentSpawnIdx == 1 && pos.x <= left) // 오른쪽에서 왼쪽
                return true;
            else if (currentSpawnIdx == 2 && pos.y >= up) // 아래에서 위
                return true;
            else if (currentSpawnIdx == 3 && pos.x >= right) // 왼쪽에서 오른쪽
                return true;
        }

        return false;
    }

    // 경계선인지 확인
    public static bool InEdge(RectInt bounds, int xPos, int yPos)
    {
        if (xPos <= bounds.xMin + 1 || xPos >= bounds.xMax - 2 ||
            yPos <= bounds.yMin + 1 || yPos >= bounds.yMax - 2)
        {
            return true;
        }

        return false;
    }

    // 중앙 보호칸 체크
    public static bool IsCenterCell(Vector3Int p)
    {
        return (Vector2Int)p == new Vector2Int(-1, -1);
    }
}
