using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleManager : MonoBehaviour
{
    public static GoogleManager Instance { get; private set; }

    private const string LEADERBOARD_ID = "CgkIorrshYUbEAIQAg";

    private bool initialized = false;
    private bool authTried = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (initialized) return;
        initialized = true;

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        SignIn();
    }

    public void SignIn()
    {
        if (IsSignedIn()) return;
        if (authTried) return;

        authTried = true;
        UnityEngine.Debug.Log("GPGS auto sign-in try");

        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            UnityEngine.Debug.Log("GPGS Auth status: " + status);
            UnityEngine.Debug.Log("GPGS authenticated: " + IsSignedIn());

            if (status == SignInStatus.Success)
            {
                UnityEngine.Debug.Log("GPGS login success");
            }
            else
            {
                UnityEngine.Debug.Log("GPGS login fail");
                authTried = false;
            }
        });
    }

    public bool IsSignedIn()
    {
        return Social.localUser != null && Social.localUser.authenticated;
    }

    public void ShowLeaderboard()
    {
        UnityEngine.Debug.Log("ShowLeaderboard called");
        UnityEngine.Debug.Log("Before leaderboard auth: " + IsSignedIn());

        if (IsSignedIn())
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_ID);
            return;
        }

        UnityEngine.Debug.Log("로그인 안 됨 → 수동 로그인 시도");

        PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
        {
            UnityEngine.Debug.Log("Manual Auth status: " + status);
            UnityEngine.Debug.Log("After manual auth: " + IsSignedIn());

            if (status == SignInStatus.Success && IsSignedIn())
            {
                PlayGamesPlatform.Instance.ShowLeaderboardUI(LEADERBOARD_ID);
            }
            else
            {
                UnityEngine.Debug.Log("수동 로그인 실패 → 리더보드 미오픈");
            }
        });
    }

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