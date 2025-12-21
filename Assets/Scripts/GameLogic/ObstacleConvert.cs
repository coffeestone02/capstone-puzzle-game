using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// 블록 삭제 후, 인접한 장애물을 다른 타일로 변환
public class ObstacleConvert : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Board board;

    [Header("장애물 판정 타일들")]
    [Tooltip("장애물로 취급할 타일들 (stone03)")]
    [SerializeField] private TileBase[] obstacleTiles;

    [Header("변환 결과 타일")]
    [SerializeField] private TileBase convertToRedTile;
    [SerializeField] private TileBase convertToBlueTile;

    private void Awake()
    {
        if (board == null)
            board = FindObjectOfType<Board>();
    }

    // 삭제된 셀 주변(상하좌우)의 장애물을 변환
    public void ConvertAround(HashSet<Vector3Int> clearedCells)
    {
        if (clearedCells == null || clearedCells.Count == 0)
            return;

        RectInt bounds = board.Bounds;

        foreach (Vector3Int c in clearedCells)
        {
            TryConvert(c + Vector3Int.up, bounds);
            TryConvert(c + Vector3Int.down, bounds);
            TryConvert(c + Vector3Int.left, bounds);
            TryConvert(c + Vector3Int.right, bounds);
        }
    }

    // 변환 시도
    private void TryConvert(Vector3Int pos, RectInt bounds)
    {
        if (!bounds.Contains((Vector2Int)pos))
            return;

        if (Util.IsCenterCell(pos))
            return;

        TileBase current = board.tilemap.GetTile(pos);
        if (!IsObstacleTile(current))
            return;

        TileBase next =
            UnityEngine.Random.value < 0.5f ? convertToRedTile : convertToBlueTile;

        board.tilemap.SetTile(pos, next);
    }

    // 장애물 타일인지 확인
    private bool IsObstacleTile(TileBase tile)
    {
        if (tile == null || obstacleTiles == null)
            return false;

        foreach (TileBase t in obstacleTiles)
        {
            if (t == tile)
                return true;
        }

        return false;
    }
}

