using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    // 방향 벡터
    public static Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

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

    // 파티클 효과를 생성하고 재생하는 함수
    public static void PlayDestroyParticle(GameObject effect, Vector3 position)
    {
        if (effect == null)
        {
            Debug.LogError("Util.cs : effect is null");
            return;
        }

        GameObject particle = MonoBehaviour.Instantiate(effect, position, Quaternion.identity);
        MonoBehaviour.Destroy(particle, 1f); // 1초 뒤에 파괴
    }
}
