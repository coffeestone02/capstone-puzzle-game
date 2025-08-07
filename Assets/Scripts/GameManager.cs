using UnityEngine;
using UnityEngine.SceneManagement;

//게임관리 기능
public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public void GameOver() //게임 오버
    {
        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true); // UI 활성화
    }

    public void RestartGame() // 재시작
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}