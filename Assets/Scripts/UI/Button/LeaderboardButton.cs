using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class LeaderboardButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        ShowLeaderBoardUI();
        Managers.Audio.PlaySFX("ButtonSFX");
    }

    private void ShowLeaderBoardUI()
    {
        if (!Managers.Google.IsSignedIn())
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate((status) =>
            {
                UnityEngine.Debug.Log("Manual Auth status: " + status);
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIorrshYUbEAIQAg");
                }
            });
            return;
        }

        PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIorrshYUbEAIQAg");
    }

}