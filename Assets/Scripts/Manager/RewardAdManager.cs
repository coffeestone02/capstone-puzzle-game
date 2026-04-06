using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class RewardAdManager : MonoBehaviour
{
    public static RewardAdManager Instance;

    private RewardedAd rewardedAd;

    private const string testAdUnitId = "ca-app-pub-9546000065350530/1136118118";

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
        }
    }

    private void Start()
    {
        LoadRewardAd();
    }

    public void LoadRewardAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(testAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("보상형 광고 로드 실패: " + error);
                return;
            }

            rewardedAd = ad;
            Debug.Log("보상형 광고 로드 완료");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("보상형 광고 닫힘");
                LoadRewardAd();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.Log("보상형 광고 표시 실패: " + adError);
                LoadRewardAd();
            };
        });
    }

    public bool IsAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public void ShowRewardAd(Action onRewarded)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"보상 지급됨: {reward.Type}, {reward.Amount}");
                onRewarded?.Invoke();
            });
        }
        else
        {
            Debug.Log("보상형 광고가 아직 준비되지 않음");
        }
    }
}