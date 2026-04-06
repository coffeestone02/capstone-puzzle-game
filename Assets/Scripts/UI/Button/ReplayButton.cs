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

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        ApplySettings();
    }

    private void ApplySettings()
    {
        var ghost = FindObjectOfType<GhostToggleSwitch>();
        if (ghost != null)
            ghost.ApplySavedSetting();

        var sfx = FindObjectOfType<EffectToggleSwitch>();
        if (sfx != null)
            sfx.ApplySavedSetting();

        var bgm = FindObjectOfType<MusicToggleSwitch>();
        if (bgm != null)
            bgm.ApplySavedSetting();
    }
}
