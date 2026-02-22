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
    private ObstacleConvert obstacleConvert;
    private Queue<Vector3Int> bombQueue;
    private Queue<Vector3Int> rocketQueue;


    private void Start()
    {
        board = GetComponent<Board>();
        bounds = board.Bounds;
        destroyParticle = Resources.Load<GameObject>("VisualAssets/Particles/DestroyParticle");
        bombParticle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
        obstacleConvert = GetComponent<ObstacleConvert>();

        bombQueue = new Queue<Vector3Int>();
        rocketQueue = new Queue<Vector3Int>();
    }

    /// <summary>
    /// 조작중인 피스를 기준으로 매칭 시도
    /// </summary>
    /// <param name="piece">기준이 될 피스</param>
    public void TryMatch(Piece piece)
    {
        if (piece == null)
            return;

        if (piece.tiles == null || piece.tiles.Length == 0 || piece.tiles[0] == null)
            return;

        if (piece.tiles[0].name == "BombTile")
        {
            HashSet<Vector3Int> clearedAllBomb = new HashSet<Vector3Int>();
            UseBombItem(piece, clearedAllBomb);

            if (obstacleConvert != null && clearedAllBomb.Count > 0)
                obstacleConvert.ConvertAround(clearedAllBomb);

            Managers.Audio.PlaySFX("ExplodeSFX");
            return;
        }

        HashSet<Vector3Int> matched = FindMainMatch(piece);
        HashSet<Vector3Int> bonusMatched = FindBonusMatch(matched);

        // 매치된 블럭 갯수 업데이트
        int matchedCount = matched.Count + bonusMatched.Count;
        Managers.Rule.BlockCounter += matchedCount;

        // 이번 턴에 "실제로 삭제된 모든 좌표" 누적
        HashSet<Vector3Int> clearedAll = new HashSet<Vector3Int>();

        // 삭제 및 점수계산
        int itemMatchedCount = DeleteMatchedPiece(matched, clearedAll) + DeleteMatchedPiece(bonusMatched, clearedAll);
        Managers.Score.SetScore(matched.Count, bonusMatched.Count, itemMatchedCount);

        // 모든 삭제 후 주변 장애물 제거 1회
        if (obstacleConvert != null && clearedAll.Count > 0)
        {
            obstacleConvert.ConvertAround(clearedAll);
        }

        // 삭제 소리 재생
        if (matchedCount > 0)
            Managers.Audio.PlaySFX("DestroySFX");
        if (itemMatchedCount > 0)
            Managers.Audio.PlaySFX("ExplodeSFX");
    }

    private int DeleteMatchedPiece(HashSet<Vector3Int> matched, HashSet<Vector3Int> clearedAll)
    {
        if (matched == null || matched.Count == 0)
            return 0;

        Tilemap tilemap = board.tilemap;
        bombQueue.Clear();
        rocketQueue.Clear();

        foreach (Vector3Int pos in matched)
        {
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (tile == null || board.IsCenterCell(pos))
                continue;

            // 아이템 위치 수집
            EnqueueItem(tile, pos);

            // 타일 삭제
            tilemap.SetTile(pos, null);

            // 삭제된 좌표 기록
            clearedAll.Add(pos);

            PlayParticle(destroyParticle, pos);
        }

        // 아이템 사용
        int itemMatchedCount = 0;

        while (bombQueue.Count > 0)
        {
            Vector3Int pos = bombQueue.Dequeue();
            itemMatchedCount += UseBomb(pos, Managers.Rule.bombRange, bombQueue, rocketQueue, clearedAll);
        }

        while (rocketQueue.Count > 0)
        {
            Vector3Int pos = rocketQueue.Dequeue();
            itemMatchedCount += UseRocket(pos, bombQueue, rocketQueue, clearedAll);
        }

        return itemMatchedCount;
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
        if (matchTile == null)
            return connections.ToArray();

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

    // 폭탄으로 제거한 개수 반환
    private int UseBomb(Vector3Int startPos, int range, Queue<Vector3Int> bombQueue, Queue<Vector3Int> rocketQueue, HashSet<Vector3Int> clearedAll)
    {
        PlayParticle(bombParticle, startPos);
        int bombCount = 0;
        int offset = range / 2;
        Tilemap tilemap = board.tilemap;

        for (int x = startPos.x - offset; x <= startPos.x + offset; x++)
        {
            for (int y = startPos.y - offset; y <= startPos.y + offset; y++)
            {
                if (x < bounds.xMin || x >= bounds.xMax || y < bounds.yMin || y >= bounds.yMax)
                    continue;

                Vector3Int pos = new Vector3Int(x, y, 0);
                Tile tile = tilemap.GetTile<Tile>(pos);
                if (board.IsCenterCell(pos) || tile == null)
                    continue;

                EnqueueItem(tile, pos);

                tilemap.SetTile(pos, null);
                clearedAll.Add(pos);
                bombCount++;
            }
        }

        return bombCount;
    }

    // 로켓으로 제거한 개수 반환
    private int UseRocket(Vector3Int startPos, Queue<Vector3Int> bombQueue, Queue<Vector3Int> rocketQueue, HashSet<Vector3Int> clearedAll)
    {
        int rocketCount = 0;
        Tilemap tilemap = board.tilemap;

        // 가로 방향
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            Vector3Int pos = new Vector3Int(x, startPos.y, 0);
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (board.IsCenterCell(pos) || tile == null)
                continue;

            // 또 다른 아이템 위치를 큐에 추가
            EnqueueItem(tile, pos);
            PlayParticle(destroyParticle, pos);
            tilemap.SetTile(pos, null);
            clearedAll.Add(pos);
            rocketCount++;
        }

        // 세로 방향
        for (int y = bounds.xMin; y < bounds.xMax; y++)
        {
            Vector3Int pos = new Vector3Int(startPos.x, y, 0);
            Tile tile = tilemap.GetTile<Tile>(pos);
            if (board.IsCenterCell(pos) || tile == null)
                continue;

            // 또 다른 아이템 위치를 큐에 추가
            EnqueueItem(tile, pos);

            PlayParticle(destroyParticle, pos);
            tilemap.SetTile(pos, null);
            clearedAll.Add(pos);
            rocketCount++;
        }

        return rocketCount;
    }

    private void PlayParticle(GameObject effect, Vector3Int position)
    {
        GameObject particle = Instantiate(effect, board.tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        Destroy(particle, 1f);
    }

    private void EnqueueItem(Tile tile, Vector3Int pos)
    {
        string tileName = tile.name.ToLowerInvariant();
        if (tileName.Contains("bomb"))
            bombQueue.Enqueue(pos);
        else if (tileName.Contains("rocket"))
            rocketQueue.Enqueue(pos);
    }

    private int UseBombItem(Piece piece, HashSet<Vector3Int> clearedAll)
    {
        HashSet<Vector3Int> matched = new HashSet<Vector3Int>();
        for (int i = 0; i < piece.cells.Length; i++)
            matched.Add(piece.cells[i] + piece.position);

        return DeleteMatchedPiece(matched, clearedAll);
    }
}