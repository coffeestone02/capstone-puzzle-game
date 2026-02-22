using UnityEngine;
using UnityEngine.UI;

public class PauseButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    private void Awake()
    {
        Managers.Input.pauseAction += Pause;
    }

    private void OnDisable()
    {
        Managers.Input.pauseAction -= Pause;
    }

    protected override void ButtonAction()
    {
        Pause();
    }

    public void Pause()
    {
        Managers.UI.ShowPopup("PausePopup");
        Managers.Rule.isPause = true;
        Time.timeScale = 0f;
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
