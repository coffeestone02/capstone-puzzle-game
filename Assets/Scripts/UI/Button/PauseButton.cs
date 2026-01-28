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
        GameObject parent = GameObject.Find("UICanvas");
        GameObject go = parent.transform.Find("PausePopup").gameObject;
        Managers.Rule.isPause = true;
        go.SetActive(true);
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
