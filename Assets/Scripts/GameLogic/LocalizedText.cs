using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key;

    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        ApplyText();
    }

    public void ApplyText()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("LocalizationManager°¡ ¾À¿¡ ¾øÀ½");
            return;
        }

        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();

        textComponent.text = LocalizationManager.Instance.GetText(key);

        TMP_FontAsset font = LocalizationManager.Instance.GetCurrentFont();
        if (font != null)
            textComponent.font = font;
    }
}