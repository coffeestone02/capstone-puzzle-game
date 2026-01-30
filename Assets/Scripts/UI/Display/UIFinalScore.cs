using TMPro;
using UnityEngine;

public class UIFinalScore : MonoBehaviour
{
    void Start()
    {
        TMP_Text finalScore = GetComponent<TMP_Text>();
        finalScore.text = Managers.Score.score.ToString();
    }
}
