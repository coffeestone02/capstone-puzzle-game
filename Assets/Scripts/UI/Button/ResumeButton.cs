using UnityEngine;
using UnityEngine.UI;

public class ResumeButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        Managers.Rule.isPause = false;
        Managers.UI.ClosePopup("PausePopup");
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
