using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

// 쿠키 주소: https://unity-and-data.du.r.appspot.com/cookie
// 전송 주소: https://unity-and-data.du.r.appspot.com/score
public class DataSendTest : MonoBehaviour
{
    public TMP_InputField serverURL;
    public TMP_InputField gameMode;
    public TMP_InputField playerName;
    public TMP_InputField finalScore;

    public string urlForCookie = "https://unity-and-data.du.r.appspot.com/cookie";
    public string urlToSend = "http://localhost:3000/score";
    private float playTime = 0f;
    private string cookie;

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

    public void SendGameData()
    {
        StartCoroutine(CookieRequestCoroutine());
    }

    private PlayerData SetPlayerValues()
    {
        string gameModeValue;
        int finalScoreValue;

        // URL 
        if (serverURL.text == "")
        {
            Debug.Log("serverURL 비어있음. 기본값 http://localhost:3000/score로 설정");
        }
        else
        {
            urlToSend = serverURL.text;
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
            name = NameParser(),
            finalScore = finalScoreValue,
            totalPlaytime = TimeFormat(),
            updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    // 쿠키에서 이름만 추출함
    private string NameParser()
    {
        string[] parts = cookie.Split(';');
        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (trimmed.StartsWith("username="))
            {
                Debug.Log(trimmed.Substring("username=".Length));
                return trimmed.Substring("username=".Length);
            }
        }

        return null;
    }

    private IEnumerator CookieRequestCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(urlForCookie);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // 쿠키 추출
            Dictionary<string, string> headers = www.GetResponseHeaders();
            if (headers.TryGetValue("Set-Cookie", out string setCookie))
            {
                cookie = setCookie;
                Debug.Log("쿠키 받음: " + cookie);

                yield return StartCoroutine(SendGameDataCoroutine());
            }
        }
        else
        {
            Debug.Log("데이터 요청 실패: " + www.error);
        }
    }

    private IEnumerator SendGameDataCoroutine()
    {
        PlayerData gameData = SetPlayerValues(); // 데이터 생성
        string jsonData = JsonUtility.ToJson(gameData); // json으로 변환

        using (UnityWebRequest request = new UnityWebRequest(urlToSend, "POST"))
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
