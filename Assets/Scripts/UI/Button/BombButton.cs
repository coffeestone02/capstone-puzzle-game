using UnityEngine;
using UnityEngine.Tilemaps;

public class BombButton : UIItemButton
{
    private GameObject particle;
    private Tile bombTile;

    protected override void Start()
    {
        base.Start();
        particle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
        bombTile = Resources.Load<Tile>("VisualAssets/Particles/BombTile");
    }

    protected override void ButtonAction()
    {
        if (Managers.Rule.isOver || Managers.Rule.isPause) return;

        Managers.Input.BlockInput(0.05f);

        if (cnt > 0 && hideImage.fillAmount <= 0f)
        {
            Piece piece = FindFirstObjectByType<Piece>();
            hideImage.fillAmount = 1f;
            SetCnt(cnt - 1);
            piece.ChangeBombTile();
            StartCoroutine(ButtonTimerCoroutine());
        }
    }
}
