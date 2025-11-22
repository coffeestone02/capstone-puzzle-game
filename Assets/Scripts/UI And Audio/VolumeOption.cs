using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeOption : MonoBehaviour
{
    public GameObject OptionPanel;
    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;
    private float volumeValue = 0f;

    private void Start()
    {
        bgmPlayer = AudioManager.instance.bgmPlayer;
        sfxPlayer = AudioManager.instance.sfxPlayer;
    }

    private void Update()
    {
        if (OptionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OptionPanel.SetActive(false);
        }
    }

    public void OpneOptionPanel()
    {
        OptionPanel.SetActive(true);
    }

    public void CloseOptionPanel()
    {
        OptionPanel.SetActive(false);
    }

    public void SetMusicVolume(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void SetEffectVolume(float volume)
    {
        sfxPlayer.volume = volume;
    }
}
