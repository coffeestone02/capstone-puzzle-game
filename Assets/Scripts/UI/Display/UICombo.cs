using TMPro;
using UnityEngine;

public class UICombo : MonoBehaviour
{
    private TMP_Text comboText;

    private void Awake()
    {
        comboText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (Managers.Instance == null) return;
        Managers.UI.updateComboText += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (Managers.Instance == null) return;
        Managers.UI.updateComboText -= Refresh;
    }

    private void Refresh()
    {
        int combo = Managers.Score.combo;

        if (combo < 2)
        {
            comboText.text = "";
        }
        else
        {
            comboText.text = "Combo " + combo.ToString();
        }
    }
}