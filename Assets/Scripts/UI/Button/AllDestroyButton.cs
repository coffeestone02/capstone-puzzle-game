using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AllDestroyButton : UIButton
{
    private TMP_Text cntText;
    private Image hideImage;
    private float coolTime = 10f;
    private int cnt = 2;

    protected override void Start()
    {
        base.Start();
        cntText = GetComponentInChildren<TMP_Text>();
        hideImage = transform.Find("HideImage").GetComponent<Image>();
        hideImage.fillAmount = 0f;
    }

    protected override void ButtonAction()
    {
        if (Managers.Rule.isOver || Managers.Rule.isPause) return;

        if (cnt > 0 && hideImage.fillAmount <= 0f && AllDestroy())
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

    private bool AllDestroy()
    {
        bool isBroken = false;
        Board board = GameObject.Find("MainBoard").GetComponent<Board>();
        Tilemap tilemap = board.tilemap;
        Piece piece = board.GetComponent<Piece>();
        RectInt bounds = board.Bounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                Tile tile = tilemap.GetTile<Tile>(pos);
                if (tile != null && board.IsCenterCell(pos) == false)
                {
                    isBroken = true;
                    // 파티클 재생
                    tilemap.SetTile(pos, null);
                }
            }
        }

        if (isBroken)
        {
            // 소리 재생
        }

        return isBroken;
    }

}
