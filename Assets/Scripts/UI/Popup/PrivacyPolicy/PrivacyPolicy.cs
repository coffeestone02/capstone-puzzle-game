using UnityEngine;
using UnityEngine.UI;

public class PrivacyPolicy : MonoBehaviour
{
    private const string KEY = "PRIVACY_ACCEPTED";

    [SerializeField] private GameObject privacyPolicy;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (PlayerPrefs.GetInt(KEY, 0) == 1)
        {
            privacyPolicy.SetActive(false);
            return;
        }

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt(KEY, 1);
            PlayerPrefs.Save();
            privacyPolicy.SetActive(false);
        });

        privacyPolicy.SetActive(true);
    }
}
