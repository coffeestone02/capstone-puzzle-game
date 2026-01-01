using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 플레이타임 및 점수 기록(구글 랭킹 리더보드에 쓰일 예정)
public class ScoreManager
{
    public int score
    {
        get => score;
        set => score = value;
    }
    public float playTime
    {
        get => playTime;
        set => playTime = value;
    }
    public float level { get; private set; }

}
