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
        obstacleText.text = obstacle.nextSpawnDir.ToString() + "\n" + Managers.Rule.obstacleCount.ToString();
    }
}
