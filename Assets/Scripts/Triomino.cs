using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 트리오미노 데이터 가짐
public enum ETriomino
{
    I,
    L
}

[System.Serializable]
public struct TriominoData
{
    public ETriomino triomino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; } 
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[triomino];
        wallKicks = Data.WallKicks[triomino];
    }
}
