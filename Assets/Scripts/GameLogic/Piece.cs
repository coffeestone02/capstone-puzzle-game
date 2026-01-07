using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

// 피스를 생성
public class Piece : MonoBehaviour
{
    public TriominoData[] triominos; // 게임에서 쓸 수 있는 트리오미노들(인스펙터에서 설정)
    public TriominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; } // 셀들의 위치 정보
    public Tile[] tiles { get; private set; } // 만들어진 타일(런타임 시간에 정해짐)
    public Vector3Int position { get; private set; } // 피스 위치 
    private EPieceDir spawnPos = EPieceDir.LEFT; // 스폰 위치

    private void Start()
    {
        foreach (TriominoData triomino in triominos)
            triomino.Init();
    }

    public void SpawnPiece()
    {
        int randomIdx = UnityEngine.Random.Range(0, triominos.Length);
        TriominoData data = triominos[randomIdx]; // 만들어질 피스를 랜덤으로 결정

        cells = new Vector3Int[data.cells.Length];
        tiles = new Tile[cells.Length];

        for (int i = 0; i < cells.Length; i++) // 셀 위치 등록
            cells[i] = (Vector3Int)data.cells[i];

        ColorSet(data); // 색상 등록

        position = Managers.Rule.spawnPositions[spawnPos]; // 최종 위치 등록

        Board board = GetComponent<Board>();
        board.Set(this);
    }

    // 색상 결정
    private void ColorSet(TriominoData data)
    {
        // 랜덤으로 한 색깔을 골라 해당 색으로 전부 만든다
        int randomIdx = Random.Range(0, data.normalTiles.Length);
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

        randomIdx = Random.Range(0, data.normalTiles.Length);
        tiles[randomIdx] = replaceTile;
    }

}
