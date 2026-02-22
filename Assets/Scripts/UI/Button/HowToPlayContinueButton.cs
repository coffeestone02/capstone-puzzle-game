using UnityEngine;

public class HowToPlayContinueButton : UIButton
{
    [SerializeField] private HowToPlayManager manager;

    protected override void ButtonAction()
    {
        manager.Close();
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}