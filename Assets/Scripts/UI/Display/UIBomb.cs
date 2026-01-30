using TMPro;
using UnityEngine;

public class UIBomb : MonoBehaviour
{
    private TMP_Text bombText;

    private void OnEnable()
    {
        Managers.UI.updateBombText += SetBombText;
    }

    private void OnDisable()
    {
        Managers.UI.updateBombText -= SetBombText;
    }

    private void Start()
    {
        bombText = GetComponent<TMP_Text>();
    }

    private void SetBombText()
    {
        bombText.text = "Bomb: " + Managers.Rule.BlockCounter.ToString() + "/" + Managers.Rule.bombSpawnLimit.ToString();
    }
}
