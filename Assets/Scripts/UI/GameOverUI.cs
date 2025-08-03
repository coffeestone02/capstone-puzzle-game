using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("��ư ����")]
    public Button mainMenuButton;
    public Button retryButton;
    public Button rankingButton;

    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ ����
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(MainMenu);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);

        if (rankingButton != null)
            rankingButton.onClick.AddListener(Ranking);
    }

    // ó������ (SampleScene���� �̵�)
    public void MainMenu()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // �ٽ��ϱ� (Play ������ �̵�)
    public void RetryGame()
    {
        SceneManager.LoadScene("Play");
    }

    // ��ŷ (Ranking ������ �̵�)
    public void Ranking()
    {
        SceneManager.LoadScene("Ranking");
    }
}
