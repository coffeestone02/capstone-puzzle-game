using System.Collections.Generic;
using UnityEngine;

// 월킥 테이블과 회전 행렬. 정적 변수로 만들어서 접근하기 쉽게 만듦
public static class Data
{
    // 행렬 회전에 쓰일 변수들
    // public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    // public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    // public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    // 셀들이 그려질 위치. 셀들이 그려져서 피스가 만들어짐
    public static readonly Dictionary<ETriomino, Vector2Int[]> Cells = new Dictionary<ETriomino, Vector2Int[]>()
    {
        { ETriomino.I, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) } },
        { ETriomino.L, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int( 0, 0), new Vector2Int( 1, 0) } },
    };

    // I형 월킥 데이터
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int( 0, 1), new Vector2Int( 1, 1), new Vector2Int(-1,-1), new Vector2Int( 1,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-1,-1), new Vector2Int( 1,-1), new Vector2Int( 1, 1) },
    };

    // L형 월킥 데이터
    private static readonly Vector2Int[,] WallKicksL = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int( 0, 1), new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int( 1,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 0,-1), new Vector2Int(-1, 1), new Vector2Int(-1,-1), new Vector2Int( 1, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-1, 1), new Vector2Int( 1, 1), new Vector2Int(-1,-1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int( 0, 1), new Vector2Int( 0,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 0,-1), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(-1,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 0, 1), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(-1,-1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 0,-1), new Vector2Int(-1,-1), new Vector2Int( 1, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-1,-1), new Vector2Int(-1, 1), new Vector2Int( 0, 1) },
    };

    // 월킥 데이터를 쉽게 가져오기 위한 딕셔너리
    public static readonly Dictionary<ETriomino, Vector2Int[,]> WallKicks = new Dictionary<ETriomino, Vector2Int[,]>()
    {
        { ETriomino.I, WallKicksI },
        { ETriomino.L, WallKicksL },
    };

}
