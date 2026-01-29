using System.Collections.Generic;
using UnityEngine;

public enum EPieceDir
{
    UP,
    RIGHT,
    DOWN,
    LEFT,
    NONE,
}

/// <summary>
/// 스폰 위치, 이동 딜레이와 같은 게임 설정
/// </summary>
public class GameManager
{
    public readonly Dictionary<EPieceDir, Vector3Int> spawnPositions = new Dictionary<EPieceDir, Vector3Int>()
    {
        {EPieceDir.UP, new Vector3Int(-1, 7)},
        {EPieceDir.RIGHT, new Vector3Int(7, -1)},
        {EPieceDir.DOWN, new Vector3Int(0, -9)},
        {EPieceDir.LEFT, new Vector3Int(-9, -1)}
    };

    public bool isPause { get; set; }
    public bool isOver { get; set; }

    // 게임 설정값들
    public float stepDelay { get; private set; } // stepDelay의 시간만큼 중심으로 이동함
    public float moveDelay { get; private set; } // 플레이어의 입력 이동 속도를 정함. 값이 클수록 입력을 많이 못함
    public float lockDelay { get; private set; } // 이 시간만큼 못 움직이면 피스를 Lock함

    public int bombSpawnLimit { get; private set; } // 폭탄 생성 한계값
    public int rocketSpawnLimit { get; private set; } // 로켓 생성 한계값
    public int obstacleSpawnLimit { get; set; } // 12 10 8 6 5

    public bool nextSpawnHasBomb { get; set; }
    public bool nextSpawnHasRocket { get; set; }

    public int bombRange { get; private set; }
    private int _blockCounter;
    public int BlockCounter
    {
        get
        {
            return _blockCounter;
        }
        set
        {
            int cnt = value - _blockCounter;
            _blockCounter = value;
            if (_blockCounter >= bombSpawnLimit)
            {
                nextSpawnHasBomb = true;
                _blockCounter = 0;
            }
            else if (cnt >= rocketSpawnLimit && nextSpawnHasRocket == false)
            {
                nextSpawnHasRocket = true;
            }
        }
    }

    public int obstacleCount { get; set; } = 0;

    public void Init()
    {
        isPause = isOver = false;

        stepDelay = 1.25f;
        moveDelay = 0.1f;
        lockDelay = 0.5f;

        bombSpawnLimit = 20;
        rocketSpawnLimit = 6;
        obstacleCount = 12;

        nextSpawnHasBomb = nextSpawnHasRocket = false;
        bombRange = 5;
        _blockCounter = 0;
    }
}
