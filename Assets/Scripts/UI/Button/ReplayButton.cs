using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        Managers.Audio.PlaySFX("ButtonSFX");
        Managers.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
