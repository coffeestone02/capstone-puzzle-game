using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    private TMP_Text scoreText;

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();

        Managers.UI.updateScoreText -= SetScoreText;
        Managers.UI.updateScoreText += SetScoreText;
    }

    private void SetScoreText()
    {
        scoreText.text = "Score: " + Managers.Score.score.ToString();
    }
}
