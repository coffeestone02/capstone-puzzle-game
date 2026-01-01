using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Board board;
    [SerializeField] private Piece piece;

    [Header("Tile Registry (index 1..N), 0 = empty")]
    [SerializeField] private TileBase[] saveTiles;

    private Dictionary<TileBase, int> tileToId;

    // 저장이 있으면 시작 시 로드 실행
    private bool shouldLoadOnStart;

    private void Awake()
    {
        if (board == null) board = GameObject.Find("Board").GetComponent<Board>();
        if (piece == null) piece = GameObject.Find("Board").GetComponent<Piece>();

        BuildTileMap();

        shouldLoadOnStart = SaveSystem.HasSave();

        if (shouldLoadOnStart && board != null)
        {
            // Board.Start에서 SpawnPiece()가 돌지 않도록 미리 막음
            board.wasLoadedBySaveController = true;
        }

        if (shouldLoadOnStart && piece != null)
        {
            // 로드 중 piece.Update가 보드를 건드리지 않게 차단
            piece.enabled = false;
        }
    }

    private void Start()
    {
        if (!shouldLoadOnStart)
            return;

        if (SaveSystem.TryLoad(out var data))
        {
            LoadFrom(data);
        }
        else
        {
            // 저장이 없으면 정상 시작
            if (piece != null) piece.enabled = true;
            if (board != null) board.wasLoadedBySaveController = false;
            if (board != null) board.SpawnPiece();
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

    public void SaveNow()
    {
        if (board == null || piece == null) return;

        // 게임오버면 이어하기 막기
        if (GameManager.Instance != null && GameManager.Instance.isOver)
        {
            SaveSystem.Clear();
            return;
        }

        // 저장 직전: 현재 피스를 타일맵에 확실히 찍어둠(프레임 타이밍 방지) -> 생성 직후 바로 나가면 게임 오버
        board.Set(piece);

        SaveData data = new SaveData();

        // 진행 상태
        data.score = board.score;
        data.level = board.level;
        data.obstacleLimit = board.obstacleLimit;
        data.obstacleCounter = board.obstacleCounter;
        data.brokenBlockCount = board.brokenBlockCount;
        data.nextSpawnHasBomb = board.nextSpawnHasBomb;
        data.nextSpawnHasRocket = board.nextSpawnHasRocket;
        data.currentSpawnIdx = board.currentSpawnIdx;

        // 고정 타일(현재 피스 제외) 저장
        TileBase[] fixedTilesTB = board.ExportFixedTilesWithoutActivePiece_TileBase();
        data.fixedTiles = ConvertTileBaseArrayToIds(fixedTilesTB);

        // 현재 피스 저장
        data.triominoIndex = board.activeTriominoIndex;
        data.posX = piece.position.x;
        data.posY = piece.position.y;
        data.rotationIdx = piece.rotationIdx;
        data.stepRemain = piece.GetStepTimeRemaining();
        data.lockTime = piece.GetLockTime();

        data.pieceTileIds = new int[3];
        for (int i = 0; i < 3; i++)
        {
            TileBase tb = piece.tiles[i];
            data.pieceTileIds[i] = (tb != null && tileToId.TryGetValue(tb, out int id)) ? id : 0;
        }

        SaveSystem.Save(data);
    }

    // 불러오기
    public void LoadFrom(SaveData data)
    {
        if (board == null || piece == null) return;

        // 로드 중 Piece.Update 차단
        piece.enabled = false;

        // (해결책 1) 로드시 먼저 보드 리셋(중심 유지)
        board.ResetBoardKeepCenter();

        // 고정 블록 복원
        TileBase[] fixedTB = ConvertIdsToTileBaseArray(data.fixedTiles);
        board.ImportFixedTiles_TileBase(fixedTB);

        // 진행 상태 복원
        board.score = data.score;
        board.level = data.level;
        board.obstacleLimit = data.obstacleLimit;
        board.obstacleCounter = data.obstacleCounter;
        board.brokenBlockCount = data.brokenBlockCount;
        board.nextSpawnHasBomb = data.nextSpawnHasBomb;
        board.nextSpawnHasRocket = data.nextSpawnHasRocket;
        board.currentSpawnIdx = data.currentSpawnIdx;
        board.activeTriominoIndex = data.triominoIndex;

        // 현재 피스 복원
        Tile[] restoredPieceTiles = new Tile[3];
        for (int i = 0; i < 3; i++)
        {
            int id = (data.pieceTileIds != null && i < data.pieceTileIds.Length) ? data.pieceTileIds[i] : 0;
            restoredPieceTiles[i] = (id > 0 && saveTiles != null && id < saveTiles.Length) ? (Tile)saveTiles[id] : null;
        }

        if (data.triominoIndex < 0 || data.triominoIndex >= board.triominos.Length)
        {
            // 저장값 이상하면 그냥 새로 시작
            piece.enabled = true;
            board.wasLoadedBySaveController = false;
            board.SpawnPiece();
            return;
        }

        TriominoData tri = board.triominos[data.triominoIndex];

        piece.ApplySavedState(
            board,
            new Vector3Int(data.posX, data.posY, 0),
            tri,
            data.rotationIdx,
            restoredPieceTiles,
            data.stepRemain,
            data.lockTime
        );

        // 타일맵에 현재 피스 다시 그리기
        board.Set(piece);

        // 로드 끝: Piece 업데이트 재개
        piece.enabled = true;

        // Board.Start에서 SpawnPiece 막기 유지(이미 true였을 확률 높음)
        board.wasLoadedBySaveController = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBgm(board.level);
    }

    // 변환 유틸
    private int[] ConvertTileBaseArrayToIds(TileBase[] tiles)
    {
        if (tiles == null) return null;

        int[] ids = new int[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            TileBase t = tiles[i];
            ids[i] = (t != null && tileToId.TryGetValue(t, out int id)) ? id : 0;
        }
        return ids;
    }

    private TileBase[] ConvertIdsToTileBaseArray(int[] ids)
    {
        if (ids == null) return null;

        TileBase[] tiles = new TileBase[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            int id = ids[i];
            tiles[i] = (id > 0 && saveTiles != null && id < saveTiles.Length) ? saveTiles[id] : null;
        }
        return tiles;
    }


}

