using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 피스 생성
public class PieceSpawner
{
    // private Vector2Int[] spawnPositions = { new Vector2Int(-1, 7),
    //                                        new Vector2Int(7, -1),
    //                                        new Vector2Int(0, -10),
    //                                        new Vector2Int(-9, -2) };
    private Dictionary<EPieceDir, Vector2Int> spawnPositions = new Dictionary<EPieceDir, Vector2Int>()
    {
        {EPieceDir.TOP, new Vector2Int(-1, 7)},
        {EPieceDir.RIGHT, new Vector2Int(7, -1)},
        {EPieceDir.BOTTOM, new Vector2Int(-1, 7)},
        {EPieceDir.LEFT, new Vector2Int(7, -1)}
    };
    private int bombSpawnLimit = 20; // 폭탄 생성 임계값
    private int rocketSpawnLimit = 6;   // 로켓 생성 임계값

    public Piece Spawn(EPieceDir spawnPos, TriominoData data)
    {
        Piece piece = new Piece();



        return piece;
    }

    // piece가 처음 생성됐을 때 색을 결정함
    // private void ColorSet(out Tile firstTile, out Tile secondTile, out Tile thirdTile)
    // {
    //     int randomIdx = Random.Range(0, 3); // switch의 조건 개수
    //     switch (randomIdx)
    //     {
    //         case 0:
    //             firstTile = piece.data.normalTiles[0];
    //             secondTile = piece.data.normalTiles[1];
    //             thirdTile = piece.data.normalTiles[1];
    //             break;
    //         case 1:
    //             firstTile = piece.data.normalTiles[1];
    //             secondTile = piece.data.normalTiles[0];
    //             thirdTile = piece.data.normalTiles[1];
    //             break;
    //         default:
    //             firstTile = piece.data.normalTiles[1];
    //             secondTile = piece.data.normalTiles[1];
    //             thirdTile = piece.data.normalTiles[0];
    //             break;
    //     }
    // }

    // // 폭탄이 포함된 Piece로 교체 후 반환
    // public Piece AddBombTile(Piece piece)
    // {
    //     Piece added = piece;
    //     int cell = Random.Range(0, added.cells.Length);
    //     Tile bombTile = GetBombTile(piece.tiles[cell]); // 해당 셀을 폭탄으로 교체
    //     piece.tiles[cell] = bombTile;

    //     return piece;
    // }

    // // 로켓이 포함된 Piece로 교체 후 반환
    // public Piece AddRocketTile(Piece piece)
    // {
    //     Piece added = piece;
    //     int cell = Random.Range(0, added.cells.Length);
    //     Tile bombTile = GetRocketTile(piece.tiles[cell]); // 해당 셀을 로켓으로 교체
    //     piece.tiles[cell] = bombTile;

    //     return piece;
    // }

    // // 타일 색에 맞는 폭탄 타일 반환
    // private Tile GetBombTile(Tile tile)
    // {
    //     Tile bombTile = null;
    //     string tileName = tile.name.ToLowerInvariant();

    //     if (tileName.Contains("purple"))
    //         bombTile = Resources.Load<Tile>("VisualAssets/Tiles/purpleBombStone");
    //     else if (tileName.Contains("blue"))
    //         bombTile = Resources.Load<Tile>("VisualAssets/Tiles/blueBombStone");
    //     else if (tileName.Contains("red"))
    //         bombTile = Resources.Load<Tile>("VisualAssets/Tiles/redBombStone");

    //     if (bombTile == null)
    //         Debug.LogError("bombTile is null");

    //     return bombTile;
    // }

    // // 타일 색에 맞는 로켓 타일 반환
    // public Tile GetRocketTile(Tile tile)
    // {
    //     Tile rocketTile = null;
    //     string tileName = tile.name.ToLowerInvariant();

    //     if (tileName.Contains("purple"))
    //         rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/purpleRocketStone");
    //     else if (tileName.Contains("blue"))
    //         rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/blueRocketStone");
    //     else if (tileName.Contains("red"))
    //         rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/redRocketStone");

    //     if (rocketTile == null)
    //         Debug.LogError("rocketTile is null");

    //     return rocketTile;
    // }
}
