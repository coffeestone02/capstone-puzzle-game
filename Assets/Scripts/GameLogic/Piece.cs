using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum EPieceDir
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}

public class Piece
{
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노들(인스펙터에서 설정)
    private TriominoData data;
    private Vector2Int position;
    private EPieceDir spawnPos;
    private EPieceDir moveDir;

    private void SpawnPiece()
    {
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];
    }
}
