using UnityEngine;

public class EffectToggleSwitch : ToggleSwitch
{
    private const string SFX_KEY = "SFX_OFF";

    protected override void Start()
    {
        base.Start();

        onToggleOn -= SFXOff;
        onToggleOff -= SFXOn;

        onToggleOn += SFXOff;
        onToggleOff += SFXOn;

        ApplySavedSetting();
    }

    private void SFXOff()
    {
        Managers.Audio.TurnOffSFX();
        PlayerPrefs.SetInt(SFX_KEY, 1);
        PlayerPrefs.Save();
    }

    private void SFXOn()
    {
        Managers.Audio.TurnOnSFX();
        PlayerPrefs.SetInt(SFX_KEY, 0);
        PlayerPrefs.Save();
    }

    public void ApplySavedSetting()
    {
        bool isSFXOff = PlayerPrefs.GetInt(SFX_KEY, 0) == 1;

        SetVisual(isSFXOff);

        if (isSFXOff)
            Managers.Audio.TurnOffSFX();
        else
            Managers.Audio.TurnOnSFX();
    }
}