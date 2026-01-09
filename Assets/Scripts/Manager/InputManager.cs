using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Vector2 touchStartPos { get; private set; }
    public Vector2 touchNowPos { get; private set; }

    // 테스트용
    public EPieceDir moveDir { get; private set; }
    public EPieceDir rotateDir { get; private set; }

    public void OnUpdate()
    {
        TouchInput();
        KeyboardInput();
    }

    private void KeyboardInput()
    {
        if (Input.GetKey(KeyCode.Q))
            rotateDir = EPieceDir.LEFT;
        else if (Input.GetKey(KeyCode.E))
            rotateDir = EPieceDir.RIGHT;
        else if (Input.GetKey(KeyCode.W))
            moveDir = EPieceDir.UP;
        else if (Input.GetKey(KeyCode.A))
            moveDir = EPieceDir.LEFT;
        else if (Input.GetKey(KeyCode.S))
            moveDir = EPieceDir.DOWN;
        else if (Input.GetKey(KeyCode.D))
            moveDir = EPieceDir.RIGHT;
        else
            moveDir = rotateDir = EPieceDir.NONE;
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
