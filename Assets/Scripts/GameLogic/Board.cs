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
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)
    public Vector3Int[] spawnPositions; // 피스가 스폰될 위치(인스펙터에서 설정)
    private Vector2Int boardSize = new Vector2Int(19, 19); // 게임보드 사이즈

    // 보드 범위를 확인하는데 사용함
    public RectInt Bounds
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
        activePiece = GetComponentInChildren<Piece>();
    }
}
