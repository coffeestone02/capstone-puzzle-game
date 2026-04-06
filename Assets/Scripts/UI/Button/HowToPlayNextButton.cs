using UnityEngine;

public class HowToPlayNextButton : UIButton
{
    [SerializeField] private HowToPlayManager manager;

    protected override void ButtonAction()
    {
        manager.NextPage();
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}