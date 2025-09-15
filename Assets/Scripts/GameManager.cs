using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

// 게임관리 기능
public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool isOver = false;

    public GameObject gameOverUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
    }

    public void GameOver()
    {
        Invoke("loadGameOver", 2f);
    }

    private void loadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void GamePlay()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void Exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 상에서 종료
        #else
        Application.Quit(); // 앱 종료
        #endif
    }
}