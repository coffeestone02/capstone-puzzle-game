using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;

public class DataSendTest : MonoBehaviour
{
    public TMP_InputField serverURL;
    public TMP_InputField gameMode;
    public TMP_InputField playerName;
    public TMP_InputField finalScore;

    public string defaultServerURL = "http://localhost:3000/score";
    private float playTime = 0f;

    private void Update()
    {
        playTime += Time.deltaTime;
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

    private string RandomNameGenerate()
    {
        return "TestPlayer" + UnityEngine.Random.Range(1, 10000).ToString();
    }

    public void SendGameData()
    {
        StartCoroutine(SendGameDataCoroutine());
    }

    private PlayerData SetPlayerValues()
    {
        string gameModeValue;
        string playerNameValue;
        int finalScoreValue;

        // URL 
        if (serverURL.text == "")
        {
            Debug.Log("serverURL 비어있음. 기본값 http://localhost:3000/score로 설정");
        }
        else
        {
            defaultServerURL = serverURL.text;
            Debug.Log($"URL 입력 완료. {serverURL}로 연결");
        }

        // 게임모드
        if (gameMode.text == "")
        {
            gameModeValue = "Classic";
            Debug.Log("gameMode 비어있음. 기본값 Classic으로 설정");
        }
        else
        {
            gameModeValue = gameMode.text;
            Debug.Log($"gameMode 입력 완료. {gameMode.text}로 설정");
        }

        // 이름
        if (playerName.text == "")
        {
            playerNameValue = RandomNameGenerate();
            Debug.Log("playerName 비어있음. 랜덤 이름 설정");
        }
        else
        {
            playerNameValue = playerName.text;
            Debug.Log($"playerName 입력 완료. {playerName.text}로 연결");
        }

        // 점수
        if (finalScore.text == "")
        {
            finalScoreValue = UnityEngine.Random.Range(1, 100000);
            Debug.Log("finalScore 비어있음. 랜덤 점수 설정");
        }
        else
        {
            finalScoreValue = int.Parse(finalScore.text);
            Debug.Log($"finalScore 입력 완료. {int.Parse(finalScore.text)}로 설정");
        }
        
        return new PlayerData()
            {
                gameMode = gameModeValue,
                name = playerNameValue,
                finalScore = finalScoreValue,
                totalPlaytime = TimeFormat(),
                updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
    }

    private IEnumerator SendGameDataCoroutine()
    {
        PlayerData gameData = SetPlayerValues();
        string jsonData = JsonUtility.ToJson(gameData);

        using (UnityWebRequest request = new UnityWebRequest(defaultServerURL, "POST"))
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
