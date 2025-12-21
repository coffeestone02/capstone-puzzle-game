using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeOption : MonoBehaviour
{
    public GameObject optionPanel;
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
        if (optionPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            optionPanel.SetActive(false);
        }
    }

    public void OpneoptionPanel()
    {
        optionPanel.SetActive(true);
    }

    public void CloseoptionPanel()
    {
        optionPanel.SetActive(false);
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
