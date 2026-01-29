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
        if (Managers.Rule.isPause || Managers.Rule.isOver)
            return;

        lockTime += Time.deltaTime;

        if (Time.time > stepTime)
            Step();
    }

    /// <summary>
    /// InputManager.moveAction에 바인딩하여 사용
    /// </summary>
    /// <param name="moveDir">이동 방향</param>
    private void OnMove(EPieceDir moveDir)
    {
        if (Time.time > moveTime)
        {
            if (moveDir == stepDir)
                stepTime = Time.time + Managers.Rule.stepDelay;

            Move(Util.GetMoveVector2Int(moveDir));
        }
    }

    private void Step()
    {
        stepTime = Time.time + Managers.Rule.stepDelay;

        Move(Util.GetMoveVector2Int(stepDir));

        if (lockTime >= Managers.Rule.lockDelay)
            board.Lock(activePiece);
    }

    public bool CanMove(Vector2Int translate)
    {
        Vector3Int newPosition = activePiece.position;
        newPosition.x += translate.x;
        newPosition.y += translate.y;

        bool valid = board.IsValidPosition(activePiece, newPosition);
        if (valid)
        {
            activePiece.position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }

    public bool Move(Vector2Int translate)
    {
        // 아무런 입력도 받지 않을 때 or 반대 방향 입력
        if (translate == Vector2Int.zero || IsOppositeDir(translate))
            return false;

        board.Clear(activePiece);

        bool valid = CanMove(translate);
        if (valid)
            moveTime = Time.time + Managers.Rule.moveDelay;

        board.Set(activePiece);

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


    /// <summary>
    /// 현재 스폰 위치에 따른 스텝 방향 결정
    /// </summary>
    public void SetStepDirection()
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

}
