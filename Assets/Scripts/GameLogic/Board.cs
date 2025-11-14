using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using System;
using TMPro;

// 블록을 관리하는 보드
public class Board : MonoBehaviour
{
    [Header("필요한 컴포넌트와 관련 변수")]
    public GameManager gameManager; // UI 연결용
    public GameObject destroyParticle; // 파괴 이펙트
    public GameObject bombParticle; // 폭발 파티클 (인스펙터에서 설정)
    public event Action<int> OnBombChanged;
    public ObstacleSpawner obstacleSpawner;
    public PieceDestroyer pieceDestroyer;
    public ItemSpawner itemSpawner;

    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)

    public int obstacleThreshold = 12; // n번째마다 장애물 스폰
    public int bombSpawnThreshold = 20; // 폭탄 생성 임계값
    public int rocketThreshold = 6;   // 로켓 생성 임계값

    public int obstacleCounter = 0; // 장애물 스폰 카운트
    private int brokenBlockCount = 0; // 부숴진 블록 카운트

    private bool nextSpawnHasBomb = false; // 다음 스폰에 폭탄 넣을지
    private bool nextSpawnHasRocket = false; // 다음 스폰에 로켓 넣을지

    [Header("보드의 범위 및 피스 스폰 위치 관련 변수")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    public Vector3Int[] spawnPositions; // 피스가 스폰될 위치(인스펙터에서 설정)
    public Vector2Int boardSize; // 게임보드 사이즈
    public int currentSpawnIdx = 0; // 스폰 위치 인덱스. 0 : 위쪽, 1 : 오른쪽, 2 : 아래쪽, 3: 왼쪽

    // 보드 범위를 확인하는데 사용함
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2 - 1, -this.boardSize.y / 2 - 1);
            return new RectInt(position, this.boardSize);
        }
    }

    [Header("점수와 관련된 변수")]
    public int[] difficultyLines = { 50000, 100000 };
    public int[] obstacleByDifficulty = { 10, 8 };
    public int score { get; private set; }
    private int combo = 0;
    public int level = 1;

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        boardSize = new Vector2Int(height, width);

        // 인스펙터에서 만들어진 트리오미노의 수만큼 진행
        for (int i = 0; i < triominos.Length; i++)
        {
            triominos[i].Initialize();
        }
    }

    void Start()
    {
        SpawnPiece();
    }

    private void Update()
    {
        if (gameManager.isOver || gameManager.isPause)
        {
            return;
        }

        SetDifficulty();
    }

    private void SetDifficulty()
    {
        for (int idx = 0; idx < difficultyLines.Length; idx++)
        {
            if (level < 3 && score >= difficultyLines[idx])
            {
                obstacleThreshold = obstacleByDifficulty[idx];
                if (AudioManager.instance.bgmPlayer.clip != AudioManager.instance.bgmClips[idx])
                {
                    AudioManager.instance.bgmPlayer.clip = AudioManager.instance.bgmClips[idx];
                    AudioManager.instance.bgmPlayer.Play();
                }
                level = idx + 2;
            }
        }
    }

    // 지정된 위치에 트리오미노를 랜덤으로 골라 스폰
    public void SpawnPiece()
    {
        obstacleCounter++;
        if (obstacleCounter >= obstacleThreshold)
        {
            obstacleSpawner.SpawnObstacle();
            obstacleCounter = 0;
        }

        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];

        activePiece.Initialize(this, spawnPositions[currentSpawnIdx], data);

        if (!IsValidPosition(activePiece, activePiece.position)) //블록 생성 후 겹칠 시 게임 오버
        {
            gameManager.isOver = true;
            return;
        }

        // 방향에 맞게 회전
        switch (currentSpawnIdx)
        {
            case 1:
                activePiece.Rotate(1, true);
                break;
            case 2:
                activePiece.Rotate(1, true);
                activePiece.Rotate(1, true);
                break;
            case 3:
                activePiece.Rotate(1, true);
                activePiece.Rotate(1, true);
                activePiece.Rotate(1, true);
                break;
        }

        if (nextSpawnHasBomb) // 폭탄 추가
        {
            activePiece = itemSpawner.AddBombTile(activePiece);
            nextSpawnHasBomb = false;
        }
        else if (nextSpawnHasRocket) // 로켓 추가 (십자형)
        {
            activePiece = itemSpawner.AddRocketTile(activePiece);
            nextSpawnHasRocket = false;
        }

        Set(activePiece);
    }

    // Piece를 타일에 그림
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            tilemap.SetTile(piece.cells[i] + piece.position, piece.tiles[i]);
        }
    }

    // Piece를 지움
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    // 셀의 자리가 유효한지 검사하는 함수
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        // 각 셀마다 검사해야함
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // 보드 범위 안에 있는지 검사
            if (bounds.Contains((Vector2Int)tilePosition) == false)
            {
                return false;
            }

            // 이미 타일이 있는지 검사
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    // 스폰 위치를 계산
    public void NextSpawnIdx()
    {
        currentSpawnIdx++;
        currentSpawnIdx = currentSpawnIdx % 4;
    }

    // 매칭 시도 및 점수 계산
    public void TryMatch(Piece piece)
    {
        HashSet<Vector3Int> matched = pieceDestroyer.FindMainMatch(piece); // 메인 피스 매칭
        HashSet<Vector3Int> bonusMatched = pieceDestroyer.FindBonusMatch(matched); // 추가 제거 매칭

        // 매치/보너스만 카운트(아이템으로 인한 파괴는 제외)
        int clearedByMatchOnly = matched.Count + bonusMatched.Count;
        brokenBlockCount += clearedByMatchOnly;

        int mainPoint = matched.Count * 100; // 메인 피스 점수 계산
        int bonusPoint = bonusMatched.Count * 60; // 추가 제거 점수 계산

        pieceDestroyer.DeleteMatchedPiece(matched); // 메인 피스 제거

        if (matched.Count == 0)
        {
            combo = 0;
        }
        else
        {
            AudioManager.instance.PlayClearSound();
        }

        pieceDestroyer.DeleteMatchedPiece(bonusMatched); // 추가 피스 제거

        // 폭탄과 로켓 스폰(로켓 폭발로 지운 칸은 포함하지 않음)
        if (brokenBlockCount >= bombSpawnThreshold)
        {
            brokenBlockCount = 0;
            nextSpawnHasBomb = true;
        }
        else if (clearedByMatchOnly >= rocketThreshold && nextSpawnHasRocket == false)
        {
            nextSpawnHasRocket = true;
        }

        if (clearedByMatchOnly > 0)
        {
            combo++;
        }

        // 최종 점수 계산 (로켓 폭발 점수는 FireCrossRocketAt, 폭탄 폭발 점수는 ExplodeBomb에서 바로 계산됨)
        score += (mainPoint + bonusPoint) * (1 + combo) * (int)(1 + 0.1 * level);
        if (OnBombChanged != null)
        {
            OnBombChanged.Invoke(brokenBlockCount);
        }
        else
        {
            Debug.LogError("TryMatch(): OnBombChanged is null.");
        }
    }

}
