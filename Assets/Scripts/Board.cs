using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 블록을 관리하는 보드겸 게임 매니저
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; } // 그려질 타일맵
    public Piece activePiece { get; private set; } // 현재 조작중인 피스
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노(인스펙터에서 설정)
    public Vector3Int[] spawnPositions; // 피스가 스폰될 위치
    public Vector2Int boardSize = new Vector2Int(19, 19); // 게임보드 사이즈

    public RectInt Bounds // 보드 범위를 확인하는데 사용함.
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2 - 1, -this.boardSize.y / 2 - 1);
            return new RectInt(position, this.boardSize);
        }
    }

    public int currentSpawnIdx = 0; // 스폰 위치 인덱스. 0 : 위쪽, 1 : 오른쪽, 2 : 아래쪽, 3: 왼쪽

    private void Awake() 
    {
        tilemap = GetComponentInChildren<Tilemap>();   
        activePiece = GetComponentInChildren<Piece>();

        // 인스펙터에서 만들어진 테트로미노의 수만큼 진행
        for (int i = 0; i < triominos.Length; i++)
        {
            triominos[i].Initialize();
        }
    }

    void Start()
    {
        SpawnPiece();
    }

    void Update()
    {
        
    }

    // 지정된 위치에 트리오미노를 랜덤으로 골라 스폰
    public void SpawnPiece()
    {
        int randomIdx = Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx];
        activePiece.Initialize(this, spawnPositions[currentSpawnIdx], data);
        Set(activePiece);
    }

    // Piece를 만들어서 타일에 그림
    public void Set(Piece piece)
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
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
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    // 스폰 위치를 계산
    private void ComputeSpawnIdx()
    {
        currentSpawnIdx++;
        currentSpawnIdx = currentSpawnIdx % 4;
    }
}
