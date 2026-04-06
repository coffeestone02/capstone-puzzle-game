using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class BannerAdManager : MonoBehaviour
{
    public static BannerAdManager Instance { get; private set; }

    private BannerView bannerView;
    private bool isBannerVisible = false;

    // 테스트 배너 ID
    private const string BannerAdUnitId = "ca-app-pub-9546000065350530/4858357814";

    // 자동으로 배너를 띄울 씬 이름
    private const string AutoShowSceneName = "GamePlayScene";

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == AutoShowSceneName)
        {
            ShowBanner();
        }
        else
        {
            HideBanner();
        }
    }

    private void CreateBannerIfNeeded()
    {
        if (bannerView != null)
            return;

        // 하단 고정 배너
        bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        var adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }

    public void ShowBanner()
    {
        CreateBannerIfNeeded();

        if (bannerView == null)
            return;

        bannerView.Show();
        isBannerVisible = true;
    }

    public void HideBanner()
    {
        if (bannerView == null)
            return;

        bannerView.Hide();
        isBannerVisible = false;
    }

    public void DestroyBanner()
    {
        if (bannerView == null)
            return;

        bannerView.Destroy();
        bannerView = null;
        isBannerVisible = false;
    }

    public bool IsBannerVisible()
    {
        return isBannerVisible;
    }
}