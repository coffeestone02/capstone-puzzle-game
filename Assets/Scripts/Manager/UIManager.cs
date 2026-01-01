using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class UIManager
{
    private GameObject gameOverPanel;
    private GameObject gamePausePanel;
    private GameObject optionPanel;

    private TMP_Text scoreText;
    private TMP_Text levelText;
    private TMP_Text playtimeText;
    private TMP_Text brokenBlockText;
    private TMP_Text bombLimitText;

    public void Init()
    {
        Transform settingParent = GameObject.Find("Setting And Over Canvas").transform; // 비활성화 오브젝트를 찾기 위함
        gameOverPanel = settingParent.Find("Gameover Panel").GetComponent<GameObject>();
        gamePausePanel = settingParent.Find("Pause Panel").GetComponent<GameObject>();
        optionPanel = settingParent.Find("Option Panel").GetComponent<GameObject>();

        scoreText = GameObject.Find("Score Text").GetComponent<TMP_Text>();
        levelText = GameObject.Find("Level Text").GetComponent<TMP_Text>();
        playtimeText = GameObject.Find("Playtime Text").GetComponent<TMP_Text>();
        brokenBlockText = GameObject.Find("Broken Count Text").GetComponent<TMP_Text>();
        bombLimitText = GameObject.Find("Bomb Limit Text").GetComponent<TMP_Text>();

        bombLimitText.text = GameManager.Instance.board.bombSpawnLimit.ToString();
    }

    public void OnUpdate()
    {
        UpdateStatusText();
    }

    // score, level, time, bomb text
    private void UpdateStatusText()
    {
        scoreText.text = GameManager.Instance.score.ToString();

        levelText.text = GameManager.Instance.level.ToString();

        playtimeText.text = TimeToString(GameManager.Instance.playtime);

        brokenBlockText.text = GameManager.Instance.board.brokenBlockCount.ToString();
    }

    // 플레이 시간을 00:00 텍스트 형태로 반환
    public string TimeToString(float playTime)
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
