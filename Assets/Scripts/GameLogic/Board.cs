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
    }
}
