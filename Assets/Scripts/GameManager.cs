using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
<<<<<<< HEAD
using System;

[System.Serializable]
public class PlayerData
{
    public string gameMode;
    public string name;
    public int finalScore;
    public string totalPlaytime;
    public string updateDate;
}

// ���Ӱ��� ���
public class GameManager : MonoBehaviour
{
    public string serverURL = "http://localhost:3000/score";
=======
using UnityEngine.UI;
using System;

// 게임관리 기능
public class GameManager : MonoBehaviour
{
    public bool isPaused = false;
    public bool isOver = false;

>>>>>>> main
    public GameObject gameOverUI;
    private float playTime = 0f;

    private void Update()
    {
        playTime += Time.deltaTime;
    }

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

    private string TimeFormat()
    {
        string min = ((int)(playTime / 60)).ToString();
        
        if (int.Parse(min) < 10)
        {
            min = "0" + min;
        }

        string sec = ((int)(playTime % 60)).ToString();
        if (int.Parse(sec) < 10)
        {
            sec = "0" + sec;
        }

        return min + ":" + sec;
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
            totalPlaytime = TimeFormat(),
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