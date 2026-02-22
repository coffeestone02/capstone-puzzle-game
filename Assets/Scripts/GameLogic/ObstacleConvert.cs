using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleConvert : MonoBehaviour
{
    private Board board;
    private RectInt bounds;
    private Tilemap tilemap;

    private TileBase obstacleTile;
    private TileBase convertToRedTile;
    private TileBase convertToBlueTile;

    private HashSet<TileBase> obstacleSet;

    private void Start()
    {
        board = GetComponent<Board>();
        if (board == null) board = FindObjectOfType<Board>();

        tilemap = board != null ? board.tilemap : GetComponentInChildren<Tilemap>();
        bounds = board.Bounds;

        obstacleTile = Resources.Load<TileBase>("VisualAssets/Tiles/ObstacleTile");
        convertToRedTile = Resources.Load<TileBase>("VisualAssets/Tiles/RedTile");
        convertToBlueTile = Resources.Load<TileBase>("VisualAssets/Tiles/BlueTile");

        obstacleSet = new HashSet<TileBase>();
        if (obstacleTile != null) obstacleSet.Add(obstacleTile);
    }

    public void ConvertAround(HashSet<Vector3Int> clearedCells)
    {
        if (board == null || tilemap == null) return;
        if (clearedCells == null || clearedCells.Count == 0) return;
        if (convertToRedTile == null || convertToBlueTile == null) return;

        foreach (var c in clearedCells)
        {
            TryConvert(c + Vector3Int.up);
            TryConvert(c + Vector3Int.down);
            TryConvert(c + Vector3Int.left);
            TryConvert(c + Vector3Int.right);
        }
    }

    private void TryConvert(Vector3Int pos)
    {
        if (!bounds.Contains((Vector2Int)pos)) return;
        if (board.IsCenterCell(pos)) return;

        TileBase current = tilemap.GetTile(pos);
        if (current == null) return;

        if (!obstacleSet.Contains(current)) return;

        TileBase next = (Random.value < 0.5f) ? convertToRedTile : convertToBlueTile;
        tilemap.SetTile(pos, next);
    }
}
