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
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        data = triominos[randomIdx]; // 만들어질 피스를 랜덤으로 결정

        cells = new Vector3Int[data.cells.Length];
        tiles = new Tile[data.cells.Length];

        for (int i = 0; i < cells.Length; i++) // 셀 위치 등록
            cells[i] = (Vector3Int)data.cells[i];

        ColorSet(data); // 색상 등록

        currentSpawnPos = nextSpawnPos;
        position = Managers.Rule.spawnPositions[nextSpawnPos]; // 스폰 위치 등록

        Board board = GetComponent<Board>();
        SetSpawnPos(); // 다음 스폰 위치로 변경
        board.Set(this); // 보드에 그리기
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

    // TODO - 아이템 추가
}
