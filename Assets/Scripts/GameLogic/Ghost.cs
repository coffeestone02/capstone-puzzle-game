using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    private Board board;
    private Piece trackingPiece;
    private Tilemap tilemap;
    private RectInt bounds;

    private Vector3Int[] cells;
    private Vector3Int position;

    private void Start()
    {
        GameObject go = GameObject.Find("MainBoard");
        board = go.GetComponent<Board>();
        trackingPiece = go.GetComponent<Piece>();

        tilemap = GetComponentInChildren<Tilemap>();
        bounds = board.Bounds;

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
        board.Clear(trackingPiece);

        switch (trackingPiece.currentSpawnPos)
        {
            case EPieceDir.UP:
                UpDrop();
                break;
            case EPieceDir.RIGHT:
                RightDrop();
                break;
            case EPieceDir.DOWN:
                DownDrop();
                break;
            case EPieceDir.LEFT:
                LeftDrop();
                break;
        }

        board.Set(trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, trackingPiece.tiles[i]);
        }
    }

    private void UpDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int dest = -bounds.size.y / 2 - 1;

        for (int pos = current; pos >= dest; pos--)
        {
            position.y = pos;

            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void RightDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.x;
        int dest = -bounds.size.x / 2 - 1;

        for (int pos = current; pos >= dest; pos--)
        {
            position.x = pos;

            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void DownDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.y;
        int dest = bounds.size.y / 2;

        for (int pos = current; pos < dest; pos++)
        {
            position.y = pos;

            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }

    private void LeftDrop()
    {
        Vector3Int position = trackingPiece.position;
        int current = position.x;
        int dest = bounds.size.x / 2;

        for (int pos = current; pos < dest; pos++)
        {
            position.x = pos;

            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else
                break;
        }
    }
}
