using System.Numerics;
using UnityEngine;
using System.IO;
using System;

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

    private void Start()
    {
        board = GetComponent<Board>();
        activePiece = GetComponent<Piece>();

        Managers.Input.moveAction -= OnMove;
        Managers.Input.moveAction += OnMove;
    }

    private void Update()
    {
        lockTime += Time.deltaTime;

        if (Time.time > stepTime) Step();
    }

    /// <summary>
    /// InputManager.moveAction에 바인딩하여 사용
    /// </summary>
    /// <param name="moveDir"></param>
    private void OnMove(EPieceDir moveDir)
    {
        if (Time.time > moveTime)
        {
            Move(activePiece, Util.GetMoveVector2Int(moveDir));
        }
    }

    private void Lock()
    {
        Debug.Log("LOCK");
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

    private void Step()
    {
        stepTime = Time.time + Managers.Rule.stepDelay;

        Move(activePiece, Util.GetMoveVector2Int(stepDir));

        if (lockTime >= Managers.Rule.lockDelay)
            Lock();
    }

    public bool Move(Piece piece, Vector2Int translate)
    {
        // 아무런 입력도 받지 않을 때 or 반대 방향 입력
        if (translate == Vector2Int.zero || IsOppositeDir(translate))
            return false;

        board.Clear(piece);
        Vector3Int newPosition = piece.position;
        newPosition.x += translate.x;
        newPosition.y += translate.y;

        bool valid = board.IsValidPosition(piece, newPosition);
        if (valid)
        {
            piece.position = newPosition;
            moveTime = Time.time + Managers.Rule.moveDelay;
            lockTime = 0f;
        }

        board.Set(piece);
        return valid;
    }

    // 움직이려는 방향이 스텝 방향과 반대 방향이면 true
    private bool IsOppositeDir(Vector2Int moveInput)
    {
        if (stepDir == EPieceDir.UP && moveInput == Vector2Int.down)
            return true;
        if (stepDir == EPieceDir.DOWN && moveInput == Vector2Int.up)
            return true;
        if (stepDir == EPieceDir.RIGHT && moveInput == Vector2Int.left)
            return true;
        if (stepDir == EPieceDir.LEFT && moveInput == Vector2Int.right)
            return true;

        return false;
    }
}
