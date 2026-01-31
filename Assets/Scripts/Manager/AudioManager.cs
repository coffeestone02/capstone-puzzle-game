using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager
{
    public AudioSource bgmPlayer { get; private set; }
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();

    public AudioSource sfxPlayer { get; private set; }
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("AudioManager");

        if (root == null)
        {
            root = new GameObject { name = "AudioManager" };
            Object.DontDestroyOnLoad(root);

            GameObject bgmGO = new GameObject { name = "bgmPlayer" };
            bgmPlayer = bgmGO.AddComponent<AudioSource>();
            bgmGO.transform.parent = root.transform;

            GameObject sfxGO = new GameObject { name = "sfxPlayer" };
            sfxPlayer = sfxGO.AddComponent<AudioSource>();
            sfxGO.transform.parent = root.transform;

            bgmPlayer.volume = 0.45f;

            LoadBGM();
            LoadSFX();
        }
        else
        {
            bgmPlayer.clip = sfxPlayer.clip = null;
        }
    }

    /// <summary>
    /// Resources/Sounds/BGM 경로의 오디오 클립을 모두 로드
    /// </summary>
    private void LoadBGM()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in clips)
        {
            bgmClips.Add(clip.name, clip);
        }
    }

    /// <summary>
    /// Resources/Sounds/SFX 경로의 오디오 클립을 모두 로드
    /// </summary>
    private void LoadSFX()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (AudioClip clip in clips)
        {
            sfxClips.Add(clip.name, clip);
        }
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="name">클립 이름</param>
    /// <param name="isLoop"></param>
    public void PlayBGM(string name, bool isLoop = true)
    {
        bgmPlayer.loop = isLoop;
        bgmPlayer.clip = bgmClips[name];
        bgmPlayer.Play();
    }

    /// <summary>
    /// 배경음악 재생
    /// </summary>
    /// <param name="level"></param>
    /// <param name="isLoop"></param>
    public void PlayBGM(int level, bool isLoop = true)
    {
        bgmPlayer.loop = isLoop;

        switch (level)
        {
            case 1:
                bgmPlayer.clip = bgmClips["LV1"];
                break;
            case 2:
                bgmPlayer.clip = bgmClips["LV2"];
                break;
            case 3:
                bgmPlayer.clip = bgmClips["LV3"];
                break;
            case 4:
                bgmPlayer.clip = bgmClips["LV4"];
                break;
            case 5:
                bgmPlayer.clip = bgmClips["LV5"];
                break;
        }

        bgmPlayer.Play();
    }

    /// <summary>
    /// 효과음 재생
    /// </summary>
    /// <param name="name">클립 이름</param>
    public void PlaySFX(string name)
    {
        if (sfxClips[name] == null)
        {
            Debug.LogError($"{name} sfx 클립이 존재하지 않음");
            return;
        }

        sfxPlayer.PlayOneShot(sfxClips[name]);
    }

    public void TurnOffBGM()
    {
        bgmPlayer.volume = 0f;
    }

    public void TurnOffSFX()
    {
        sfxPlayer.volume = 0f;
    }

    public void TurnOnBGM()
    {
        bgmPlayer.volume = 0.45f;
    }

    public void TurnOnSFX()
    {
        sfxPlayer.volume = 1f;
    }
}
