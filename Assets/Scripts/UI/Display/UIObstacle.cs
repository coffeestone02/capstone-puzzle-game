using TMPro;
using UnityEngine;

public class UIObstacle : MonoBehaviour
{
    private TMP_Text obstacleText;
    private Obstacle obstacle;

    private void OnEnable()
    {
        Managers.UI.updateObstacleText += SetObstacleText;
    }

    private void OnDisable()
    {
        Managers.UI.updateObstacleText -= SetObstacleText;
    }

    private void Start()
    {
        obstacleText = GetComponent<TMP_Text>();
        obstacle = FindFirstObjectByType<Obstacle>();
    }

    private void SetObstacleText()
    {
        int obstacleCount = Managers.Rule.obstacleSpawnLimit - Managers.Rule.obstacleCount;
        obstacleText.text = obstacle.nextSpawnDir.ToString() + "\n" + obstacleCount.ToString();
    }
}
