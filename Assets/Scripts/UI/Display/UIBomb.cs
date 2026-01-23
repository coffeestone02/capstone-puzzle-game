using TMPro;
using UnityEngine;

public class UIBomb : MonoBehaviour
{
    private TMP_Text bombText;

    private void Start()
    {
        bombText = GetComponent<TMP_Text>();

        Managers.UI.updateBombText -= SetBombText;
        Managers.UI.updateBombText += SetBombText;
    }

    private void SetBombText()
    {
        bombText.text = "Bomb: " + Managers.Rule.BlockCounter.ToString() + "/" + Managers.Rule.bombSpawnLimit.ToString();
    }
}
