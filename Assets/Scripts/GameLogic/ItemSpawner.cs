using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemSpawner : MonoBehaviour
{
    private Board board;

    private void Start()
    {
        board = FindObjectOfType<Board>();

        if (board == null)
        {
            Debug.LogError("ObstacleSpawner.cs : board is null");
        }
    }

    // 폭탄이 포함된 Piece로 교체 후 반환
    public Piece AddBombTile(Piece piece)
    {
        Piece added = piece;
        int cell = UnityEngine.Random.Range(0, added.cells.Length);
        Tile bombTile = GetBombTile(piece.tiles[cell]); // 해당 셀을 폭탄으로 교체
        piece.tiles[cell] = bombTile;

        return piece;
    }

    // 로켓이 포함된 Piece로 교체 후 반환
    public Piece AddRocketTile(Piece piece)
    {
        Piece added = piece;
        int cell = UnityEngine.Random.Range(0, added.cells.Length);
        Tile bombTile = GetRocketTile(piece.tiles[cell]); // 해당 셀을 로켓으로 교체
        piece.tiles[cell] = bombTile;

        return piece;
    }

    // 타일 색에 맞는 폭탄 타일 반환
    private Tile GetBombTile(Tile tile)
    {
        Tile bombTile = null;
        string tileName = tile.name.ToLowerInvariant();

        if (tileName.Contains("purple"))
        {
            bombTile = Resources.Load<Tile>("VisualAssets/Tiles/purpleBombStone");
        }
        else if (tileName.Contains("blue"))
        {
            bombTile = Resources.Load<Tile>("VisualAssets/Tiles/blueBombStone");
        }
        else if (tileName.Contains("red"))
        {
            bombTile = Resources.Load<Tile>("VisualAssets/Tiles/redBombStone");
        }

        if (bombTile == null)
        {
            Debug.LogError("ItemSpawner: bombTile is null");
        }

        return bombTile;
    }

    // 타일 색에 맞는 로켓 타일 반환
    public Tile GetRocketTile(Tile tile)
    {
        Tile rocketTile = null;
        string tileName = tile.name.ToLowerInvariant();

        if (tileName.Contains("purple"))
        {
            rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/purpleRocketStone");
        }
        else if (tileName.Contains("blue"))
        {
            rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/blueRocketStone");
        }
        else if (tileName.Contains("red"))
        {
            rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/redRocketStone");
        }

        if (rocketTile == null)
        {
            Debug.LogError("ItemSpawner: rocketTile is null");
        }

        return rocketTile;
    }
}
