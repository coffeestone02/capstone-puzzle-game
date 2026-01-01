using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData
{
    public bool hasRun;

    // 보드 범위 + 고정 블록(현재 피스 제외)
    public int xMin, yMin, width, height;
    public int[] fixedTiles; // 0=empty, 1..N=tileId

    // 진행 상태
    public int score;
    public int level;
    public int obstacleLimit;
    public int obstacleCounter;
    public int brokenBlockCount;
    public bool nextSpawnHasBomb;
    public bool nextSpawnHasRocket;
    public int currentSpawnIdx;

    // 현재 피스
    public int triominoIndex;
    public int posX, posY;
    public int rotationIdx;
    public int[] pieceTileIds; // 길이 3
    public float stepRemain;   // stepTime - Time.time
    public float lockTime;
}

