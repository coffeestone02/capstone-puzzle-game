using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager
{
    private AudioSource bgmPlayer;
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();

    private AudioSource sfxPlayer;
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

            LoadBGM();
            LoadSFX();
        }
    }

    private void LoadBGM()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in clips)
        {
            bgmClips.Add(clip.name, clip);
        }
    }

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
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayBGM(string name, bool isLoop = true)
    {

    }

    public void PlayBGM(int level)
    {

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
}
