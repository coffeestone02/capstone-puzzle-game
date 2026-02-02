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
    private GameObject bombParticle;

    protected override void Start()
    {
        base.Start();
        cntText = GetComponentInChildren<TMP_Text>();
        hideImage = transform.Find("HideImage").GetComponent<Image>();
        hideImage.fillAmount = 0f;
        bombParticle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
    }

    protected override void ButtonAction()
    {
        if (Managers.Rule.isOver || Managers.Rule.isPause) return;

        if (cnt > 0 && hideImage.fillAmount <= 0f && AllDestroy())
        {
            hideImage.fillAmount = 1f;
            cnt--;
            cntText.text = cnt.ToString();
            Managers.Audio.PlaySFX("ExplodeSFX");
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
                    PlayParticle(bombParticle, board, pos); // 파티클 재생
                    tilemap.SetTile(pos, null);
                }
            }
        }

        if (isBroken)
        {
            Managers.Audio.PlaySFX("ExplodeSFX"); // 소리 재생
        }

        return isBroken;
    }

    private void PlayParticle(GameObject effect, Board board, Vector3Int position)
    {
        GameObject particle = Instantiate(effect, board.tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)), Quaternion.identity);
        Destroy(particle, 1f);
    }

}
