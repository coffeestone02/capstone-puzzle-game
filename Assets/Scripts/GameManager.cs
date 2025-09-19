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

// °ÔÀÓ°ü¸® ±â´É
public class GameManager : MonoBehaviour
{
    public string serverURL = "http://localhost:3000/score";
=======
using UnityEngine.UI;
using System;

// ê²Œì„ê´€ë¦¬ ê¸°ëŠ¥
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
        UnityEditor.EditorApplication.isPlaying = false; // ì—ë””í„° ìƒì—ì„œ ì¢…ë£Œ
        #else
        Application.Quit(); // ì•± ì¢…ë£Œ
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

    // Å×½ºÆ®¿ë ´Ğ³×ÀÓ »ı¼º
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
                Debug.Log("µ¥ÀÌÅÍ Àü¼Û ¼º°ø: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("µ¥ÀÌÅÍ Àü¼Û ½ÇÆĞ: " + request.error);
            }
        }
    }
}