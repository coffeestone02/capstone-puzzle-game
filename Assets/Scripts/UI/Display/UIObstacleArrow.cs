using UnityEngine;
using UnityEngine.UI;

public class UIObstacleArrow : MonoBehaviour
{
    private Image arrowImage;
    private Obstacle obstacle;

    private void OnEnable()
    {
        Managers.UI.updateObstacleText += SetArrow;
    }

    private void OnDisable()
    {
        Managers.UI.updateObstacleText -= SetArrow;
    }

    private void Start()
    {
        arrowImage = GetComponent<Image>();
        obstacle = FindFirstObjectByType<Obstacle>();
        SetArrow();
    }

    private void SetArrow()
    {
        if (arrowImage == null)
            arrowImage = GetComponent<Image>();

        if (obstacle == null)
            obstacle = FindFirstObjectByType<Obstacle>();

        if (arrowImage == null || obstacle == null)
            return;

        float zRotation = 0f;

        switch (obstacle.nextSpawnDir)
        {
            case EPieceDir.UP:
                zRotation = 180f;
                break;

            case EPieceDir.DOWN:
                zRotation = 0f;
                break;

            case EPieceDir.RIGHT:
                zRotation = 90f;
                break;

            case EPieceDir.LEFT:
                zRotation = -90f;
                break;
        }

        arrowImage.rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
    }
}