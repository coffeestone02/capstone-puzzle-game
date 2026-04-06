using TMPro;
using UnityEngine;

public class UIObstacleCount : MonoBehaviour
{
    private TMP_Text text;

    private void OnEnable()
    {
        Managers.UI.updateObstacleText += SetText;
    }

    private void OnDisable()
    {
        Managers.UI.updateObstacleText -= SetText;
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void SetText()
    {
        int obstacleCount = Managers.Rule.obstacleSpawnLimit - Managers.Rule.obstacleCount;
        Debug.Log($"[UIObstacleCount] SetText »£√‚µ  / limit={Managers.Rule.obstacleSpawnLimit}, count={Managers.Rule.obstacleCount}, result={obstacleCount}");
        text.text = obstacleCount.ToString();
    }
}