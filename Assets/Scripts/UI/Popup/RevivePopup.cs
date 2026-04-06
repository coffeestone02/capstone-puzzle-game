using UnityEngine;
using TMPro;

public class RevivePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Start()
    {
        Managers.Revive.BindUI(timerText);
    }
}