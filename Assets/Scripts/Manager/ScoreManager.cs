using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 플레이타임 및 점수 기록(구글 랭킹 리더보드에 쓰일 예정)
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Board mainBoard;
    private int score;
    private float playTime;

    private void Update()
    {
        if (gameManager.isOver || gameManager.isPause)
        {
            return;
        }

        score = mainBoard.score;
        playTime += Time.deltaTime;
    }

    // 플레이 시간을 00:00 텍스트 형태로 반환
    public string GetTimeText()
    {
        string min;
        string sec;

        // 분
        if (playTime < 60f)
        {
            min = "00";
        }
        else if (playTime < 600f)
        {
            min = "0" + Math.Truncate(playTime / 60f).ToString();
        }
        else
        {
            min = Math.Truncate(playTime / 60f).ToString();
        }

        // 초
        if (Math.Truncate(playTime % 60f) < 10f)
        {
            sec = "0" + Math.Truncate(playTime % 60f).ToString();
        }
        else
        {
            sec = Math.Truncate(playTime % 60f).ToString();
        }

        return min + ":" + sec;
    }
}
