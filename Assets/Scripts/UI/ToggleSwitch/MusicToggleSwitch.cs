using UnityEngine;

public class MusicToggleSwitch : ToggleSwitch
{
    protected override void Start()
    {
        base.Start();

        onToggleOn -= MusicOff;
        onToggleOff -= MusicOn;

        onToggleOn += MusicOff;
        onToggleOff += MusicOn;
    }

    private void MusicOff()
    {
        Managers.Audio.TurnOffBGM();
    }

    private void MusicOn()
    {
        Managers.Audio.TurnOnBGM();
    }
}
