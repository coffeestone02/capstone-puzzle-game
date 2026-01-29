using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.IO;
using System;

/// <summary>
/// 피스를 생성하고 위치값을 관리함
/// </summary>
public class Piece : MonoBehaviour
{
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노들(인스펙터에서 설정)
    public TriominoData data { get; private set; } // 생성에 사용될 트리오미노 데이터
    public Vector3Int[] cells { get; set; } // 셀들의 위치 정보
    public Tile[] tiles { get; private set; } // 만들어진 타일
    public Vector3Int position { get; set; } // 피스의 기준 위치 
    public EPieceDir currentSpawnPos { get; private set; } = EPieceDir.UP; // 현재 스폰 위치
    public EPieceDir nextSpawnPos { get; set; } = EPieceDir.UP; // 다음 스폰 위치

    private void Awake()
    {
        foreach (TriominoData triomino in triominos)
            triomino.Init();
    }

    /// <summary>
    /// 피스 생성
    /// </summary>
    public void SpawnPiece()
    {
        // 1. 만들어질 피스를 랜덤으로 결정
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        data = triominos[randomIdx];
        cells = new Vector3Int[data.cells.Length];
        tiles = new Tile[data.cells.Length];

        // 2. 셀 위치 등록
        for (int i = 0; i < cells.Length; i++)
            cells[i] = (Vector3Int)data.cells[i];

        // 3. 색상 등록
        ColorSet(data);

        // 4. 스폰 위치 등록
        currentSpawnPos = nextSpawnPos;
        position = Managers.Rule.spawnPositions[nextSpawnPos];
        SetSpawnPos();

        // 5. 방향과 위치를 초기 설정에 맞게 세팅해줌
        PieceRotator pr = GetComponent<PieceRotator>();
        PieceMover pm = GetComponent<PieceMover>();
        switch (currentSpawnPos)
        {
            case EPieceDir.UP:
                break;
            case EPieceDir.RIGHT:
                pr.ApplyRotation(1);
                break;
            case EPieceDir.DOWN:
                pm.CanMove(Util.GetMoveVector2Int(EPieceDir.LEFT));
                pr.ApplyRotation(1);
                pr.ApplyRotation(1);
                break;
            case EPieceDir.LEFT:
                pr.ApplyRotation(1);
                pr.ApplyRotation(1);
                pr.ApplyRotation(1);
                break;
        }

        // 6. 아이템 추가
        if (Managers.Rule.nextSpawnHasBomb)
            AddBombTile();
        else if (Managers.Rule.nextSpawnHasRocket)
            AddRocketTile();

        Board board = GetComponent<Board>();
        if (board.IsValidPosition(this, position) == false) // 7. 그려진 위치가 스폰 위치면 게임 오버
        {
            Managers.Rule.isOver = true;
            Debug.Log("게임종료");
        }
        else // 8. 보드에 그리기
        {
            board.Set(this);
        }
    }

    // 다음 스폰 위치로 변경
    private void SetSpawnPos()
    {
        switch (currentSpawnPos)
        {
            case EPieceDir.UP:
                nextSpawnPos = EPieceDir.RIGHT;
                break;
            case EPieceDir.RIGHT:
                nextSpawnPos = EPieceDir.DOWN;
                break;
            case EPieceDir.DOWN:
                nextSpawnPos = EPieceDir.LEFT;
                break;
            case EPieceDir.LEFT:
                nextSpawnPos = EPieceDir.UP;
                break;
        }
    }

    // 색상 결정
    private void ColorSet(TriominoData data)
    {
        // 랜덤으로 한 색깔을 골라 해당 색으로 전부 만든다
        int randomIdx = UnityEngine.Random.Range(0, data.normalTiles.Length);
        for (int i = 0; i < cells.Length; i++)
        {
            tiles[i] = data.normalTiles[randomIdx];
        }

        // 남은 색상 타일을 한 곳에 끼워넣는다
        Tile replaceTile = data.normalTiles[0];
        switch (randomIdx)
        {
            case 0:
                replaceTile = data.normalTiles[1];
                break;
            case 1:
                replaceTile = data.normalTiles[0];
                break;
            default:
                break;
        }

        randomIdx = UnityEngine.Random.Range(0, data.normalTiles.Length);
        tiles[randomIdx] = replaceTile;
    }

    private void AddBombTile()
    {
        int cellIdx = UnityEngine.Random.Range(0, cells.Length);
        Tile bombTile = GetBombTile(tiles[cellIdx]);
        tiles[cellIdx] = bombTile;
        Managers.Rule.nextSpawnHasBomb = false; // 개선 필요
    }

    private void AddRocketTile()
    {
        int cellIdx = UnityEngine.Random.Range(0, cells.Length);
        Tile rocketTile = GetRocketTile(tiles[cellIdx]);
        tiles[cellIdx] = rocketTile;
        Managers.Rule.nextSpawnHasRocket = false; // 개선 필요
    }

    private Tile GetBombTile(Tile tile)
    {
        Tile bombTile = null;
        string tileName = tile.name.ToLowerInvariant();

        if (tileName.Contains("blue"))
            bombTile = Resources.Load<Tile>("VisualAssets/Tiles/BlueBombTile");
        else if (tileName.Contains("red"))
            bombTile = Resources.Load<Tile>("VisualAssets/Tiles/RedBombTile");

        return bombTile;
    }

    // 타일 색에 맞는 로켓 타일 반환
    public Tile GetRocketTile(Tile tile)
    {
        Tile rocketTile = null;
        string tileName = tile.name.ToLowerInvariant();

        if (tileName.Contains("blue"))
            rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/BlueRocketTile");
        else if (tileName.Contains("red"))
            rocketTile = Resources.Load<Tile>("VisualAssets/Tiles/RedRocketTile");

        return rocketTile;
    }
}
