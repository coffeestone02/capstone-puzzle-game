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
    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)
    public Vector3Int[] spawnPositions; // 피스가 스폰될 위치(인스펙터에서 설정)
    private Vector2Int boardSize; // 게임보드 사이즈
    public GameManager gameManager; // UI 연결용
    public Tile grayTile; // 가장자리에 닿으면 변하는 타일
    public GameObject destroyParticles;
    public float playTime { get; private set; }

    public bool isMatching { get; private set; }
    [SerializeField] private int width;
    [SerializeField] private int height;
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


    public TMP_Text scoreText;
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
    }

    // 지정된 위치에 트리오미노를 랜덤으로 골라 스폰
    public void SpawnPiece()
    {
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];

        activePiece.Initialize(this, spawnPositions[currentSpawnIdx], data);

        if (!IsValidPosition(activePiece, activePiece.position)) //블록 생성 후 겹칠 시 게임 오버
        {
            gameManager.GameOver();
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
        int mainPoint = 0;
        int bonusPoint = 0;

        HashSet<Vector3Int> matched = FindMainMatch(piece); // 메인 피스 매칭
        mainPoint += matched.Count * 100; // 메인 피스 점수 계산
        DeleteMatchedPiece(matched); // 메인 피스 제거

        if (matched.Count == 0)
        {
            combo = 0;
        }
        else
        {
            piece.PlaySound(piece.soundClear);
        }

        HashSet<Vector3Int> bonusMatched = FindBonusMatch(matched); // 추가 제거 매칭
        bonusPoint = bonusMatched.Count * 60; // 추가 제거 점수 계산
        DeleteMatchedPiece(bonusMatched); // 추가 피스 제거

        score += (mainPoint + bonusPoint) * (1 + combo) * (int)(1 + 0.1 * level); // 최종 점수 계산
        scoreText.text = score.ToString();

        isMatching = false;
    }

    // 매치된 피스를 제거
    private void DeleteMatchedPiece(HashSet<Vector3Int> matched)
    {
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
    
        // 파티클 효과를 생성하고 재생하는 함수
    private void PlayDestroyParticles(Vector3 position)
    {
        if (destroyParticles == null)
        {
            return;
        }

        GameObject particles = Instantiate(destroyParticles, tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        float scaleMultiplier = 0.2f; // 파티클 크기를 줄임
        particles.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        Destroy(particles, 1f); // 1초 뒤에 파괴
    }
}
