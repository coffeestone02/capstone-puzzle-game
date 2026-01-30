using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    private TMP_Text scoreText;

    private void OnEnable()
    {
        Managers.UI.updateScoreText += SetScoreText;
    }

    private void OnDisable()
    {
        Managers.UI.updateScoreText -= SetScoreText;
    }

    private void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    private void SetScoreText()
    {
        scoreText.text = Managers.Score.score.ToString();
    }
}
