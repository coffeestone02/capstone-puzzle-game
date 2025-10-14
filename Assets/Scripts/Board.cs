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

    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)

    public Tile obstacleTile; // 장애물 타일(1x1)
    private int spawnObstacleCounter = 0;  // 장애물 스폰 카운트
    public int obstacleEverySpawns = 12; // n번째마다 장애물 스폰

    [SerializeField] private int rocketThreshold = 6;   // 로켓 조건, 한 번에 n개 제거하면 다음 피스에 로켓 포함
    private bool nextSpawnHasRocket = false; // 다음 스폰에 로켓 넣을지
    private enum ColorId { Unknown, Purple, Blue, Red } // 색 아이디/유틸 

    // 색상별 십자 로켓 타일(가로+세로 동시 폭발)
    public Tile rocketCross_Purple;
    public Tile rocketCross_Blue;
    public Tile rocketCross_Red;

    // 폭탄 관련 변수 추가
    private int brokenBlockCount = 0; // 부숴진 블록 카운트
    [SerializeField] private int bombSpawnThreshold = 20; // 블록을 이 변수값만큼 제거하면 다음 피스에 폭탄 포함
    private bool nextSpawnHasBomb = false; // 다음 스폰에 폭탄 넣을지

    // 폭탄 타일
    public Tile bomb_Purple;
    public Tile bomb_Blue;
    public Tile bomb_Red;

    // 2-Bag 방향 관리
    private enum DropDir { Up, Right, Down, Left }
    private List<DropDir> dirBag = new List<DropDir>();
    private int dirBagIdx = 0;

    // 각 피스의 출발 방향 저장(해당 피스의 '반대쪽 모서리'를 소멸 조건으로 사용)
    private Dictionary<Piece, DropDir> pieceSpawnFrom = new Dictionary<Piece, DropDir>();


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

    // 회색 블록 탐색을 위한 방향 벡터
    private Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    [Header("점수 및 UI와 관련 변수")]
    public int[] difficultyLines = { 50000, 100000 };
    public int[] obstacleByDifficulty = { 10, 8 };
    public float playTime { get; private set; }
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text playtimeText;
    public TMP_Text brokenBlockText;
    public TMP_Text bombThresholdText;
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

        // Bag 초기화
        RefillBag();
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
        TimeTextUpdate();
    }

    private void SetDifficulty()
    {
        for (int idx = 0; idx < difficultyLines.Length; idx++)
        {
            if (level < 3 && score >= difficultyLines[idx])
            {
                obstacleEverySpawns = obstacleByDifficulty[idx];
                if (AudioManager.instance.bgmPlayer.clip != AudioManager.instance.bgmClips[idx])
                {
                    AudioManager.instance.bgmPlayer.clip = AudioManager.instance.bgmClips[idx];
                    AudioManager.instance.bgmPlayer.Play();
                }

                level = idx + 2;
                levelText.text = level.ToString();
            }
        }
    }

    private void TimeTextUpdate()
    {
        playTime += Time.deltaTime;
        string min;
        string sec;

        // 분
        if (playTime < 60f)
        {
            min = "00";
        }
        else if (playTime < 600f)
        {
            min = "0" + Math.Truncate(playTime / 60f).ToString();
        }
        else
        {
            min = Math.Truncate(playTime / 60f).ToString();
        }

        // 초
        if (Math.Truncate(playTime % 60f) < 10f)
        {
            sec = "0" + Math.Truncate(playTime % 60f).ToString();
        }
        else
        {
            sec = Math.Truncate(playTime % 60f).ToString();
        }

        playtimeText.text = min + ":" + sec;
    }

    // 지정된 위치에 트리오미노를 랜덤으로 골라 스폰
    public void SpawnPiece()
    {
        if (obstacleTile != null && obstacleEverySpawns > 0) //새 피스 뽑기 '직전'에 장애물 1줄 드랍
        {
            spawnObstacleCounter++;
            if (spawnObstacleCounter % obstacleEverySpawns == 0)
            {
                DropDir dir = NextBagDir();   // 1-bag에서 다음 방향
                DropObstacleLine(dir);
            }
        }

        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];

        activePiece.Initialize(this, spawnPositions[currentSpawnIdx], data);

        if (!IsValidPosition(activePiece, activePiece.position)) //블록 생성 후 겹칠 시 게임 오버
        {
            gameManager.isOver = true;
            return;
        }

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

        // 이 피스의 출발 방향 기록(해당 피스의 소멸 모서리 계산용)
        pieceSpawnFrom[activePiece] = (DropDir)currentSpawnIdx;

        if (nextSpawnHasBomb) // 폭탄 추가
        {
            int cell = UnityEngine.Random.Range(0, activePiece.cells.Length); // 3칸 중 1칸만
            TileBase baseTile = activePiece.tiles[cell];                      // 그 칸의 색을 읽음
            ColorId c = GetColorId(baseTile);
            Tile bombVariant = ChooseBombForColor(c);

            if (bombVariant != null) // 색을 알 수 없으면 넣지 않음
                activePiece.tiles[cell] = bombVariant;

            nextSpawnHasBomb = false;
        }
        else if (nextSpawnHasRocket) // 로켓 추가 (십자형)
        {
            int cell = UnityEngine.Random.Range(0, activePiece.cells.Length); // 3칸 중 1칸만
            TileBase baseTile = activePiece.tiles[cell];                      // 그 칸의 색을 읽음
            ColorId c = GetColorId(baseTile);
            Tile rocketVariant = ChooseCrossRocketForColor(c);

            if (rocketVariant != null) // 색을 알 수 없으면 넣지 않음
                activePiece.tiles[cell] = rocketVariant;

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

    // 매칭 시도 및 점수 획득
    public void TryMatch(Piece piece)
    {
        int mainPoint = 0;
        int bonusPoint = 0;

        HashSet<Vector3Int> matched = FindMainMatch(piece); // 메인 피스 매칭
        HashSet<Vector3Int> bonusMatched = FindBonusMatch(matched); // 추가 제거 매칭

        // 매치/보너스만 카운트(로켓 폭발분은 카운트 제외)
        int clearedByMatchOnly = matched.Count + bonusMatched.Count;
        brokenBlockCount += clearedByMatchOnly;
        brokenBlockText.text = brokenBlockCount.ToString();

        mainPoint = matched.Count * 100; // 메인 피스 점수 계산
        bonusPoint = bonusMatched.Count * 60; // 추가 제거 점수 계산

        DeleteMatchedPiece(matched); // 메인 피스 제거

        if (matched.Count == 0)
        {
            combo = 0;
        }
        else
        {
            AudioManager.instance.PlayClearSound();
        }

        DeleteMatchedPiece(bonusMatched); // 추가 피스 제거

        // 폭탄과 로켓 스폰(로켓 폭발로 지운 칸은 포함하지 않음)
        if (brokenBlockCount >= bombSpawnThreshold)
        {
            brokenBlockCount = 0;
            brokenBlockText.text = brokenBlockCount.ToString();
            nextSpawnHasBomb = true;
        }
        else if (clearedByMatchOnly >= rocketThreshold && !nextSpawnHasRocket)
        {
            nextSpawnHasRocket = true;
        }

        if (clearedByMatchOnly > 0)
        {
            combo++;
        }

        // 최종 점수 계산 (로켓 폭발 점수는 FireCrossRocketAt, 폭탄 폭발 점수는 ExplodeBomb에서 바로 계산됨)
        score += (mainPoint + bonusPoint) * (1 + combo) * (int)(1 + 0.1 * level);
        scoreText.text = score.ToString();
    }

    // 매치된 피스를 제거
    private void DeleteMatchedPiece(HashSet<Vector3Int> matched)
    {
        if (matched == null || matched.Count == 0)
            return;

        // 로켓 먼저 수집
        List<Vector3Int> crossRockets = new List<Vector3Int>();
        foreach (Vector3Int pos in matched)
        {
            TileBase tb = tilemap.GetTile<TileBase>(pos);
            if (tb == null) continue;
            if (IsCrossRocket(tb)) crossRockets.Add(pos);
        }

        // 폭탄 수집
        List<Vector3Int> bombs = new List<Vector3Int>();
        foreach (Vector3Int pos in matched)
        {
            TileBase tb = tilemap.GetTile<TileBase>(pos);
            if (tb == null) continue;
            if (IsBomb(tb)) bombs.Add(pos);
        }

        // 일반 제거(중앙 보호)
        foreach (Vector3Int pos in matched)
        {
            if ((Vector2Int)pos == new Vector2Int(-1, -1)) // 중앙 블록 보호
                continue;

            TileBase tb = tilemap.GetTile<TileBase>(pos);
            if (tb == null) continue;

            PlayDestroyParticle(destroyParticle, pos);
            tilemap.SetTile(pos, null);
        }

        if(bombs.Count > 0)
            AudioManager.instance.PlayBombSound();
        foreach (var b in bombs)
        {
            ExplodeBomb(b);
        }

        // 로켓 발사 (제거 이후 십자 처리) — 여기서 점수 60/칸씩 즉시 가산, 카운트에는 미포함
        foreach (var r in crossRockets)
        {
            FireCrossRocketAt(r);
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
        bool xStraightCheck = true;
        bool yStraightCheck = true;
        for (int i = 1; i < connections.Length; i++)
        {
            if (connections[i].x != x)
            {
                xStraightCheck = false;
            }

            if (connections[i].y != y)
            {
                yStraightCheck = false;
            }
        }

        if (xStraightCheck || yStraightCheck)
        {
            return false;
        }

        return true;
    }

    // 연결을 확인함 (색 동치로 탐색)
    private Vector3Int[] FindConnections(Piece piece, int cellIdx)
    {
        Vector3Int start = piece.cells[cellIdx] + piece.position;
        TileBase matchTile = tilemap.GetTile<TileBase>(start); // 시작칸의 '색' 기준

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

                TileBase nextTile = tilemap.GetTile<TileBase>(next);
                if (nextTile == null) continue;

                // 핵심: 같은 색이면 연결 (일반/로켓 구분 없이)
                if (!IsSameColor(nextTile, matchTile))
                {
                    continue;
                }

                queue.Enqueue(next);
                visited.Add(next);
            }
        }

        return connections.ToArray();
    }

    // 파티클 효과를 생성하고 재생하는 함수
    private void PlayDestroyParticle(GameObject effect, Vector3 position)
    {
        if (effect == null)
        {
            return;
        }

        float scaleMultiplier;
        if (effect == bombParticle)
        {
            scaleMultiplier = 0.8f;
        }
        else
        {
            scaleMultiplier = 0.2f;
        }

        GameObject particles = Instantiate(effect, tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        particles.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        Destroy(particles, 1f); // 1초 뒤에 파괴
    }

    private ColorId GetColorId(TileBase t)
    {
        if (t == null) return ColorId.Unknown;

        // 십자 로켓 변형 → 색 매핑
        if (t == rocketCross_Purple) return ColorId.Purple;
        if (t == rocketCross_Blue) return ColorId.Blue;
        if (t == rocketCross_Red) return ColorId.Red;

        // 일반 타일 이름 기반(프로젝트 네이밍에 맞게 보강 가능)
        string n = t.name.ToLowerInvariant();
        if (n.Contains("purple") || n.Contains("보")) return ColorId.Purple;
        if (n.Contains("blue") || n.Contains("파")) return ColorId.Blue;
        if (n.Contains("red") || n.Contains("빨")) return ColorId.Red;

        return ColorId.Unknown;
    }

    private bool IsSameColor(TileBase a, TileBase b)
    {
        var ca = GetColorId(a);
        var cb = GetColorId(b);
        return ca != ColorId.Unknown && ca == cb;
    }

    private bool IsCrossRocket(TileBase t)
    {
        return t == rocketCross_Purple || t == rocketCross_Blue || t == rocketCross_Red;
    }

    private bool IsBomb(TileBase t)
    {
        return t == bomb_Blue || t == bomb_Purple || t == bomb_Red;
    }

    private Tile ChooseCrossRocketForColor(ColorId c)
    {
        switch (c)
        {
            case ColorId.Purple: return rocketCross_Purple;
            case ColorId.Blue: return rocketCross_Blue;
            case ColorId.Red: return rocketCross_Red;
            default: return null;
        }
    }

    private Tile ChooseBombForColor(ColorId c)
    {
        switch (c)
        {
            case ColorId.Purple: return bomb_Purple;
            case ColorId.Blue: return bomb_Blue;
            case ColorId.Red: return bomb_Red;
            default: return null;
        }
    }

    private void RefillBag()
    {
        dirBag.Clear();
        dirBag.AddRange(new[] { DropDir.Up, DropDir.Right, DropDir.Down, DropDir.Left }); // 셔플

        for (int i = 0; i < dirBag.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, dirBag.Count);
            (dirBag[i], dirBag[j]) = (dirBag[j], dirBag[i]);
        }
        dirBagIdx = 0;
    }

    private DropDir NextBagDir()
    {
        if (dirBag.Count == 0 || dirBagIdx >= dirBag.Count)
            RefillBag();
        return dirBag[dirBagIdx++];
    }
    // 모서리 닿았는지
    private bool IsTouchingEdge(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPos = piece.cells[i] + piece.position;
            if (InEdge(cellPos.x, cellPos.y))
                return true;
        }
        return false;
    }

    // 피스가 낙하 반대편 모서리에 닿았는지 검사
    private bool IsTouchingVanishEdge(Piece piece)
    {
        if (!pieceSpawnFrom.TryGetValue(piece, out DropDir from))
        {
            return IsTouchingEdge(piece);
        }

        RectInt b = this.Bounds;
        int vanishXLeft = b.xMin + 1;
        int vanishXRight = b.xMax - 2;
        int vanishYBottom = b.yMin + 1;
        int vanishYTop = b.yMax - 2;

        // 출발 방향의 '반대편 모서리'에 닿았는지 검사
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int p = piece.cells[i] + piece.position;

            switch (from)
            {
                case DropDir.Up:    // 위에서 내려옴 → 아래 모서리에 닿으면 소멸
                    if (p.y <= vanishYBottom) return true;
                    break;
                case DropDir.Right: // 오른쪽에서 옴 → 왼쪽 모서리에 닿으면 소멸
                    if (p.x <= vanishXLeft) return true;
                    break;
                case DropDir.Down:  // 아래에서 올라옴 → 위 모서리에 닿으면 소멸
                    if (p.y >= vanishYTop) return true;
                    break;
                case DropDir.Left:  // 왼쪽에서 옴 → 오른쪽 모서리에 닿으면 소멸
                    if (p.x >= vanishXRight) return true;
                    break;
            }
        }
        return false;
    }

    // 낙하 반대편 모서리에 닿으면 제거 후 다음 스폰 (장애물 드랍 카운트 포함)
    public bool DestroyIfEdgeSolo(Piece piece)
    {
        if (!IsTouchingVanishEdge(piece)) return false;

        // 어떤 경우든 고정하지 않고 제거
        Clear(piece);
        pieceSpawnFrom.Remove(piece);         // 기록 정리
        NextSpawnIdx();
        SpawnPiece();
        return true;
    }

    // 십자 로켓 폭발 (가로+세로, 중앙 보호) — 점수 60/칸 즉시 가산, 카운트에는 미포함
    private void FireCrossRocketAt(Vector3Int startPos)
    {
        // 가로 라인
        for (int x = Bounds.xMin; x < Bounds.xMax; x++)
        {
            Vector3Int p = new Vector3Int(x, startPos.y, 0);
            if ((Vector2Int)p == new Vector2Int(-1, -1)) // 중앙 보호
                continue;

            TileBase tb = tilemap.GetTile<TileBase>(p);
            if (tb == null) continue;

            PlayDestroyParticle(destroyParticle, p);
            tilemap.SetTile(p, null);

            // 로켓 점수는 여기서 줌 (카운트는 X)
            score += 60;
        }

        // 세로 라인
        for (int y = Bounds.yMin; y < Bounds.yMax; y++)
        {
            Vector3Int p = new Vector3Int(startPos.x, y, 0);
            if ((Vector2Int)p == new Vector2Int(-1, -1)) // 중앙 보호
                continue;

            TileBase tb = tilemap.GetTile<TileBase>(p);
            if (tb == null) continue;

            PlayDestroyParticle(destroyParticle, p);
            tilemap.SetTile(p, null);

            // 로켓 점수는 여기서 줌 (카운트는 X)
            score += 60;
        }
    }

    // 폭탄 폭발
    private void ExplodeBomb(Vector3Int startPos)
    {
        PlayDestroyParticle(bombParticle, startPos);

        // 5x5 범위로 폭발
        for (int x = startPos.x - 2; x <= startPos.x + 2; x++)
        {
            for (int y = startPos.y - 2; y <= startPos.y + 2; y++)
            {
                if (x < Bounds.xMin || x >= Bounds.xMax || y < Bounds.yMin || y >= Bounds.yMax)
                {
                    continue;
                }

                Vector3Int p = new Vector3Int(x, y, 0);
                if ((Vector2Int)p == new Vector2Int(-1, -1)) // 중앙 보호
                    continue;

                TileBase tb = tilemap.GetTile<TileBase>(p);
                if (tb == null) continue;

                tilemap.SetTile(p, null);

                // 폭탄 점수는 여기서 줌 (카운트는 X)
                score += 60;
            }
        }
    }

    // 중앙 보호칸 체크
    private bool IsCenterCell(Vector3Int p)
    {
        return (Vector2Int)p == new Vector2Int(-1, -1);
    }

    // 장애물 1줄 드랍(총 19칸) — 방향에 따라 칸별로 독립 낙하(즉시 배치 버전)
    private void DropObstacleLine(DropDir dir)
    {
        if (obstacleTile == null) return;

        RectInt b = this.Bounds;
        int xMin = b.xMin + 1;
        int xMax = b.xMax - 2; // 포함 범위
        int yMin = b.yMin + 1;
        int yMax = b.yMax - 2; // 포함 범위

        if (dir == DropDir.Up)
        {
            // 아래 → 위
            for (int x = xMin; x <= xMax; x++)
            {
                int? firstOccY = null;
                for (int y = yMin; y <= yMax; y++)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0))) { firstOccY = y; break; }
                }
                if (firstOccY.HasValue)
                {
                    int restY = firstOccY.Value - 1;
                    if (restY >= yMin)
                    {
                        Vector3Int pos = new Vector3Int(x, restY, 0);
                        if (!IsCenterCell(pos) && !tilemap.HasTile(pos))
                            tilemap.SetTile(pos, obstacleTile);
                    }
                }
            }
        }
        else if (dir == DropDir.Down)
        {
            // 위 → 아래
            for (int x = xMin; x <= xMax; x++)
            {
                int? firstOccY = null;
                for (int y = yMax; y >= yMin; y--)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0))) { firstOccY = y; break; }
                }
                if (firstOccY.HasValue)
                {
                    int restY = firstOccY.Value + 1;
                    if (restY <= yMax)
                    {
                        Vector3Int pos = new Vector3Int(x, restY, 0);
                        if (!IsCenterCell(pos) && !tilemap.HasTile(pos))
                            tilemap.SetTile(pos, obstacleTile);
                    }
                }
            }
        }
        else if (dir == DropDir.Right)
        {
            // 좌 → 우
            for (int y = yMin; y <= yMax; y++)
            {
                int? firstOccX = null;
                for (int x = xMin; x <= xMax; x++)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0))) { firstOccX = x; break; }
                }
                if (firstOccX.HasValue)
                {
                    int restX = firstOccX.Value - 1;
                    if (restX >= xMin)
                    {
                        Vector3Int pos = new Vector3Int(restX, y, 0);
                        if (!IsCenterCell(pos) && !tilemap.HasTile(pos))
                            tilemap.SetTile(pos, obstacleTile);
                    }
                }
            }
        }
        else // 우 → 좌
        {
            for (int y = yMin; y <= yMax; y++)
            {
                int? firstOccX = null;
                for (int x = xMax; x >= xMin; x--)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0))) { firstOccX = x; break; }
                }
                if (firstOccX.HasValue)
                {
                    int restX = firstOccX.Value + 1;
                    if (restX <= xMax)
                    {
                        Vector3Int pos = new Vector3Int(restX, y, 0);
                        if (!IsCenterCell(pos) && !tilemap.HasTile(pos))
                            tilemap.SetTile(pos, obstacleTile);
                    }
                }
            }
        }
    }
}
