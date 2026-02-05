using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AllDestroyButton : UIItemButton
{
    private GameObject particle;

    protected override void Start()
    {
        base.Start();
        particle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
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

    private bool AllDestroy()
    {
        bool isBroken = false;
        Board board = GameObject.Find("MainBoard").GetComponent<Board>();
        Piece piece = board.GetComponent<Piece>();
        Tilemap tilemap = board.tilemap;
        RectInt bounds = board.Bounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                Tile tile = tilemap.GetTile<Tile>(pos);
                if (tile != null && board.IsCenterCell(pos) == false && board.IsActivePiece(pos, piece) == false)
                {
                    isBroken = true;
                    PlayParticle(particle, board, pos);
                    tilemap.SetTile(pos, null);
                }
            }
        }

        if (isBroken)
        {
            Managers.Audio.PlaySFX("ExplodeSFX");
        }

        return isBroken;
    }
}
