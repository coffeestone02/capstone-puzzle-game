using UnityEngine;
using TMPro;

public class ReviveManager
{
    public bool isActive { get; private set; }
    public float remainTime { get; private set; }

    private const float REVIVE_TIME = 5f;

    private TMP_Text _timerText;
    private bool _isInitializedUI;

    public void Init()
    {
        isActive = false;
        remainTime = 0f;
    }

    public void BindUI(TMP_Text timerText)
    {
        _timerText = timerText;
        _isInitializedUI = true;
    }

    private void RefreshTimerText()
    {
        if (_isInitializedUI == false || _timerText == null)
            return;

        _timerText.text = Mathf.CeilToInt(remainTime).ToString();
    }

    public void OnUpdate()
    {
        if (isActive == false) return;

        remainTime -= Time.unscaledDeltaTime;

        if (remainTime <= 0f)
        {
            remainTime = 0f;
            RefreshTimerText();
            Timeout();
            return;
        }

        RefreshTimerText();
    }

    public void Open()
    {
        bool isOnline = Application.internetReachability != NetworkReachability.NotReachable;
        bool canShowAd = RewardAdManager.Instance != null && RewardAdManager.Instance.IsAdReady();

        if (Managers.Rule.CanRevive == false || isOnline == false || canShowAd == false)
        {
            Managers.Rule.FinalizeGameOver();
            Managers.UI.ShowPopup("GameoverPopup");
            return;
        }

        isActive = true;
        remainTime = REVIVE_TIME;

        Managers.UI.ShowPopup("RevivePopup");
        RefreshTimerText();
    }

    public void Close()
    {
        isActive = false;
        remainTime = 0f;

        Managers.UI.ClosePopup("RevivePopup");
    }

    public void ReviveSuccess()
    {
        if (isActive == false) return;

        Close();
    }

    private void Timeout()
    {
        if (isActive == false) return;

        Close();
        Managers.Rule.FinalizeGameOver();
        Managers.UI.ShowPopup("GameoverPopup");
    }
}