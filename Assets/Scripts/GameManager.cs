using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class PlayerData
{
    public string gameMode;
    public string name;
    public int finalScore;
    public string updateDate;
}

// ���Ӱ��� ���
public class GameManager : MonoBehaviour
{
    public string serverURL = "http://localhost:3000/score";
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

    // �׽�Ʈ�� �г��� ����
    private string RandomNameGenerate()
    {
        return "TestPlayer" + UnityEngine.Random.Range(1, 10000).ToString();
    }

    public void SendGameData(int score)
    {
        StartCoroutine(SendGameDataCoroutine(score));
    }

    private IEnumerator SendGameDataCoroutine(int score)
    {
        PlayerData gameData = new PlayerData()
        {
            gameMode = "Classic",
            name = RandomNameGenerate(),
            finalScore = score,
            updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        string jsonData = JsonUtility.ToJson(gameData);

        using (UnityWebRequest request = new UnityWebRequest(serverURL, "POST"))
        {
            byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("������ ���� ����: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("������ ���� ����: " + request.error);
            }
        }
    }
}