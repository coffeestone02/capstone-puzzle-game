using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleManager
{
    public static GoogleManager Instance { get; private set; }

    private const string LEADERBOARD_ID = "CgkIorrshYUbEAIQAg";

    private bool authTried = false;

    public void Init()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.DebugLogEnabled = true;
        SignIn();
    }

    public void SignIn()
    {
        if (IsSignedIn()) return;
        if (authTried) return;
        authTried = true;

        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        UnityEngine.Debug.Log("GPGS Auth status: " + status);

        if (status == SignInStatus.Success)
        {
            UnityEngine.Debug.Log("success");
        }
        else
        {
            UnityEngine.Debug.Log("fail");
        }
    }

    public bool IsSignedIn()
    {
        return Social.localUser != null && Social.localUser.authenticated;
    }

    // 리더보드 열기
    public void ShowLeaderboard()
    {
        if (!IsSignedIn())
        {
            UnityEngine.Debug.Log("로그인 안 됨 → 수동 로그인 시도 후 리더보드");
            PlayGamesPlatform.Instance.ManuallyAuthenticate((status) =>
            {
                UnityEngine.Debug.Log("Manual Auth status: " + status);
                if (status == SignInStatus.Success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_ID);
                }
            });
            return;
        }

        PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_ID);
    }

    // 점수 제출
    public void ReportScore(int score)
    {
        if (!IsSignedIn())
        {
            UnityEngine.Debug.Log("로그인 안 됨 → 점수 제출 스킵");
            return;
        }

        PlayGamesPlatform.Instance.ReportScore(score, LEADERBOARD_ID, success =>
        {
            UnityEngine.Debug.Log($"ReportScore({score}) success={success}");
        });
    }
}
