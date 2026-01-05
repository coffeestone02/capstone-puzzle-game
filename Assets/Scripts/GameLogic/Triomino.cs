using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// 트리오미노 데이터 가짐
public enum ETriomino
{
    I,
    L
}

// 인스펙터에서 설정가능하게 함
public class TriominoData
{
    public ETriomino triomino; // 어떤 타입인지
    public Tile[] normalTiles; // 어떤 타일로 그려져야 하는지
    public Vector2Int[] cells { get; private set; } // 그려져야 하는 셀들의 정보
    public Vector2Int[,] wallKicks { get; private set; } // 월킥 정보

    // 초기화 함수
    public void Init()
    {
        cells = Data.Cells[triomino];
        wallKicks = Data.WallKicks[triomino];
    }
}
