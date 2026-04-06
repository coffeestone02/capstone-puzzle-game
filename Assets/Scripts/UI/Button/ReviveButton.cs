using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ReviveButton : MonoBehaviour
{
    private Button btn;
    private GameObject particle;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(ButtonAction);

        particle = Resources.Load<GameObject>("VisualAssets/Particles/BombParticle");
    }

    private void ButtonAction()
    {
        if (Managers.Rule.isOver == false) return;
        if (Managers.Rule.CanRevive == false) return;

        if (RewardAdManager.Instance == null)
        {
            return;
        }

        if (RewardAdManager.Instance.IsAdReady() == false)
        {
            return;
        }

        Managers.Input.BlockInput(0.05f);

        RewardAdManager.Instance.ShowRewardAd(() =>
        {
            // 광고 보상을 실제로 받은 뒤에만 부활 처리
            if (Managers.Rule.TryUseRevive() == false) return;

            Board board = GameObject.Find("MainBoard").GetComponent<Board>();
            Piece piece = board.GetComponent<Piece>();

            Revive(board, piece);

            Managers.Rule.CancelGameOverPending();
            Managers.Revive.ReviveSuccess();   // RevivePopup 닫기
            piece.SpawnPiece();                // 다시 시작
            board.GetComponent<PieceMover>().SetStepDirection();
        });
    }

    private void Revive(Board board, Piece piece)
    {
        Tilemap tilemap = board.tilemap;
        RectInt bounds = board.Bounds;

        bool isBroken = false;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                Tile tile = tilemap.GetTile<Tile>(pos);
                if (tile != null && board.IsCenterCell(pos) == false)
                {
                    isBroken = true;

                    if (particle != null)
                        PlayParticle(board, pos);

                    tilemap.SetTile(pos, null);
                }
            }
        }

        if (isBroken)
            Managers.Audio.PlaySFX("ExplodeSFX");
    }

    private void PlayParticle(Board board, Vector3Int position)
    {
        GameObject obj = Instantiate(
            particle,
            board.tilemap.GetCellCenterWorld(Vector3Int.FloorToInt(position)),
            Quaternion.identity
        );

        Destroy(obj, 1f);
    }
}