using TMPro;
using UnityEngine;

public class UILevel : MonoBehaviour
{
    private TMP_Text levelText;

    private void Start()
    {
        levelText = GetComponent<TMP_Text>();

        Managers.UI.updateLevelText -= SetlevelText;
        Managers.UI.updateLevelText += SetlevelText;
    }

    private void SetlevelText()
    {
        levelText.text = "Level: " + Managers.Score.GetLevel().ToString();
    }
}
