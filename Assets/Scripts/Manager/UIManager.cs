using System;
using UnityEngine;

public class UIManager
{
    public Action updateScoreText = null;
    public Action updateLevelText = null;
    public Action updatePlaytimeText = null;
    public Action updateComboText = null;
    public Action updateBombText = null;
    public Action updateObstacleText = null;
    public Action updateBombGauge = null;
        
    public void OnUpdate()
    {
        SetScoreText();
        SetLevelText();
        SetPlaytimeText();
        SetComboText();
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
        updateScoreText?.Invoke();
    }

    private void SetLevelText()
    {
        updateLevelText?.Invoke();
    }

    private void SetPlaytimeText()
    {
        updatePlaytimeText?.Invoke();
    }

    private void SetComboText()
    {
        updateComboText?.Invoke();
    }

    private void SetBombText()
    {
        updateBombText?.Invoke();
    }

    private void SetObstacleText()
    {
        updateObstacleText?.Invoke();
    }

    private void SetBombGauge()
    {
        updateBombGauge?.Invoke();
    }
}
