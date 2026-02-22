using UnityEngine;

public class HowToPlayButton : UIButton
{
    [SerializeField] private HowToPlayManager manager;

    protected override void ButtonAction()
    {
        manager.Open();
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}