using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    // TEMP
    public Action<EPieceDir> moveAction = null;
    public Action<int> rotateAction = null;
    public Action pauseAction = null;

    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private EPieceDir keepMoveDir;
    private float swipeRange = 80f;
    private bool isHolding = false;

    private float holdMoveInterval = 0.11f;
    private float nextHoldMoveTime = 0f;
    private float ignoreInputUntil = 0f;

    public void OnUpdate()
    {
        if (Time.unscaledTime < ignoreInputUntil) return;
        
        TouchInput();
        PauseAction();

        // DEBUG
        RotateInput();
        MoveInput();
    }

    private void PauseAction()
    {
        if (Managers.Rule.isOver == false && UnityEngine.Input.GetKeyDown(KeyCode.Escape)) // 뒤로가기 누르면 일시정지
        {
            pauseAction?.Invoke();
        }
    }


    private void KeepMove()
    {
        if (isHolding == false) return;
        if (Time.unscaledTime < nextHoldMoveTime) return;

        nextHoldMoveTime = Time.unscaledTime + holdMoveInterval;
        moveAction?.Invoke(keepMoveDir);
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
        Managers.Audio.PlaySFX("MoveSFX"); // 반대 방향으로 이동해서 소리나는 문제 발생
        touchCurrentPos = touchStartPos = touchPos;
        isHolding = false;
    }

    // 디버깅용
    private void RotateInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Managers.Audio.PlaySFX("RotateSFX");
            rotateAction?.Invoke(1);
        }
    }

    private void MoveInput()
    {
        if (Input.GetKey(KeyCode.W)) moveAction?.Invoke(EPieceDir.UP);
        else if (Input.GetKey(KeyCode.A)) moveAction?.Invoke(EPieceDir.LEFT);
        else if (Input.GetKey(KeyCode.S)) moveAction?.Invoke(EPieceDir.DOWN);
        else if (Input.GetKey(KeyCode.D)) moveAction?.Invoke(EPieceDir.RIGHT);
    }

    public void BlockInput(float seconds)
    {
        ignoreInputUntil = Mathf.Max(ignoreInputUntil, Time.unscaledTime + seconds);
        isHolding = false; // 혹시 홀드 이동 남아있으면 끊기
        nextHoldMoveTime = Time.unscaledTime + holdMoveInterval;
    }
}
