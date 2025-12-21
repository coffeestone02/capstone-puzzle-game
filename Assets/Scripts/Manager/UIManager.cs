using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("TMP_Text")]
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text playtimeText;
    public TMP_Text brokenBlockText;
    public TMP_Text bombThresholdText;

    [Header("Other Scripts")]
    [SerializeField] private Board mainBoard;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        UpdateTimeText();
        UpdateScoreText();
        UpdateLevelText();
    }

    private void UpdateTimeText()
    {
        playtimeText.text = scoreManager.GetTimeText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = mainBoard.score.ToString();
    }

    private void UpdateLevelText()
    {
        levelText.text = mainBoard.level.ToString();
    }

    private void UpdateBombText()
    {
        brokenBlockText.text = mainBoard.brokenBlockCount.ToString();
        bombThresholdText.text = mainBoard.bombSpawnThreshold.ToString();
    }
}
