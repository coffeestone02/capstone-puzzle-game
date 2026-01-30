using UnityEngine;

public class EffectToggleSwitch : ToggleSwitch
{
    protected override void Start()
    {
        base.Start();

        onToggleOn -= SFXOff;
        onToggleOff -= SFXOn;

        onToggleOn += SFXOff;
        onToggleOff += SFXOn;
    }

    private void SFXOff()
    {
        Managers.Audio.TurnOffSFX();
    }

    private void SFXOn()
    {
        Managers.Audio.TurnOnSFX();
    }
}
