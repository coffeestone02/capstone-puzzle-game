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
        levelText.text = "Level: " + Managers.Score.GetLevel().ToString();
    }
}
