using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Board mainBoard;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; } // 셀들의 위치 정보
    public Vector3Int position { get; private set; } // 피스의 위치 정보

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[3];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        mainBoard.Clear(trackingPiece);

        switch (mainBoard.currentSpawnIdx)
        {
            case 0:
                UpDrop();
                break;
            case 1:
                RightDrop();
                break;
            case 2:
                DownDrop();
                break;
            case 3:
                LeftDrop();
                break;
        }

        mainBoard.Set(trackingPiece);
    }

    private void UpDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int dest = -mainBoard.boardSize.y / 2 - 1;

        for (int pos = current; pos >= dest; pos--)
        {
            position.y = pos;

            if (Util.IsValidPosition(mainBoard, trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void RightDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.x;
        int dest = -mainBoard.boardSize.x / 2 - 1;

        for (int pos = current; pos >= dest; pos--)
        {
            position.x = pos;

            if (Util.IsValidPosition(mainBoard, trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void DownDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int dest = mainBoard.boardSize.y / 2;

        for (int pos = current; pos < dest; pos++)
        {
            position.y = pos;

            if (Util.IsValidPosition(mainBoard, trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void LeftDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.x;
        int dest = mainBoard.boardSize.x / 2;

        for (int pos = current; pos < dest; pos++)
        {
            position.x = pos;

            if (Util.IsValidPosition(mainBoard, trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, trackingPiece.tiles[i]);
        }
    }
}
