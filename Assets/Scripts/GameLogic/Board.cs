using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using TMPro;

// 피스가 그려짐
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    private Piece activePiece; // 조작중인 피스
    private Vector2Int boardSize = new Vector2Int(19, 19); // 게임보드 사이즈
    public RectInt Bounds // 보드 범위를 확인하는데 사용함
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2 - 1, -this.boardSize.y / 2 - 1);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponent<Piece>();
    }

    private void Start()
    {
        activePiece.SpawnPiece();
    }

    // Piece를 타일맵에 그림
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, piece.tiles[i]);
        }
    }

    // Piece를 지움
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // 유효한 위치인지 확인
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
}
