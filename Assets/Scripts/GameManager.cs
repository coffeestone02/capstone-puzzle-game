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

// 게임관리 기능
public class GameManager : MonoBehaviour
{
    public string serverURL = "http://localhost:3000/score";
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

    // 테스트용 닉네임 생성
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
                Debug.Log("데이터 전송 성공: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("데이터 전송 실패: " + request.error);
            }
        }
    }
}