using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrivacyLink : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string privacyUrl = "https://dropblast.github.io/DropAndBlast.github.io/ ";

    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text text = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);

        if (linkIndex == -1) return;

        var linkInfo = text.textInfo.linkInfo[linkIndex];

        if (linkInfo.GetLinkID() == "privacy")
        {
            Application.OpenURL(privacyUrl);
        }
    }
}