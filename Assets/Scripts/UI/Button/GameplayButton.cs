using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayButton : UIButton
{
    private const string HOW_TO_PLAY_KEY = "HOW_TO_PLAY_SHOWN";

    [SerializeField] private HowToPlayManager howToPlayManager;

    protected override void Start()
    {
        base.Start();
    }

    protected override void ButtonAction()
    {
        if (PlayerPrefs.GetInt(HOW_TO_PLAY_KEY, 0) == 0)
        {
            howToPlayManager.OpenFromGameplay();
        }
        else
        {
            SceneManager.LoadScene("GamePlayScene");
        }

        Managers.Audio.PlaySFX("ButtonSFX");
    }
}