using UnityEngine;
using UnityEngine.SceneManagement;

//���Ӱ��� ���
public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public void GameOver() //���� ����
    {
        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true); // UI Ȱ��ȭ
    }

    public void RestartGame() // �����
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}