using System;
using UnityEngine;

public class UIManager
{
    public Action updateScoreText = null;
    public Action updateLevelText = null;
    public Action updatePlaytimeText = null;
    public Action updateBombText = null;
    public Action updateObstacleText = null;
    public Action updateBombGauge = null;

    public void OnUpdate()
    {
        SetScoreText();
        SetLevelText();
        SetPlaytimeText();
        SetBombText();
        SetObstacleText();
        SetBombGauge();
    }

    public void ShowPopup(string name)
    {
        GameObject parent = GameObject.Find("UICanvas");
        GameObject popup = parent.transform.Find(name).gameObject;
        if (popup == null)
        {
            Debug.Log($"{name} 팝업을 찾을 수 없음");
            return;
        }

        popup.SetActive(true);
    }

    public void ClosePopup(string name)
    {
        GameObject popup = GameObject.Find(name);
        if (popup == null)
        {
            Debug.Log($"{name} 팝업을 찾을 수 없음");
            return;
        }

        popup.SetActive(false);
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

    private void SetBombGauge()
    {
        if (updateBombGauge != null)
            updateBombGauge.Invoke();
    }
}
