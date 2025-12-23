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
    [SerializeField] private TMP_Text cntText;
    public PieceDestroyer pieceDestroyer;
    private float coolTime = 5f;
    private int cnt = 2;

    private void Start()
    {
        hideImage.fillAmount = 0f;
        cntText.text = cnt.ToString();
    }

    // 쿨타임 대기 시간이면 버튼을 비활성화해서 클릭 못하게 만듦
    public void OnDestroyButtonClick()
    {
        if (cnt <= 0) return;

        bool isBroken = pieceDestroyer.StoneDestroy();
        if (isBroken)
        {
            hideImage.fillAmount = 1f;
            cnt--;
            cntText.text = cnt.ToString();
            StartCoroutine(ButtonTimerCoroutine());
        }
    }

    private IEnumerator ButtonTimerCoroutine()
    {
        float elasedTime = 0f;

        if (cnt > 0)
        {
            while (elasedTime < coolTime)
            {
                elasedTime += Time.deltaTime;
                hideImage.fillAmount = 1f - (elasedTime / coolTime);
                yield return null;
            }

            hideImage.fillAmount = 0f;
        }
        else
        {
            hideImage.fillAmount = 1f;
        }

    }
}
