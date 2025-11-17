using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleSpawner : MonoBehaviour
{
    private Board board;
    private Tile obstacleTile; // 장애물 타일

    private void Start()
    {
        board = FindObjectOfType<Board>();
        obstacleTile = Resources.Load<Tile>("VisualAssets/Tiles/stone03");

        if (board == null)
        {
            Debug.LogError("ObstacleSpawner.cs : board is null");
        }
        if (obstacleTile == null)
        {
            Debug.LogError("ObstacleSpawner.cs : obstacleTile is null");
        }
    }

    public void SpawnObstacle()
    {
        RectInt bounds = board.Bounds;
        int xMin = bounds.xMin + 1;
        int xMax = bounds.xMax - 2; // 포함 범위
        int yMin = bounds.yMin + 1;
        int yMax = bounds.yMax - 2; // 포함 범위

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
            int? firstOccY = null;
            for (int y = yMin; y <= yMax; y++)
            {
                if (board.tilemap.HasTile(new Vector3Int(x, y, 0)))
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
                    if (Util.IsCenterCell(pos) == false && board.tilemap.HasTile(pos) == false)
                        board.tilemap.SetTile(pos, obstacleTile);
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
                if (board.tilemap.HasTile(new Vector3Int(x, y, 0)))
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
                    if (Util.IsCenterCell(pos) == false && board.tilemap.HasTile(pos) == false)
                        board.tilemap.SetTile(pos, obstacleTile);
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
                if (board.tilemap.HasTile(new Vector3Int(x, y, 0)))
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
                    if (Util.IsCenterCell(pos) == false && board.tilemap.HasTile(pos) == false)
                        board.tilemap.SetTile(pos, obstacleTile);
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
                if (board.tilemap.HasTile(new Vector3Int(x, y, 0)))
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
                    if (Util.IsCenterCell(pos) == false && board.tilemap.HasTile(pos) == false)
                        board.tilemap.SetTile(pos, obstacleTile);
                }
            }
        }
    }

}
