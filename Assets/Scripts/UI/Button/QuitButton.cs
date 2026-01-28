using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        SceneManager.LoadScene("TitleScene");
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
