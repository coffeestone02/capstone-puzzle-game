using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrivacyPolicy : MonoBehaviour
{
    private const string KEY = "PRIVACY_ACCEPTED";

    [SerializeField] private GameObject privacyPolicy;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnClickContinue);
    }

    private IEnumerator Start()
    {
        yield return null;

        int accepted = PlayerPrefs.GetInt(KEY, 0);
        Debug.Log("PRIVACY_ACCEPTED = " + accepted);

        bool shouldShow = accepted != 1;
        privacyPolicy.SetActive(shouldShow);

        Debug.Log("privacyPolicy activeSelf = " + privacyPolicy.activeSelf);
        Debug.Log("privacyPolicy activeInHierarchy = " + privacyPolicy.activeInHierarchy);
    }

    private void OnClickContinue()
    {
        PlayerPrefs.SetInt(KEY, 1);
        PlayerPrefs.Save();
        privacyPolicy.SetActive(false);
        Debug.Log("Privacy accepted");
    }

    [ContextMenu("Reset Privacy Accepted")]
    private void ResetPrivacyAccepted()
    {
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
        Debug.Log("PRIVACY_ACCEPTED reset");
    }
}