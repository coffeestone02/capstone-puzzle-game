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
        GameObject go = GameObject.Find("PausePopup");
        Managers.Rule.isPause = false;
        go.SetActive(false);
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
