using UnityEngine;

/// <summary>
/// 피스의 회전
/// </summary>
public class PieceRotator : MonoBehaviour
{
    private Piece activePiece;
    private int rotationIndex;

    private void Awake()
    {
        activePiece = GetComponent<Piece>();
    }

    private void OnEnable()
    {
        Managers.Input.rotateAction += OnRotate;
    }

    private void OnDisable()
    {
        Managers.Input.rotateAction -= OnRotate;
    }

    /// <summary>
    /// InputManager.rotateAction에 바인딩하여 사용
    /// </summary>
    /// <param name="direction">-1은 왼쪽회전, 1은 오른쪽회전</param>
    private void OnRotate(int direction)
    {
        Board board = GetComponent<Board>();

        board.Clear(activePiece);
        Rotate(direction);
        board.Set(activePiece);
    }

    /// <summary>
    /// 조작중인 피스를 회전시킴
    /// </summary>
    /// <param name="piece">조작중인 피스</param>
    /// <param name="direction">1은 오른쪽, -1은 왼쪽 회전</param>
    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        ApplyRotation(direction);

        if (TestWallKicks(rotationIndex, direction) == false)
        {
            rotationIndex = originalRotation;
            ApplyRotation(-direction);
        }
    }

    public void ApplyRotation(int direction)
    {
        for (int i = 0; i < activePiece.cells.Length; i++)
        {
            int x, y;
            if (direction > 0)
            {
                x = activePiece.cells[i].y;
                y = -activePiece.cells[i].x;
            }
            else
            {
                x = -activePiece.cells[i].y;
                y = activePiece.cells[i].x;
            }

            activePiece.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        Board board = GetComponent<Board>();
        TriominoData data = activePiece.data;
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = activePiece.data.wallKicks[wallKickIndex, i];
            Vector3Int newPosition = activePiece.position;
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            if (board.IsValidPosition(activePiece, newPosition))
            {
                activePiece.position = newPosition;
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        TriominoData data = activePiece.data;
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
            wallKickIndex--;

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
}
