using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 피스를 삭제함
/// </summary>
public class PieceMatcher : MonoBehaviour
{
    private Board board;
    private RectInt bounds;
    private GameObject destroyParticle;
    private GameObject bombParticle;

    private void Start()
    {
        board = GetComponent<Board>();
        bounds = board.Bounds;
        destroyParticle = Resources.Load<GameObject>("VisualAssets/Particles/DestroyParticle");
        bombParticle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
    }

    /// <summary>
    /// 조작중인 피스를 기준으로 매칭 시도
    /// </summary>
    /// <param name="piece">기준이 될 피스</param>
    public void TryMatch(Piece piece)
    {
        HashSet<Vector3Int> matched = FindMainMatch(piece);
        HashSet<Vector3Int> bonusMatched = FindBonusMatch(matched);

        // 매치된 블럭 갯수 업데이트
        int matchedCount = matched.Count + bonusMatched.Count;
        Managers.Rule.BlockCounter += matchedCount;

        // 삭제 및 점수계산
        int mainPoint = matched.Count * 100;
        int bonusPoint = bonusMatched.Count * 60;
        int itemPoint = DeleteMatchedPiece(matched) + DeleteMatchedPiece(bonusMatched);

        // 삭제 소리 재생
        if (matchedCount > 0)
            Managers.Audio.PlaySFX("DestroySFX");
        if (itemPoint > 0)
            Managers.Audio.PlaySFX("ExplodeSFX");
    }

    private int DeleteMatchedPiece(HashSet<Vector3Int> matched)
    {
        if (matched == null || matched.Count == 0)
            return 0;

        Tilemap tilemap = board.tilemap;
        List<Vector3Int> bombs = new List<Vector3Int>(); // 폭탄 위치
        List<Vector3Int> rockets = new List<Vector3Int>(); // 로켓 위치

        foreach (Vector3Int pos in matched)
        {
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (tile == null || IsCenterCell(pos))
                continue;

            // 아이템 위치 수집
            string tileName = tile.name.ToLowerInvariant();
            if (tileName.Contains("bomb"))
                bombs.Add(pos);
            else if (tileName.Contains("rocket"))
                rockets.Add(pos);

            // 타일 삭제
            tilemap.SetTile(pos, null);
            PlayParticle(destroyParticle, pos);
        }

        // 아이템 사용
        int itemScore = 0;
        foreach (Vector3Int pos in bombs)
            itemScore += UseBomb(pos, Managers.Rule.bombRange);
        foreach (Vector3Int pos in rockets)
            itemScore += UseRocket(pos);

        return itemScore;
    }

    private HashSet<Vector3Int> FindMainMatch(Piece piece)
    {
        HashSet<Vector3Int> matched = new HashSet<Vector3Int>();

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int[] connections = FindConnections(piece, i);
            if (CanDelete(connections))
            {
                foreach (Vector3Int pos in connections)
                {
                    matched.Add(pos);
                }
            }
        }

        return matched;
    }

    private HashSet<Vector3Int> FindBonusMatch(HashSet<Vector3Int> matched)
    {
        HashSet<Vector3Int> bonusMatched = new HashSet<Vector3Int>();
        if (matched == null || matched.Count == 0) return bonusMatched;

        Tilemap tilemap = board.tilemap;
        HashSet<int> xCross = new HashSet<int>();
        HashSet<int> yCross = new HashSet<int>();
        foreach (Vector3Int pos in matched)
        {
            xCross.Add(pos.x);
            yCross.Add(pos.y);
        }

        foreach (int x in xCross)
        {
            foreach (int y in yCross)
            {
                Vector3Int cross = new Vector3Int(x, y, 0);

                if (matched.Contains(cross) || bounds.Contains((Vector2Int)cross) == false)
                    continue;

                if (tilemap.HasTile(cross))
                    bonusMatched.Add(cross);
            }
        }

        return bonusMatched;
    }

    // 연결된 셀이 4개 이상이면서 I 모양이 아닌 경우 true
    private bool CanDelete(Vector3Int[] connections)
    {
        if (connections.Length < 4) return false;

        int x = connections[0].x;
        int y = connections[0].y;
        bool xICheck = true;
        bool yICheck = true;
        for (int i = 1; i < connections.Length; i++)
        {
            if (connections[i].x != x)
                xICheck = false;

            if (connections[i].y != y)
                yICheck = false;
        }

        if (xICheck || yICheck)
            return false;

        return true;
    }

    private Vector3Int[] FindConnections(Piece piece, int cellIdx)
    {
        List<Vector3Int> connections = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        Vector3Int start = piece.cells[cellIdx] + piece.position;
        Tile matchTile = board.tilemap.GetTile<Tile>(start);

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            connections.Add(current);

            foreach (Vector3Int dir in Util.directions)
            {
                Vector3Int next = current + dir;

                if (bounds.Contains((Vector2Int)next) == false || visited.Contains(next))
                    continue;

                Tile nextTile = board.tilemap.GetTile<Tile>(next);
                if (nextTile == null)
                    continue;

                string matchTileColor = matchTile.name.ToLowerInvariant();
                string nextTileColor = nextTile.name.ToLowerInvariant();
                if (IsSameColor(matchTileColor, nextTileColor) == false)
                    continue;

                queue.Enqueue(next);
                visited.Add(next);
            }
        }

        return connections.ToArray();
    }

    private bool IsSameColor(string color1, string color2)
    {
        if (color1.Contains("purple") && color2.Contains("purple"))
            return true;
        if (color1.Contains("blue") && color2.Contains("blue"))
            return true;
        if (color1.Contains("red") && color2.Contains("red"))
            return true;

        return false;
    }

    private bool IsCenterCell(Vector3Int pos)
    {
        return (Vector2Int)pos == new Vector2Int(-1, -1);
    }

    /// <summary>
    /// startPos 기준으로 range 범위 내의 블록을 모두 제거함
    /// </summary>
    /// <param name="startPos">기준점</param>
    /// <param name="range">폭발 범위</param>
    /// <returns></returns>
    private int UseBomb(Vector3Int startPos, int range)
    {
        PlayParticle(bombParticle, startPos);
        int bombScore = 0;
        int offset = range / 2;
        Tilemap tilemap = board.tilemap;

        for (int x = startPos.x - offset; x <= startPos.x + offset; x++)
        {
            for (int y = startPos.y - offset; y <= startPos.y + offset; y++)
            {
                if (x < bounds.xMin || x >= bounds.xMax || y < bounds.yMin || y >= bounds.yMax)
                    continue;

                Vector3Int pos = new Vector3Int(x, y, 0);
                if (IsCenterCell(pos) || tilemap.GetTile(pos) == null)
                    continue;

                tilemap.SetTile(pos, null);
                bombScore += 60;
            }
        }

        return bombScore;
    }

    /// <summary>
    /// startPos 기준으로 가로 세로에 있는 모든 블록을 제거
    /// </summary>
    /// <param name="startPos">시작점</param>
    /// <returns></returns>
    private int UseRocket(Vector3Int startPos) // <----------- 제대로 삭제 안되는 문제 발생
    {
        int rocketScore = 0;
        Tilemap tilemap = board.tilemap;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            Vector3Int pos = new Vector3Int(x, startPos.y, 0);
            if (IsCenterCell(pos) || tilemap.GetTile(pos) == null)
                continue;

            PlayParticle(destroyParticle, pos);
            tilemap.SetTile(pos, null);
            rocketScore += 60;
        }

        for (int y = bounds.xMin; y < bounds.xMax; y++)
        {
            Vector3Int pos = new Vector3Int(startPos.x, y, 0);
            if (IsCenterCell(pos) || tilemap.GetTile(pos) == null)
                continue;

            PlayParticle(destroyParticle, pos);
            tilemap.SetTile(pos, null);
            rocketScore += 60;
        }

        return rocketScore;
    }

    private void PlayParticle(GameObject effect, Vector3Int position)
    {
        GameObject particle = Instantiate(effect, board.tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        Destroy(particle, 1f);
    }
}
