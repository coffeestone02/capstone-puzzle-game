using UnityEngine;

public class MusicToggleSwitch : ToggleSwitch
{
    private const string BGM_KEY = "BGM_OFF";

    protected override void Start()
    {
        base.Start();

        onToggleOn -= BGMOff;
        onToggleOff -= BGMOn;

        onToggleOn += BGMOff;
        onToggleOff += BGMOn;

        ApplySavedSetting();
    }

    private void BGMOff()
    {
        Managers.Audio.TurnOffBGM();
        PlayerPrefs.SetInt(BGM_KEY, 1);
        PlayerPrefs.Save();
    }

    private void BGMOn()
    {
        Managers.Audio.TurnOnBGM();
        PlayerPrefs.SetInt(BGM_KEY, 0);
        PlayerPrefs.Save();
    }

    public void ApplySavedSetting()
    {
        bool isBGMOff = PlayerPrefs.GetInt(BGM_KEY, 0) == 1;

        SetVisual(isBGMOff);

        if (isBGMOff)
            Managers.Audio.TurnOffBGM();
        else
            Managers.Audio.TurnOnBGM();
    }
}