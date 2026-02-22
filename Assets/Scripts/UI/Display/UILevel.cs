using TMPro;
using UnityEngine;

public class UILevel : MonoBehaviour
{
    private TMP_Text levelText;

    private void OnEnable()
    {
        Managers.UI.updateLevelText += SetlevelText;
    }

    private void OnDisable()
    {
        Managers.UI.updateLevelText -= SetlevelText;
    }

    private void Start()
    {
        levelText = GetComponent<TMP_Text>();
    }

    private void SetlevelText()
    {
        int level = Managers.Score.GetLevel();

        if (level >= 5)
            levelText.text = "Lv. Max";
        else
            levelText.text = "Lv. " + level.ToString();
    }
}
