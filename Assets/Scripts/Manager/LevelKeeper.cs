using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 레벨 조절용 클래스
/// </summary>
public class LevelKeeper
{
    public int level { get; private set; } = 1;

    public void Init()
    {
        SetLevel(0);
    }

    /// <summary>
    /// 점수에 따라 레벨을 조정
    /// </summary>
    /// <param name="score"></param>
    public void SetLevel(int score)
    {
        AudioClip clip = Managers.Audio.bgmPlayer.clip;

        if (score < 1000)
        {
            // 장애물 세팅
            Managers.Rule.obstacleSpawnLimit = 12;
            level = 1;
            if (clip == null || clip.name != "LV1")
            {
                Managers.Rule.obstacleCount = 0;
                Managers.Audio.PlayBGM(level);
            }
        }
        else if (score < 5000)
        {
            Managers.Rule.obstacleSpawnLimit = 10;
            level = 2;
            if (clip == null || clip.name != "LV2")
            {
                Managers.Rule.obstacleCount = 0;
                Managers.Audio.PlayBGM(level);
            }
        }
        else if (score < 10000)
        {
            Managers.Rule.obstacleSpawnLimit = 8;
            level = 3;
            if (clip == null || clip.name != "LV3")
            {
                Managers.Rule.obstacleCount = 0;
                Managers.Audio.PlayBGM(level);
            }
        }
        else if (score < 150000)
        {
            Managers.Rule.obstacleSpawnLimit = 6;
            level = 4;
            if (clip == null || clip.name != "LV4")
            {
                Managers.Rule.obstacleCount = 0;
                Managers.Audio.PlayBGM(level);
            }
        }
        else
        {
            Managers.Rule.obstacleSpawnLimit = 5;
            level = 5;
            if (clip == null || clip.name != "LV5")
            {
                Managers.Rule.obstacleCount = 0;
                Managers.Audio.PlayBGM(level);
            }
        }
    }
}
