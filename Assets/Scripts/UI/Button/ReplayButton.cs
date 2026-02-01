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
        SaveSystem.Clear();
        Managers.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
