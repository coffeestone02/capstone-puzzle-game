using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    private static bool isInitialized = false;

    void Awake()
    {
        if (isInitialized) return;

        isInitialized = true;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob 초기화 완료");
        });
    }
}