using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("�Ͻ����� �г�")]
    public GameObject pausePanel;            // �г� ������Ʈ
    public Animator pauseAnimator;           // Animator

    [Header("UI ��ư")]
    public Button resumeButton;
    public Button mainMenuButton;
    public Button pauseButton;               // ���� ����� �Ͻ����� ��ư

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);  // ó���� ����

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(() =>
            {
                if (!isPaused)
                    PauseGame();
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        // pauseAnimator.SetTrigger("Show");
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // �ð� ����
        SceneManager.LoadScene("Title");
    }
}
