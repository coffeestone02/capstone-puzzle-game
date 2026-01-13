using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Vector2 touchStartPos { get; private set; }
    public Vector2 touchNowPos { get; private set; }

    // TEMP
    public Action<EPieceDir> moveAction = null;

    public void OnUpdate()
    {
        // TouchInput();
        MoveInput();
    }

    private void MoveInput()
    {
        if (moveAction != null && Input.GetKey(KeyCode.W)) moveAction.Invoke(EPieceDir.UP);
        else if (moveAction != null && Input.GetKey(KeyCode.A)) moveAction.Invoke(EPieceDir.LEFT);
        else if (moveAction != null && Input.GetKey(KeyCode.S)) moveAction.Invoke(EPieceDir.DOWN);
        else if (moveAction != null && Input.GetKey(KeyCode.D)) moveAction.Invoke(EPieceDir.RIGHT);
    }

    private void TouchInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0); // 터치 정보 가져옴

        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touchNowPos = touch.position;
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    touchNowPos = touch.position;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    break;
            }
        }
    }
}
