using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceDestroyer : MonoBehaviour
{
    private Board mainBoard;

    private void Start()
    {
        mainBoard = FindObjectOfType<Board>();

        if (mainBoard == null)
        {
            Debug.LogError("ObstacleSpawner.cs : mainBoard is null");
        }
    }

    // 메인 피스 매칭
    public HashSet<Vector3Int> FindMainMatch(Piece piece)
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

    // 추가로 제거할 수 있는 피스 찾기
    public HashSet<Vector3Int> FindBonusMatch(HashSet<Vector3Int> matched)
    {
        HashSet<Vector3Int> bonusMatched = new HashSet<Vector3Int>();
        if (matched == null || matched.Count == 0)
        {
            return bonusMatched;
        }

        RectInt bounds = mainBoard.Bounds;

        // 후보 범위 찾기
        HashSet<int> xCross = new HashSet<int>();
        HashSet<int> yCross = new HashSet<int>();
        foreach (Vector3Int pos in matched)
        {
            xCross.Add(pos.x);
            yCross.Add(pos.y);
        }

        // 가로/세로 교차점(새로운 제거 후보 위치) 전부 찾기
        foreach (int x in xCross)
        {
            foreach (int y in yCross)
            {
                Vector3Int cross = new Vector3Int(x, y, 0);

                // 이미 기존 matched에 들어있거나(중복방지) 범위 밖이면 패스
                if (matched.Contains(cross) || bounds.Contains((Vector2Int)cross) == false)
                {
                    continue;
                }

                // 실제로 타일이 있어야만 추가
                if (mainBoard.tilemap.HasTile(cross))
                {
                    bonusMatched.Add(cross);
                }
            }
        }

        return bonusMatched;
    }

    // 연결이 4개 이상이면서 일자 모양이 아닌 경우 true
    private bool CanDelete(Vector3Int[] connections)
    {
        if (connections.Length < 4)
        {
            return false;
        }

        // x좌표가 모두 같거나 y좌표가 모두 동일하면 일자모양
        int x = connections[0].x;
        int y = connections[0].y;
        bool xStraightCheck = true;
        bool yStraightCheck = true;
        for (int i = 1; i < connections.Length; i++)
        {
            if (connections[i].x != x)
            {
                xStraightCheck = false;
            }

            if (connections[i].y != y)
            {
                yStraightCheck = false;
            }
        }

        if (xStraightCheck || yStraightCheck)
        {
            return false;
        }

        return true;
    }

    // 연결을 확인함
    private Vector3Int[] FindConnections(Piece piece, int cellIdx)
    {
        Vector3Int start = piece.cells[cellIdx] + piece.position;
        Tile matchTile = mainBoard.tilemap.GetTile<Tile>(start); // 시작칸의 '색' 기준

        List<Vector3Int> connections = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        RectInt bounds = mainBoard.Bounds;

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
                {
                    continue;
                }

                Tile nextTile = mainBoard.tilemap.GetTile<Tile>(next); if (nextTile == null) continue;
                string matchTileColor = matchTile.name.ToLowerInvariant();
                string nextTileColor = nextTile.name.ToLowerInvariant();
                if (IsSameColor(matchTileColor, nextTileColor) == false)
                {
                    continue;
                }

                queue.Enqueue(next);
                visited.Add(next);
            }
        }

        return connections.ToArray();
    }

    private bool IsSameColor(string color1, string color2)
    {
        if ((color1.Contains("purple") && color2.Contains("purple")) ||
            (color1.Contains("blue") && color2.Contains("blue")) ||
            (color1.Contains("red") && color2.Contains("red")))
        {
            return true;
        }

        return false;
    }

    // 매치된 피스를 제거
    public void DeleteMatchedPiece(HashSet<Vector3Int> matched)
    {
        if (matched == null || matched.Count == 0)
            return;

        List<Vector3Int> bombs = new List<Vector3Int>();
        List<Vector3Int> rockets = new List<Vector3Int>();

        // 일반 제거
        foreach (Vector3Int pos in matched)
        {
            if (Util.IsCenterCell(pos)) // 중앙 블록 보호
                continue;

            // 폭탄과 로켓 위치 수집
            Tile tile = mainBoard.tilemap.GetTile<Tile>(pos);
            string tileName = tile.name.ToLowerInvariant();
            if (tileName.Contains("bomb"))
            {
                bombs.Add(pos);
            }
            else if (tileName.Contains("rocket"))
            {
                rockets.Add(pos);
            }

            Util.PlayDestroyParticle(mainBoard.destroyParticle, pos);
            mainBoard.tilemap.SetTile(pos, null);
        }

        // 폭탄 사용
        foreach (Vector3Int pos in bombs)
        {
            UseBomb(pos);
        }

        // 로켓 사용
        foreach (Vector3Int pos in bombs)
        {
            UseRocket(pos);
        }
    }

    // 폭탄 폭발
    private int UseBomb(Vector3Int startPos)
    {
        Util.PlayDestroyParticle(mainBoard.bombParticle, startPos);
        RectInt bounds = mainBoard.Bounds;
        int bombScore = 0;

        // 5x5 범위로 폭발
        for (int x = startPos.x - 2; x <= startPos.x + 2; x++)
        {
            for (int y = startPos.y - 2; y <= startPos.y + 2; y++)
            {
                if (x < bounds.xMin || x >= bounds.xMax || y < bounds.yMin || y >= bounds.yMax)
                {
                    continue;
                }

                Vector3Int pos = new Vector3Int(x, y, 0);
                if (Util.IsCenterCell(pos)) // 중앙 보호
                    continue;

                mainBoard.tilemap.SetTile(pos, null);

                // 폭탄 점수는 여기서 줌 (카운트는 X)
                bombScore += 60;
            }
        }

        return bombScore;
    }

    // 십자 로켓 폭발. 로켓 파괴 점수 반환
    private int UseRocket(Vector3Int startPos)
    {
        RectInt bounds = mainBoard.Bounds;
        int rocketScore = 0;

        // 가로 라인
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            Vector3Int pos = new Vector3Int(x, startPos.y, 0);
            if (Util.IsCenterCell(pos)) // 중앙 보호
                continue;

            Util.PlayDestroyParticle(mainBoard.destroyParticle, pos);
            mainBoard.tilemap.SetTile(pos, null);

            // 로켓 점수는 여기서 줌 (카운트는 X)
            rocketScore += 60;
        }

        // 세로 라인
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            Vector3Int pos = new Vector3Int(startPos.x, y, 0);
            if (Util.IsCenterCell(pos)) // 중앙 보호
                continue;

            Util.PlayDestroyParticle(mainBoard.destroyParticle, pos);
            mainBoard.tilemap.SetTile(pos, null);

            // 로켓 점수는 여기서 줌 (카운트는 X)
            rocketScore += 60;
        }

        return rocketScore;
    }

}
