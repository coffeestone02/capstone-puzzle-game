using UnityEngine;

// 피스의 움직임
public class PieceMover : MonoBehaviour
{
    private Board board;
    private Piece activePiece;

    public float stepDelay = 1.25f; // stepDelay의 시간만큼 중심으로 이동함
    public float moveDelay = 0.1f; // 플레이어의 입력 이동 속도를 정함. 값이 클수록 입력을 많이 못함
    public float lockDelay = 0.5f; // 이 시간만큼 못 움직이면 피스를 Lock함

    private float stepTime;
    private float moveTime;
    private float lockTime;

    private void Start()
    {
        board = GetComponent<Board>();
        activePiece = GetComponent<Piece>();
        board.Clear(activePiece);
        Move(Vector2Int.down);
        Move(Vector2Int.down);
        Move(Vector2Int.down);
        board.Set(activePiece);
    }

    private void Update()
    {
        // lockTime += Time.deltaTime;
        // stepTime = Time.time + stepDelay;
        // moveTime = Time.time + moveDelay;
    }

    private void Step(Piece piece)
    {

    }

    private bool Move(Vector2Int moveDir)
    {
        Vector3Int newPosition = activePiece.position;
        newPosition.x += moveDir.x;
        newPosition.y += moveDir.y;

        bool valid = board.IsValidPosition(activePiece, newPosition);

        if (valid)
        {
            activePiece.position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f;
        }

        return valid;
    }
}
