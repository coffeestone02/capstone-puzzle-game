using System;
using UnityEngine;

public class UIManager
{
    public Action updateScoreText = null;
    public Action updateLevelText = null;
    public Action updatePlaytimeText = null;
    public Action updateBombText = null;
    public Action updateObstacleText = null;

    public void OnUpdate()
    {
        SetScoreText();
        SetLevelText();
        SetPlaytimeText();
        SetBombText();
        SetObstacleText();
    }

    private void SetScoreText()
    {
        if (updateScoreText != null)
            updateScoreText.Invoke();
    }

    private void SetLevelText()
    {
        if (updateLevelText != null)
            updateLevelText.Invoke();
    }

    private void SetPlaytimeText()
    {
        if (updatePlaytimeText != null)
            updatePlaytimeText.Invoke();
    }

    private void SetBombText()
    {
        if (updateBombText != null)
            updateBombText.Invoke();
    }

    private void SetObstacleText()
    {
        if (updateObstacleText != null)
            updateObstacleText.Invoke();
    }
}
