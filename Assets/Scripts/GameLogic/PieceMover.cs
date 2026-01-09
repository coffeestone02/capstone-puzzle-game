using System.Numerics;
using UnityEngine;

/// <summary>
/// 피스의 움직임
/// </summary>
public class PieceMover : MonoBehaviour
{
    private Board board;
    private Piece activePiece;

    private float stepTime;
    private float moveTime;
    private float lockTime;
    private EPieceDir stepDir = EPieceDir.DOWN;

    private void Awake()
    {
        board = GetComponent<Board>();
        activePiece = GetComponent<Piece>();
    }

    private void Update()
    {
        board.Clear(activePiece);

        lockTime += Time.deltaTime;

        HandleRotate();
        if (Time.time > moveTime)
        {
            // 움직임 입력
            HandleMovement();
        }

        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(activePiece);
    }

    private void HandleRotate()
    {

    }

    private void HandleMovement()
    {
        Move(activePiece, Managers.Input.moveDir);
    }

    private void Lock()
    {
        board.Set(activePiece);
        activePiece.SpawnPiece();
        SetStepDirection();
    }

    /// <summary>
    /// 현재 스폰 위치에 따른 스텝 방향 결정
    /// </summary>
    private void SetStepDirection()
    {
        switch (activePiece.currentSpawnPos)
        {
            case EPieceDir.UP:
                stepDir = EPieceDir.DOWN;
                break;
            case EPieceDir.RIGHT:
                stepDir = EPieceDir.LEFT;
                break;
            case EPieceDir.DOWN:
                stepDir = EPieceDir.UP;
                break;
            case EPieceDir.LEFT:
                stepDir = EPieceDir.RIGHT;
                break;
        }
    }

    /// <summary>
    /// EpieceDir의 열거형을 Vector2Int 형태로 변환
    /// </summary>
    private Vector2Int GetMoveVector2(EPieceDir moveDir)
    {
        Vector2Int moveVec = Vector2Int.up;
        switch (moveDir)
        {
            case EPieceDir.UP:
                moveVec = Vector2Int.up;
                break;
            case EPieceDir.RIGHT:
                moveVec = Vector2Int.right;
                break;
            case EPieceDir.DOWN:
                moveVec = Vector2Int.down;
                break;
            case EPieceDir.LEFT:
                moveVec = Vector2Int.left;
                break;
        }

        return moveVec;
    }

    private void Step()
    {
        stepTime = Time.time + Managers.Rule.stepDelay;

        switch (stepDir)
        {
            case EPieceDir.UP:
                Move(activePiece, EPieceDir.UP);
                break;
            case EPieceDir.RIGHT:
                Move(activePiece, EPieceDir.RIGHT);
                break;
            case EPieceDir.DOWN:
                Move(activePiece, EPieceDir.DOWN);
                break;
            case EPieceDir.LEFT:
                Move(activePiece, EPieceDir.LEFT);
                break;
        }

        if (lockTime >= Managers.Rule.lockDelay)
            Lock();
    }

    public bool Move(Piece piece, EPieceDir moveDir)
    {
        if (moveDir == EPieceDir.NONE)
            return false;

        Vector2Int moveVec = GetMoveVector2(moveDir);
        Vector3Int newPosition = piece.position;
        newPosition.x += moveVec.x;
        newPosition.y += moveVec.y;

        bool valid = board.IsValidPosition(piece, newPosition);
        if (valid)
        {
            piece.position = newPosition;
            moveTime = Time.time + Managers.Rule.moveDelay;
            lockTime = 0f;
        }

        return valid;
    }
}
