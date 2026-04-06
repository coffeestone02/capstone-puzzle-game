using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayCloseButton : UIButton
{
    private const string HOW_TO_PLAY_KEY = "HOW_TO_PLAY_SHOWN";

    [SerializeField] private HowToPlayManager manager;

    protected override void ButtonAction()
    {
        if (manager.IsOpenedFromGameplay())
        {
            PlayerPrefs.SetInt(HOW_TO_PLAY_KEY, 1);
            PlayerPrefs.Save();

            manager.Close();
            Managers.Audio.PlaySFX("ButtonSFX");
            SceneManager.LoadScene("GamePlayScene");
        }
        else
        {
            manager.Close();
            Managers.Audio.PlaySFX("ButtonSFX");
        }
    }
}