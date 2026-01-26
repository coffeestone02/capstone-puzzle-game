using TMPro;
using UnityEngine;

public class UIObstacle : MonoBehaviour
{
    private TMP_Text obstacleText;
    private Obstacle obstacle;

    private void Start()
    {
        obstacleText = GetComponent<TMP_Text>();
        obstacle = FindFirstObjectByType<Obstacle>();

        Managers.UI.updateObstacleText -= SetObstacleText;
        Managers.UI.updateObstacleText += SetObstacleText;
    }

    private void SetObstacleText()
    {
        int obstacleCount = Managers.Rule.obstacleSpawnLimit - Managers.Rule.obstacleCount;
        obstacleText.text = obstacle.nextSpawnDir.ToString() + "\n" + obstacleCount.ToString();
    }
}
