using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;

/// <summary>
/// 피스를 그려주기 위한 클래스
/// </summary>
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    private Piece activePiece;
    private Vector2Int boardSize = new Vector2Int(19, 19);
    public RectInt Bounds // 보드 범위
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2 - 1, -this.boardSize.y / 2 - 1);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponent<Piece>();
        activePiece.SpawnPiece();
    }

    /// <summary>
    /// Piece를 타일맵에 그림
    /// </summary>
    /// <param name="piece"></param>
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, piece.tiles[i]);
        }
    }

    /// <summary>
    /// Piece를 타일맵에서 지움
    /// </summary>
    /// <param name="piece"></param>
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    /// <summary>
    /// 피스의 모든 셀을 검사해 그려지는 위치가 유효한지 확인
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // 셀이 범위 밖으로 나간 경우
            if (bounds.Contains((Vector2Int)tilePosition) == false)
                return false;

            // 다른 셀이 있는 경우
            if (tilemap.HasTile(tilePosition))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 중앙셀인지 확인
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool IsCenterCell(Vector3Int pos)
    {
        return (Vector2Int)pos == new Vector2Int(-1, -1);
    }
}
