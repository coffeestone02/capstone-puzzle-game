using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string gameMode;
    public string name;
    public int finalScore;
    public string totalPlaytime;
    public string updateDate;
}

public class WebDataManager : MonoBehaviour
{
    private string urlForCookie = "https://unity-and-data.du.r.appspot.com/cookie";
    private string urlToSend = "https://unity-and-data.du.r.appspot.com/score";
    private string cookie;

    private string TimeFormat(float playTime)
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

    // 인자로 게임 모드 받을 예정
    public void SendGameData(int score, float playTime)
    {
        StartCoroutine(CookieRequestCoroutine(score, playTime));
    }

    // 쿠키 요청
    private IEnumerator CookieRequestCoroutine(int score, float playTime)
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
                Debug.Log("Received Cookie: " + cookie);
                yield return StartCoroutine(SendGameDataCoroutine(score, playTime));
            }
        }
        else
        {
            Debug.Log("데이터 요청 실패: " + www.error);
        }
    }

    // 게임 데이터 전달
    private IEnumerator SendGameDataCoroutine(int score, float playTime)
    {
        PlayerData gameData = new PlayerData()
        {
            gameMode = "Classic",
            name = NameParser(),
            finalScore = score,
            totalPlaytime = TimeFormat(playTime),
            updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
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
