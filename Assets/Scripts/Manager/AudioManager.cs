using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PlayRotateSound()
    {
        instance.sfxPlayer.PlayOneShot(soundRotate);
    }

    public void PlayClearSound()
    {
        instance.sfxPlayer.PlayOneShot(soundClear);
    }

    public void PlayButtonSound()
    {
        instance.sfxPlayer.PlayOneShot(soundButton);
    }

    public void PlayLockSound()
    {
        instance.sfxPlayer.PlayOneShot(soundLock);
    }

    public void PlayBombSound()
    {
        instance.sfxPlayer.PlayOneShot(soundBomb);
    }

    public void PlayMoveSound()
    {
        instance.sfxPlayer.PlayOneShot(soundMove);
    }

    public void PlayBgm(int level)
    {
        if (level == 0)
        {
            Debug.LogError("레벨은 0이 될 수 없습니다");
            return;
        }

        if (instance.bgmPlayer.clip != instance.bgmClips[level - 1])
        {
            instance.bgmPlayer.clip = instance.bgmClips[level - 1];
            bgmPlayer.Play();
        }
    }
}
