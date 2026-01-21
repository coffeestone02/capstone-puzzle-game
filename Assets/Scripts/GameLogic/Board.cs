using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using UnityEngine.UIElements;

/// <summary>
/// 피스를 그려주기 위한 클래스
/// </summary>
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
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
        Piece piece = GetComponent<Piece>();
        piece.SpawnPiece();
    }

    /// <summary>
    /// Piece를 타일맵에 그림
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
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
    /// <param name="piece">조작중인 피스</param>
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    /// <summary>
    /// 조작중인 피스를 고정함
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
    public void Lock(Piece piece)
    {
        PieceMover pieceMover = GetComponent<PieceMover>();

        if (IsBoundary(piece) == false)
        {
            Set(piece);
            Managers.Audio.PlaySFX("LockSFX");
            PieceMatcher pieceMatcher = GetComponent<PieceMatcher>();
            pieceMatcher.TryMatch(piece);
        }
        else
        {
            Clear(piece);
        }

        Obstacle obstacle = GetComponent<Obstacle>();
        obstacle.SpawnObstacle();
        piece.SpawnPiece();
        pieceMover.SetStepDirection();
    }

    /// <summary>
    /// 피스의 모든 셀을 검사해 그려지는 위치가 유효한지 확인
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
    /// <param name="position">확인하고 싶은 위치</param>
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
    /// 중심 블록인지 확인
    /// </summary>
    /// <param name="pos">셀 위치</param>
    /// <returns></returns>
    public bool IsCenterCell(Vector3Int pos)
    {
        return (Vector2Int)pos == new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 피스가 사라지는 경계선인지 확인
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
    /// <returns></returns>
    public bool IsBoundary(Piece piece)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int pos = piece.cells[i] + piece.position;

            if (piece.currentSpawnPos == EPieceDir.UP && pos.y <= bounds.yMin + 1)
                return true;
            if (piece.currentSpawnPos == EPieceDir.RIGHT && pos.x <= bounds.xMin + 1)
                return true;
            if (piece.currentSpawnPos == EPieceDir.DOWN && pos.y >= bounds.yMax - 2)
                return true;
            if (piece.currentSpawnPos == EPieceDir.LEFT && pos.x >= bounds.xMax - 2)
                return true;
        }

        return false;
    }
}
