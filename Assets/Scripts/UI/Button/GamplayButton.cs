using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamplayButton : UIButton
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        SceneManager.LoadScene("GamePlayScene");
        Managers.Audio.PlaySFX("ButtonSFX");
    }
}
