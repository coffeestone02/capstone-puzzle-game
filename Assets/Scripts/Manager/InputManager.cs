using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    // TEMP
    public Action<EPieceDir> moveAction = null;
    public Action<int> rotateAction = null;

    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private EPieceDir keepMoveDir;
    private float swipeRange = 50f;
    private bool isHolding = false;

    public void OnUpdate()
    {
        TouchInput();

        // TEMP
        RotateInput();
        MoveInput();
    }

    private void KeepMove()
    {
        if (isHolding == false || moveAction == null) return;

        moveAction.Invoke(keepMoveDir);
    }

    private void TouchInput()
    {
        // 터치가 없거나 UI를 터치한 경우 동작안함
        if (Input.touchCount <= 0 ||
            EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        Touch touch = Input.GetTouch(0); // 터치 정보 가져옴
        KeepMove();
        switch (touch.phase)
        {
            case TouchPhase.Began:
                TouchStart(touch.position);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                TouchHold(touch.position);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                TouchEnd();
                break;
        }
    }

    private void TouchEnd()
    {
        Vector2 swipeDistance = touchCurrentPos - touchStartPos;
        if (isHolding == false && swipeDistance.magnitude < swipeRange && rotateAction != null)
            rotateAction.Invoke(1);

        isHolding = false;
    }

    private void TouchHold(Vector2 touchPos)
    {
        if (isHolding || moveAction == null) return;

        // 스와이프 길이
        touchCurrentPos = touchPos;
        Vector2 swipeDistance = touchCurrentPos - touchStartPos;

        // 스와이프 방향
        float absX = Mathf.Abs(swipeDistance.x);
        float absY = MathF.Abs(swipeDistance.y);
        if (absX > swipeRange || absY > swipeRange)
        {
            isHolding = true;
            if (absX > absY && swipeDistance.x > 0) // 오른쪽
                keepMoveDir = EPieceDir.RIGHT;
            else if (absX > absY && swipeDistance.x <= 0) // 왼쪽
                keepMoveDir = EPieceDir.LEFT;
            else if (absX <= absY && swipeDistance.y > 0) // 위
                keepMoveDir = EPieceDir.UP;
            else // 아래
                keepMoveDir = EPieceDir.DOWN;
        }
    }

    private void TouchStart(Vector2 touchPos)
    {
        touchCurrentPos = touchStartPos = touchPos;
        isHolding = false;
    }

    // 디버깅용
    private void RotateInput()
    {
        if (rotateAction != null && Input.GetKeyDown(KeyCode.E)) rotateAction.Invoke(1);
    }

    private void MoveInput()
    {
        if (moveAction != null && Input.GetKey(KeyCode.W)) moveAction.Invoke(EPieceDir.UP);
        else if (moveAction != null && Input.GetKey(KeyCode.A)) moveAction.Invoke(EPieceDir.LEFT);
        else if (moveAction != null && Input.GetKey(KeyCode.S)) moveAction.Invoke(EPieceDir.DOWN);
        else if (moveAction != null && Input.GetKey(KeyCode.D)) moveAction.Invoke(EPieceDir.RIGHT);
    }
}
