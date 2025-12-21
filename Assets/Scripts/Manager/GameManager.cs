using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

// 게임관리 기능
public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject gamePausePanel;
    public GameObject optionPanel;
    public bool isPause = false;
    public bool isOver = false;
    private SaveController saveController;

    private void Awake()
    {
        saveController = FindObjectOfType<SaveController>();
    }

    private void Update()
    {
        if (gameOverPanel != null && isOver)
        {
            gameOverPanel.SetActive(true);
        }
        else if (optionPanel.activeSelf == false && gamePausePanel != null && Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
            gamePausePanel.SetActive(isPause);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void GamePlay()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void Restart()
    {
        SaveSystem.Clear();
        AudioManager.instance.PlayBgm(1);
        SceneManager.LoadScene("GamePlayScene");
    }

    public void Resume()
    {
        isPause = false;
        gamePausePanel.SetActive(isPause);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 상에서 종료
#else
        Application.Quit(); // 앱 종료
#endif
    }

    private void OnApplicationPause(bool pause) //일시정지 시 저장
    {
        if (pause && saveController != null)
            saveController.SaveNow();
    }

    private void OnApplicationQuit() // 나갈 때 저장
    {
        if (saveController != null)
            saveController.SaveNow();
    }
}