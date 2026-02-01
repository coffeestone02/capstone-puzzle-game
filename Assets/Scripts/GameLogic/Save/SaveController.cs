using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveController : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Piece piece;

    [SerializeField] private TileBase[] saveTiles;

    private Dictionary<TileBase, int> tileToId;
    private bool shouldLoadOnStart;

    private void Awake()
    {
        if (board == null) board = FindObjectOfType<Board>();
        if (piece == null) piece = FindObjectOfType<Piece>();

        BuildTileMap();

        shouldLoadOnStart = SaveSystem.HasSave();

        if (shouldLoadOnStart && board != null)
            board.skipSpawnOnStart = true; // Board.Start에서 자동 Spawn 막기

        if (shouldLoadOnStart && piece != null)
            piece.enabled = false; // 로드 중 입력/갱신 차단
    }

    private void Start()
    {
        if (!shouldLoadOnStart) return;

        if (SaveSystem.TryLoad(out var data))
        {
            LoadFrom(data);
        }
        else
        {
            // 저장이 깨졌으면 정상 시작
            if (board != null) board.skipSpawnOnStart = false;
            if (piece != null)
            {
                piece.enabled = true;
                piece.SpawnPiece();
            }
        }
    }

    private void BuildTileMap()
    {
        tileToId = new Dictionary<TileBase, int>();
        if (saveTiles == null) return;

        for (int i = 1; i < saveTiles.Length; i++)
        {
            var t = saveTiles[i];
            if (t != null && !tileToId.ContainsKey(t))
                tileToId.Add(t, i);
        }
    }

    // 저장
    public void SaveNow()
    {
        if (board == null || board.tilemap == null) return;

        // 현재 피스를 보드에 찍어둠
        if (piece != null)
            board.Set(piece);

        RectInt b = board.Bounds;
        var d = new SaveData();

        d.xMin = b.xMin;
        d.yMin = b.yMin;
        d.width = b.width;
        d.height = b.height;

        d.boardTileIds = ExportBoard(board.tilemap, b);

        // Managers.Rule 상태 저장
        d.blockCounter = Managers.Rule.BlockCounter;
        d.nextSpawnHasBomb = Managers.Rule.nextSpawnHasBomb;
        d.nextSpawnHasRocket = Managers.Rule.nextSpawnHasRocket;
        d.obstacleCount = Managers.Rule.obstacleCount;
        d.obstacleSpawnLimit = Managers.Rule.obstacleSpawnLimit;

        // Managers.Score 
        d.playtime = Managers.Score.playtime;
        d.score = Managers.Score.score;
        d.combo = Managers.Score.combo;

        // Piece 저장
        if (piece != null && piece.tiles != null && piece.cells != null)
        {
            d.hasPiece = true;
            d.triominoIndex = FindTriominoIndex(piece);

            d.posX = piece.position.x;
            d.posY = piece.position.y;

            d.currentSpawnPos = (int)piece.currentSpawnPos;
            d.nextSpawnPos = (int)piece.nextSpawnPos;

            d.cellX = new int[piece.cells.Length];
            d.cellY = new int[piece.cells.Length];
            for (int i = 0; i < piece.cells.Length; i++)
            {
                d.cellX[i] = piece.cells[i].x;
                d.cellY[i] = piece.cells[i].y;
            }

            d.pieceTileIds = new int[piece.tiles.Length];
            for (int i = 0; i < piece.tiles.Length; i++)
            {
                TileBase tb = piece.tiles[i];
                d.pieceTileIds[i] = (tb != null && tileToId.TryGetValue(tb, out int id)) ? id : 0;
            }
        }
        else
        {
            d.hasPiece = false;
        }

        SaveSystem.Save(d);
    }

    private int FindTriominoIndex(Piece p)
    {
        var target = p.data;
        if (target == null) return 0;

        for (int i = 0; i < p.triominos.Length; i++)
            if (p.triominos[i] == target) return i;

        return 0;
    }

    private int[] ExportBoard(Tilemap tm, RectInt b)
    {
        int[] ids = new int[b.width * b.height];

        for (int y = b.yMin; y < b.yMin + b.height; y++)
        {
            for (int x = b.xMin; x < b.xMin + b.width; x++)
            {
                int idx = (x - b.xMin) + (y - b.yMin) * b.width;
                TileBase tb = tm.GetTile(new Vector3Int(x, y, 0));
                ids[idx] = (tb != null && tileToId.TryGetValue(tb, out int id)) ? id : 0;
            }
        }
        return ids;
    }

    // 블러오기
    public void LoadFrom(SaveData d)
    {
        if (board == null || board.tilemap == null) return;

        if (piece != null)
            piece.enabled = false;

        // 보드 복원
        board.tilemap.ClearAllTiles();
        ImportBoard(board.tilemap, d);

        // Managers.Rule 복원
        Managers.Rule.BlockCounter = d.blockCounter;
        Managers.Rule.nextSpawnHasBomb = d.nextSpawnHasBomb;
        Managers.Rule.nextSpawnHasRocket = d.nextSpawnHasRocket;
        Managers.Rule.obstacleCount = d.obstacleCount;
        Managers.Rule.obstacleSpawnLimit = d.obstacleSpawnLimit;

        // 점수 복원
        Managers.Score.ApplyLoadedState(d.playtime, d.score, d.combo);

        // Piece 복원
        if (piece != null && d.hasPiece)
        {
            Vector3Int savedPos = new Vector3Int(d.posX, d.posY, 0);

            var savedCells = new Vector3Int[3];
            for (int i = 0; i < 3; i++)
                savedCells[i] = new Vector3Int(d.cellX[i], d.cellY[i], 0);

            var savedTiles = new Tile[3];
            for (int i = 0; i < 3; i++)
            {
                int id = d.pieceTileIds[i];
                savedTiles[i] = (id > 0 && saveTiles != null && id < saveTiles.Length) ? (Tile)saveTiles[id] : null;
            }

            piece.ApplySavedState(

                d.triominoIndex,
                savedPos,
                (EPieceDir)d.currentSpawnPos,
                (EPieceDir)d.nextSpawnPos,
                savedCells,
                savedTiles
            );

            var mover = piece.GetComponent<PieceMover>();
            if (mover != null) mover.SetStepDirection();

            board.Set(piece);
            piece.enabled = true;
        }
        else
        {
            // 피스 정보가 없으면 새로 시작
            if (board != null) board.skipSpawnOnStart = false;
            if (piece != null)
            {
                piece.enabled = true;
                piece.SpawnPiece();
            }
        }
    }

    private void ImportBoard(Tilemap tm, SaveData d)
    {
        if (d.boardTileIds == null) return;

        int xMin = d.xMin;
        int yMin = d.yMin;
        int w = d.width;
        int h = d.height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = x + y * w;
                int id = d.boardTileIds[idx];

                TileBase tb = (id > 0 && saveTiles != null && id < saveTiles.Length) ? saveTiles[id] : null;
                tm.SetTile(new Vector3Int(xMin + x, yMin + y, 0), tb);
            }
        }

    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveNow();
    }

    private void OnApplicationQuit()
    {
        SaveNow();
    }

}
