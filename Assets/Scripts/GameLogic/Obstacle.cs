using UnityEngine;
using UnityEngine.Tilemaps;

public class Obstacle : MonoBehaviour
{
    private Board board;
    private RectInt bounds;
    private Tilemap tilemap;
    private Tile obstacleTile;
    public EPieceDir nextSpawnDir { get; private set; }

    private void Start()
    {
        board = GetComponent<Board>();
        bounds = board.Bounds;
        tilemap = board.tilemap;
        obstacleTile = Resources.Load<Tile>("VisualAssets/Tiles/ObstacleTile");
        SetNextSpawnPos();
    }

    // 스폰 방향을 미리 결정해놓음
    private void SetNextSpawnPos()
    {
        int randomPos = UnityEngine.Random.Range(0, 4);
        switch (randomPos)
        {
            case 0:
                nextSpawnDir = EPieceDir.UP;
                break;
            case 1:
                nextSpawnDir = EPieceDir.RIGHT;
                break;
            case 2:
                nextSpawnDir = EPieceDir.DOWN;
                break;
            case 3:
                nextSpawnDir = EPieceDir.LEFT;
                break;
        }
    }

    /// <summary>
    /// 장애물을 랜덤한 방향으로 스폰함
    /// </summary>
    public void SpawnObstacle()
    {
        Managers.Rule.obstacleCount++;
        Debug.Log(Managers.Rule.obstacleCount);
        if (Managers.Rule.obstacleCount >= Managers.Rule.obstacleSpawnLimit)
        {
            Managers.Rule.obstacleCount = 0;
            switch (nextSpawnDir)
            {
                case EPieceDir.UP:
                    UpSpawn();
                    break;
                case EPieceDir.RIGHT:
                    RightSpawn();
                    break;
                case EPieceDir.DOWN:
                    DownSpawn();
                    break;
                case EPieceDir.LEFT:
                    LeftSpawn();
                    break;
            }
            SetNextSpawnPos();
        }
    }

    private void UpSpawn()
    {
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            bool hasTile = false;
            int ty = 0;

            // 가장 위쪽 타일 검색
            for (int y = bounds.yMax; y >= bounds.yMin; y--)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    hasTile = true;
                    ty = y;
                    break;
                }
            }

            // 타일을 찾았으면
            Vector3Int obstaclePos = new Vector3Int(x, ty + 1, 0);
            if (hasTile)
            {
                SetObstacle(obstaclePos);
            }
        }
    }

    private void DownSpawn()
    {
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            bool hasTile = false;
            int ty = 0;

            // 가장 아래쪽 타일 검색
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    hasTile = true;
                    ty = y;
                    break;
                }
            }

            // 타일을 찾았으면
            Vector3Int obstaclePos = new Vector3Int(x, ty - 1, 0);
            if (hasTile)
            {
                SetObstacle(obstaclePos);
            }
        }
    }

    private void RightSpawn()
    {
        for (int y = bounds.yMax; y >= bounds.yMin; y--)
        {
            bool hasTile = false;
            int tx = 0;

            // 가장 오른쪽 타일 검색
            for (int x = bounds.xMax; x >= bounds.xMin; x--)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    hasTile = true;
                    tx = x;
                    break;
                }
            }

            // 타일을 찾았으면
            Vector3Int obstaclePos = new Vector3Int(tx + 1, y, 0);
            if (hasTile)
            {
                SetObstacle(obstaclePos);
            }
        }
    }

    private void LeftSpawn()
    {
        for (int y = bounds.yMax; y >= bounds.yMin; y--)
        {
            bool hasTile = false;
            int tx = 0;

            // 가장 왼쪽 타일 검색
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    hasTile = true;
                    tx = x;
                    break;
                }
            }

            // 타일을 찾았으면
            Vector3Int obstaclePos = new Vector3Int(tx - 1, y, 0);
            if (hasTile)
            {
                SetObstacle(obstaclePos);
            }
        }
    }

    private void SetObstacle(Vector3Int obstaclePos)
    {
        if (board.IsCenterCell(obstaclePos) == false && bounds.Contains((Vector2Int)obstaclePos))
            tilemap.SetTile(obstaclePos, obstacleTile);
    }

}
