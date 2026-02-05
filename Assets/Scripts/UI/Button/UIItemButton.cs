using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIItemButton : MonoBehaviour
{
    protected Button btn;
    protected TMP_Text cntText;
    protected Image hideImage;
    protected float coolTime = 10f;
    protected int cnt = 2;

    protected virtual void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonAction);

        cntText = GetComponentInChildren<TMP_Text>();
        hideImage = transform.Find("HideImage").GetComponent<Image>();
        hideImage.fillAmount = 0f;
    }

    protected abstract void ButtonAction();

    protected IEnumerator ButtonTimerCoroutine()
    {
        float elasedTime = 0f;

        if (cnt > 0)
        {
            while (elasedTime < coolTime)
            {
                if (Managers.Rule.isPause == false) // 정지 상태면 쿨타임 작동 안함
                {
                    elasedTime += Time.deltaTime;
                    hideImage.fillAmount = 1f - (elasedTime / coolTime);
                }
                yield return null;
            }

            hideImage.fillAmount = 0f;
        }
        else
        {
            hideImage.fillAmount = 1f;
        }
    }

    protected void PlayParticle(GameObject effect, Board board, Vector3Int position)
    {
        GameObject particle = Instantiate(effect, board.tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        Destroy(particle, 1f);
    }
}
