using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleSpawner : MonoBehaviour
{
    private Board mainBoard;
    private Tile obstacleTile; // 장애물 타일(1x1)

    private void Start()
    {
        mainBoard = FindObjectOfType<Board>();
        obstacleTile = Resources.Load<Tile>("VisualAssets/Tiles/stone03");

        if (mainBoard == null)
        {
            Debug.LogError("ObstacleSpawner.cs : mainBoard is null");
        }
        if (obstacleTile == null)
        {
            Debug.LogError("ObstacleSpawner.cs : obstacleTile is null");
        }
    }

    public void SpawnObstacle()
    {
        if (mainBoard.obstacleCounter >= mainBoard.obstacleThreshold)
        {
            DropObstacleLine();
        }
    }

    // 장애물 1줄 드랍(총 19칸) — 방향에 따라 칸별로 독립 낙하(즉시 배치 버전)
    private void DropObstacleLine()
    {
        RectInt b = mainBoard.Bounds;
        int xMin = b.xMin + 1;
        int xMax = b.xMax - 2; // 포함 범위
        int yMin = b.yMin + 1;
        int yMax = b.yMax - 2; // 포함 범위

        int randomPos = UnityEngine.Random.Range(0, 4);
        switch (randomPos)
        {
            case 0:
                UpDrop(xMin, xMax, yMin, yMax);
                break;
            case 1:
                RightDrop(xMin, xMax, yMin, yMax);
                break;
            case 2:
                DownDrop(xMin, xMax, yMin, yMax);
                break;
            case 3:
                LeftDrop(xMin, xMax, yMin, yMax);
                break;
        }
    }

    private void UpDrop(int xMin, int xMax, int yMin, int yMax)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            int? firstOccY = 0;
            for (int y = yMin; y <= yMax; y++)
            {
                if (mainBoard.tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    firstOccY = y;
                    break;
                }
            }
            if (firstOccY.HasValue)
            {
                int restY = firstOccY.Value - 1;
                if (restY >= yMin)
                {
                    Vector3Int pos = new Vector3Int(x, restY, 0);
                    if (!Util.IsCenterCell(pos) && !mainBoard.tilemap.HasTile(pos))
                        mainBoard.tilemap.SetTile(pos, obstacleTile);
                }
            }
        }
    }

    private void RightDrop(int xMin, int xMax, int yMin, int yMax)
    {
        for (int y = yMin; y <= yMax; y++)
        {
            int? firstOccX = null;
            for (int x = xMin; x <= xMax; x++)
            {
                if (mainBoard.tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    firstOccX = x;
                    break;
                }
            }
            if (firstOccX.HasValue)
            {
                int restX = firstOccX.Value - 1;
                if (restX >= xMin)
                {
                    Vector3Int pos = new Vector3Int(restX, y, 0);
                    if (!Util.IsCenterCell(pos) && !mainBoard.tilemap.HasTile(pos))
                        mainBoard.tilemap.SetTile(pos, obstacleTile);
                }
            }
        }
    }

    private void DownDrop(int xMin, int xMax, int yMin, int yMax)
    {
        for (int x = xMin; x <= xMax; x++)
        {
            int? firstOccY = null;
            for (int y = yMax; y >= yMin; y--)
            {
                if (mainBoard.tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    firstOccY = y;
                    break;
                }
            }
            if (firstOccY.HasValue)
            {
                int restY = firstOccY.Value + 1;
                if (restY <= yMax)
                {
                    Vector3Int pos = new Vector3Int(x, restY, 0);
                    if (!Util.IsCenterCell(pos) && !mainBoard.tilemap.HasTile(pos))
                        mainBoard.tilemap.SetTile(pos, obstacleTile);
                }
            }
        }
    }

    private void LeftDrop(int xMin, int xMax, int yMin, int yMax)
    {
        for (int y = yMin; y <= yMax; y++)
        {
            int? firstOccX = null;
            for (int x = xMax; x >= xMin; x--)
            {
                if (mainBoard.tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    firstOccX = x;
                    break;
                }
            }
            if (firstOccX.HasValue)
            {
                int restX = firstOccX.Value + 1;
                if (restX <= xMax)
                {
                    Vector3Int pos = new Vector3Int(restX, y, 0);
                    if (!Util.IsCenterCell(pos) && !mainBoard.tilemap.HasTile(pos))
                        mainBoard.tilemap.SetTile(pos, obstacleTile);
                }
            }
        }
    }

}
