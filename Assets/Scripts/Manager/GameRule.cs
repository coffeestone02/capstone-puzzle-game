using System.Collections.Generic;
using UnityEngine;

public enum EPieceDir
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

public class GameRule
{
    public readonly Dictionary<EPieceDir, Vector3Int> spawnPositions = new Dictionary<EPieceDir, Vector3Int>()
    {
        {EPieceDir.TOP, new Vector3Int(-1, 7)},
        {EPieceDir.RIGHT, new Vector3Int(7, -1)},
        {EPieceDir.BOTTOM, new Vector3Int(0, -10)},
        {EPieceDir.LEFT, new Vector3Int(-9, -1)}
    };
    private int bombSpawnLimit = 20; // 폭탄 생성 한계값
    private int rocketSpawnLimit = 6;   // 로켓 생성 한계값

}
