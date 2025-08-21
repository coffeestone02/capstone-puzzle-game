using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button mainMenuButton;
    public Button retryButton;
    public Button rankingButton;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(MainMenu);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);

        if (rankingButton != null)
            rankingButton.onClick.AddListener(Ranking);
    }

    // 처음으로 (SampleScene으로 이동)
    public void MainMenu()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // 다시하기 (Play 씬으로 이동)
    public void RetryGame()
    {
        SceneManager.LoadScene("Play");
    }

    // 랭킹 (Ranking 씬으로 이동)
    public void Ranking()
    {
        SceneManager.LoadScene("Ranking");
    }
}
