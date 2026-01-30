using UnityEngine;
using UnityEngine.UI;

public class PauseButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        Managers.UI.ShowPopup("PausePopup");
        Managers.Rule.isPause = true;
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
