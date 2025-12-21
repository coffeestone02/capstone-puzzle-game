using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DestroyButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image hideImage;
    public PieceDestroyer pieceDestroyer;
    private float coolTime = 30f;

    private void Start()
    {
        button.enabled = true;
        hideImage.fillAmount = 0f;
    }

    // 쿨타임 대기 시간이면 버튼을 비활성화해서 클릭 못하게 만듦
    public void OnDestroyButtonClick()
    {
        pieceDestroyer.StoneDestroy();
        hideImage.fillAmount = 1f;
        button.enabled = false;
        StartCoroutine(ButtonTimerCoroutine());
    }

    private IEnumerator ButtonTimerCoroutine()
    {
        float elasedTime = 0f;

        while (elasedTime < coolTime)
        {
            elasedTime += Time.deltaTime;
            hideImage.fillAmount = 1f - (elasedTime / coolTime);
            yield return null;
        }

        hideImage.fillAmount = 0f;
        button.enabled = true;
    }
}
