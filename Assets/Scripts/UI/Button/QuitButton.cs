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
        var saver = Object.FindFirstObjectByType<SaveController>();
        if (saver != null) saver.SaveNow();

        Managers.Audio.PlaySFX("ButtonSFX");
        SceneManager.LoadScene("TitleScene");
        Managers.Reset();
        
    }
}
