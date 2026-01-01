using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 클래스
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // Sound
    public AudioSource bgmPlayer;
    public AudioSource sfxPlayer;

    public AudioClip[] bgmClips;
    public AudioClip soundRotate;          // 회전 성공
    public AudioClip soundClear;           // 블록 파괴(클리어)
    public AudioClip soundButton;
    public AudioClip soundLock;
    public AudioClip soundBomb;
    public AudioClip soundMove;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void TurnOnBGM()
    {
        bgmPlayer.volume = 0.6f;
    }

    public void TurnOffBGM()
    {
        bgmPlayer.volume = 0f;
    }

    public void TurnOnSFX()
    {
        sfxPlayer.volume = 0.7f;
    }

    public void TurnOffSFX()
    {
        sfxPlayer.volume = 0f;
    }

    public void PlayRotateSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundRotate);
    }

    public void PlayClearSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundClear);
    }

    public void PlayButtonSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundButton);
    }

    public void PlayLockSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundLock);
    }

    public void PlayBombSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundBomb);
    }

    public void PlayMoveSound()
    {
        Instance.sfxPlayer.PlayOneShot(soundMove);
    }

    public void PlayBgm(int level)
    {
        if (level == 0)
        {
            Debug.LogError("레벨은 0이 될 수 없습니다");
            return;
        }

        if (Instance.bgmPlayer.clip != Instance.bgmClips[level - 1])
        {
            Instance.bgmPlayer.clip = Instance.bgmClips[level - 1];
            bgmPlayer.Play();
        }
    }
}
