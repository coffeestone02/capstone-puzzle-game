using System;
using UnityEngine;

public class SaveData
{
    public bool hasRun;

    // Board 범위
    public int xMin, yMin, width, height;

    // 타일맵 전체 저장
    public int[] boardTileIds;

    // Managers.Rule
    public int blockCounter;
    public bool nextSpawnHasBomb;
    public bool nextSpawnHasRocket;
    public int obstacleCount;
    public int obstacleSpawnLimit;

    // ScoreManager
    public float playtime;
    public int score;
    public int combo;

    // Settings (기본은 ON 상태)
    public bool sfxOff;
    public bool bgmOff;
    public bool ghostOff;

    // Piece 저장
    public bool hasPiece;

    public int triominoIndex;
    public int posX, posY;

    public int currentSpawnPos;
    public int nextSpawnPos;

    // cells
    public int[] cellX;
    public int[] cellY;

    // tiles
    public int[] pieceTileIds;

    // 아이템 버튼 횟수
    public int itemBombCnt;
    public int itemAllDestroyCnt;
    public int itemObstacleDestroyCnt;
}
