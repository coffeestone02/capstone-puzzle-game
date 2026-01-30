using System;
using TMPro;
using UnityEngine;

public class UIPlaytime : MonoBehaviour
{
    private TMP_Text playtimeText;

    private void OnEnable()
    {
        Managers.UI.updatePlaytimeText += SetPlaytimeText;
    }

    private void OnDisable()
    {
        Managers.UI.updatePlaytimeText -= SetPlaytimeText;
    }

    private void Start()
    {
        playtimeText = GetComponent<TMP_Text>();
    }

    private void SetPlaytimeText()
    {
        playtimeText.text = TimeToString();
    }

    // 플레이 시간을 00:00 텍스트 형태로 반환
    private string TimeToString()
    {
        float playtime = Managers.Score.playtime;
        string min;
        string sec;

        // 분
        if (playtime < 60f)
            min = "00";
        else if (playtime < 600f)
            min = "0" + Math.Truncate(playtime / 60f).ToString();
        else
            min = Math.Truncate(playtime / 60f).ToString();

        // 초
        if (Math.Truncate(playtime % 60f) < 10f)
            sec = "0" + Math.Truncate(playtime % 60f).ToString();
        else
            sec = Math.Truncate(playtime % 60f).ToString();

        return min + ":" + sec;
    }
}
