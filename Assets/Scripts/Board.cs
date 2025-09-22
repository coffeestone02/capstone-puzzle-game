using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using System;

// 블록을 관리하는 보드
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)
    public Vector3Int[] spawnPositions; // 피스가 스폰될 위치(인스펙터에서 설정)
    private Vector2Int boardSize; // 게임보드 사이즈
    public GameManager gameManager; // UI 연결용
    public Tile grayTile; // 가장자리에 닿으면 변하는 타일
    public GameObject destroyParticles;
    public float playTime { get; private set; }

    // ⭐ 보호 Tile 변수 추가
    public Tile protectedFloorTile;

    // 폭탄 파티클 크기 조절 변수 이름 복원
    public float BombParticleScale = 0.2f;

    public bool isMatching { get; private set; }
    [SerializeField] private int width;
    [SerializeField] private int height;

    // 폭탄 관련 변수 추가
    public Tile bombTile; // 폭탄 블록 이미지 (인스펙터에서 설정)
    public GameObject bombParticles; // 폭발 파티클 (인스펙터에서 설정)
    private int clearedBlockCount = 0; // 부숴진 블록 카운트
    private const int BombSpawnThreshold = 10; // 폭탄 생성 임계값
    private bool isBombExploding = false; // 폭탄 폭발 중인지 확인

    // 폭탄이 Lock되어 보드에 고정되었을 때의 위치를 추적 (다음 폭탄 생성 제어용)
    private Vector3Int? lockedBombPosition = null;

    private bool spawnNextPieceAsBomb = false; // 다음에 스폰될 Piece에 폭탄을 심을지 여부
    private int bombCellIndexInActivePiece = -1; // ActivePiece 내에서 폭탄 타일이 있는 cells 인덱스

    public RectInt Bounds // 보드 범위를 확인하는데 사용함.
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2 - 1, -this.boardSize.y / 2 - 1);
            return new RectInt(position, this.boardSize);
        }
    }

    // 회색 블록 탐색을 위한 방향 벡터
    private Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    // 임시 난이도. 나중에 기준치 높일 것
    public int[] difficultyLines = { 3000, 4000, 5000 };

    public int score { get; private set; }
    private int combo = 0;
    private int level = 1;
    public int currentSpawnIdx = 0; // 스폰 위치 인덱스. 0 : 위쪽, 1 : 오른쪽, 2 : 아래쪽, 3: 왼쪽

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
        playTime += Time.deltaTime;

        // 폭발 중이라면 다른 로직 실행 방지
        if (isBombExploding)
        {
            return;
        }
    }

    // 지정된 위치에 트리오미노를 랜덤으로 골라 스폰
    public void SpawnPiece()
    {
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];

        activePiece.Initialize(this, spawnPositions[currentSpawnIdx], data);
        bombCellIndexInActivePiece = -1; // 새 피스 스폰 시 인덱스 초기화

        // 폭탄 블록 생성 로직: 다음에 스폰될 블록에 폭탄을 심음
        if (spawnNextPieceAsBomb)
        {
            // 랜덤으로 한 셀을 폭탄 블록으로 변경
            bombCellIndexInActivePiece = UnityEngine.Random.Range(0, activePiece.cells.Length);

            // Piece의 tiles 배열을 수정하여 폭탄 타일로 변경
            activePiece.tiles[bombCellIndexInActivePiece] = bombTile;

            // 폭탄 블록이 ActivePiece에 심겼음을 표시. Lock되기 전까지는 lockedBombPosition은 null 유지
            spawnNextPieceAsBomb = false; // 플래그 리셋
        }


        if (!IsValidPosition(activePiece, activePiece.position)) //블록 생성 후 겹칠 시 게임 오버
        {
            gameManager.GameOver();
            return;
        }

        switch (currentSpawnIdx)
        {
            case 1:
                activePiece.Rotate(1);
                break;
            case 2:
                activePiece.Rotate(1);
                activePiece.Rotate(1);
                break;
            case 3:
                activePiece.Rotate(1);
                activePiece.Rotate(1);
                activePiece.Rotate(1);
                break;
        }

        Set(activePiece);
    }

    // Piece를 타일에 그림
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            Tile tileToSet = piece.tiles[i];

            tilemap.SetTile(tilePosition, tileToSet);
        }
    }

    private bool InEdge(int xPos, int yPos)
    {
        RectInt bounds = this.Bounds;
        if (xPos <= bounds.xMin + 1 || xPos >= bounds.xMax - 2 ||
            yPos <= bounds.yMin + 1 || yPos >= bounds.yMax - 2)
        {
            return true;
        }

        return false;
    }

    // 가장자리 낙하시 회색 블록으로 변화
    public void ChangeGray(Piece piece)
    {
        bool EdgeOrGray = false;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPos = piece.cells[i] + piece.position;

            // 보드 가장자리인지 확인
            if (InEdge(cellPos.x, cellPos.y))
            {
                EdgeOrGray = true;
                break;
            }
        }

        // 조건 만족 시 전체 블록 회색으로 변경
        if (EdgeOrGray)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, grayTile);
            }
        }
    }

    // 게임 오버인지 확인
    public bool IsGameover(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPos = piece.cells[i] + piece.position;

            // 보드 가장자리인지 확인
            if (InEdge(cellPos.x, cellPos.y))
            {
                gameManager.isOver = true;
                return true;
            }
        }

        return false;
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
            if (this.tilemap.HasTile(tilePosition))
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

    // 매칭 시도 및 점수 획득
    public void TryMatch(Piece piece)
    {
        isMatching = true;

        // 1. 폭탄 고정 시 폭발
        if (bombCellIndexInActivePiece != -1)
        {
            Vector3Int bombPos = piece.cells[bombCellIndexInActivePiece] + piece.position;
            lockedBombPosition = bombPos; // 폭탄 위치 기록
            bombCellIndexInActivePiece = -1;

            ExplodeBomb(bombPos);

            // 폭탄이 터졌으므로, 이 턴의 나머지 매칭 로직은 스킵합니다.
            isMatching = false;
            return;
        }
        
        // 2. 일반 매칭 및 보너스 매칭 진행 (폭탄이 없을 때만)

        int mainPoint = 0;
        int totalClearedCount = 0;

        HashSet<Vector3Int> matched = FindMainMatch(piece); // 메인 피스 매칭
        mainPoint += matched.Count * 100;
        totalClearedCount += matched.Count;
        DeleteMatchedPiece(matched); // 일반 제거

        if (matched.Count == 0)
        {
            combo = 0;
        }

        // 보너스 매칭 계산
        HashSet<Vector3Int> bonusMatched = FindBonusMatch(matched);
        int bonusPoint = bonusMatched.Count * 60;
        totalClearedCount += bonusMatched.Count;
        DeleteMatchedPiece(bonusMatched); // 보너스 제거

        score += (mainPoint + bonusPoint) * (1 + combo) * (int)(1 + 0.1 * level); // 최종 점수 계산

        // 부숴진 블록 카운트 증가 및 폭탄 생성 플래그 설정
        clearedBlockCount += totalClearedCount;

        // lockedBombPosition이 null일 때만 (현재 보드에 Lock된 폭탄이 없을 때만) 다음 폭탄을 예약
        if (clearedBlockCount >= BombSpawnThreshold && !lockedBombPosition.HasValue)
        {
            spawnNextPieceAsBomb = true;
            clearedBlockCount = 0; // 폭탄 생성 플래그를 켰으므로 카운트 초기화
        }

        isMatching = false;
    }

    // 매치된 피스를 제거
    private void DeleteMatchedPiece(HashSet<Vector3Int> matched)
    {
        // 이 함수는 TryMatch에서 폭탄이 터지지 않은 경우에만 호출됩니다.

        foreach (Vector3Int pos in matched)
        {
            if ((Vector2Int)pos == new Vector2Int(-1, -1)) // 중앙 블록
            {
                continue;
            }

            // 파티클 효과 함수 호출
            PlayDestroyParticles(pos);
            tilemap.SetTile(pos, null);
        }
    }

    // 메인 피스 매칭
    private HashSet<Vector3Int> FindMainMatch(Piece piece)
    {
        HashSet<Vector3Int> matched = new HashSet<Vector3Int>();
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int[] connections = FindConnections(piece, i);
            if (CanDelete(connections))
            {
                foreach (Vector3Int pos in connections)
                {
                    matched.Add(pos);
                }
            }
        }

        if (matched.Count > 0)
        {
            combo++;
        }

        return matched;
    }

    // 추가로 제거할 수 있는 피스 찾기
    private HashSet<Vector3Int> FindBonusMatch(HashSet<Vector3Int> matched)
    {
        HashSet<Vector3Int> bonusMatched = new HashSet<Vector3Int>();
        if (matched == null || matched.Count == 0)
        {
            return bonusMatched;
        }

        RectInt bounds = this.Bounds;

        // 이미 찾아진 x/y 좌표 모으기
        HashSet<int> xCross = new HashSet<int>();
        HashSet<int> yCross = new HashSet<int>();

        foreach (Vector3Int pos in matched)
        {
            xCross.Add(pos.x);
            yCross.Add(pos.y);
        }

        // 가로/세로 교차점(새로운 제거 후보 위치) 전부 찾기
        foreach (int x in xCross)
        {
            foreach (int y in yCross)
            {
                Vector3Int cross = new Vector3Int(x, y, 0);

                // 이미 기존 matched에 들어있거나(중복방지) 범위 밖이면 패스
                if (matched.Contains(cross) || bounds.Contains((Vector2Int)cross) == false)
                {
                    continue;
                }

                // 실제로 타일이 있어야만 추가
                if (tilemap.HasTile(cross))
                {
                    bonusMatched.Add(cross);
                }
            }
        }

        return bonusMatched;
    }

    // 연결이 4개 이상이면서 일자 모양이 아닌 경우 삭제 가능
    private bool CanDelete(Vector3Int[] connections)
    {
        if (connections.Length < 4)
        {
            return false;
        }

        // x좌표가 모두 같거나 y좌표가 모두 동일하면 일자모양
        int x = connections[0].x;
        int y = connections[0].y;
        bool xCrosstraightCheck = true;
        bool yStraightCheck = true;
        for (int i = 1; i < connections.Length; i++)
        {
            if (connections[i].x != x)
            {
                xCrosstraightCheck = false;
            }

            if (connections[i].y != y)
            {
                yStraightCheck = false;
            }
        }

        if (xCrosstraightCheck || yStraightCheck)
        {
            return false;
        }

        return true;
    }

    // 연결을 확인함
    private Vector3Int[] FindConnections(Piece piece, int cellIdx)
    {
        Vector3Int start = piece.cells[cellIdx] + piece.position;
        Tile matchTile = piece.tiles[cellIdx];

        List<Vector3Int> connections = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        RectInt bounds = this.Bounds;

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            connections.Add(current);

            foreach (Vector3Int dir in directions)
            {
                Vector3Int next = current + dir;

                if (bounds.Contains((Vector2Int)next) == false || visited.Contains(next))
                {
                    continue;
                }

                Tile nextTile = tilemap.GetTile<Tile>(next);
                if (nextTile == null || nextTile != matchTile)
                {
                    continue;
                }

                queue.Enqueue(next);
                visited.Add(next);
            }
        }

        return connections.ToArray();
    }

    // 폭탄 폭발 처리
    public void ExplodeBomb(Vector3Int center) // 폭탄 위치를 인수로 받음
    {
        if (isBombExploding) return;

        isBombExploding = true;

        // 5x5 폭발 범위 계산
        int radius = 2; // (5-1)/2 = 2
        HashSet<Vector3Int> explodedCells = new HashSet<Vector3Int>();
        RectInt bounds = this.Bounds;
        int totalCleared = 0;

        // 파괴를 방지할 특정 좌표들
        Vector3Int protectedCenter = new Vector3Int(0, 0, 0);


        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                // Z 좌표는 Tilemap 레이어에 따라 0으로 고정하여 검사합니다.
                Vector3Int pos = new Vector3Int(x, y, 0);

                // 맵 중심 블록 (0, 0, 0)을 파괴하지 않음
                if (pos == protectedCenter)
                {
                    continue;
                }

                // 타일을 가져와서 보호 Tile인지 확인합니다.
                Tile currentTile = tilemap.GetTile<Tile>(pos);

                // 보호 Tile이 설정되어 있고 현재 타일이 보호 Tile과 일치하면 파괴를 건너뜁니다.
                if (protectedFloorTile != null && currentTile == protectedFloorTile)
                {
                    continue;
                }

                // 보드 범위 내에 있고, 타일이 있는 경우
                if (bounds.Contains((Vector2Int)pos) && currentTile != null)
                {
                    explodedCells.Add(pos);
                }
            }
        }

        // 폭발 범위 내의 블록 제거
        foreach (Vector3Int pos in explodedCells)
        {
            // 폭탄 블록 위치는 폭발 파티클로, 나머지 블록은 일반 파티클로 구분
            if (pos == center)
            {
                PlayDestroyParticles(pos, bombParticles);
            }
            else
            {
                PlayDestroyParticles(pos); // 일반 블록 파티클만 재생
            }

            // 블록 파괴 기능을 유지
            tilemap.SetTile(pos, null);
            totalCleared++;
        }

        // 폭발에 의한 점수 획득 (100점 * 폭발 블록 수, 폭탄 블록 자신은 점수에서 제외)
        int scoreCount = totalCleared - 1;
        if (scoreCount > 0)
        {
            int explosionScore = scoreCount * 100;
            score += explosionScore;
        }

        // 폭발 후 lockedBombPosition 초기화: 폭탄이 제거되었다고 처리하여 다음 폭탄 생성을 허용
        lockedBombPosition = null;
        isBombExploding = false;
    }


    // 파티클 효과를 생성하고 재생하는 함수
    private void PlayDestroyParticles(Vector3 position, GameObject particlePrefab = null)
    {
        // particlePrefab이 null이면 destroyParticles(일반 블록 파티클)을 사용
        GameObject prefabToUse = (particlePrefab == null) ? destroyParticles : particlePrefab;

        if (prefabToUse == null)
        {
            return;
        }

        GameObject particles = Instantiate(prefabToUse, tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);

        // 폭탄 파티클일 경우에만 BombParticleScale 적용
        if (prefabToUse == bombParticles)
        {
            particles.transform.localScale = new Vector3(BombParticleScale, BombParticleScale, BombParticleScale);
        }
        else
        {
            // 일반 블록 파티클은 크기 (0.2f) 고정
            particles.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        Destroy(particles, 1f); // 1초 뒤에 파괴
    }
}