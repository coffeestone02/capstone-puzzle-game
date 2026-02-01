using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 점수 관리 클래스
/// </summary>
public class ScoreManager
{
    public float playtime { get; private set; }
    public int score { get; private set; }
    public int combo { get; private set; }
    private LevelKeeper levelKeeper = new LevelKeeper();

    public void ApplyLoadedState(float loadedPlaytime, int loadedScore, int loadedCombo)
    {
        playtime = loadedPlaytime;
        score = loadedScore;
        combo = loadedCombo;

        //로드시 재계산
        levelKeeper.SetLevel(score);
    }

    public void Init()
    {
        levelKeeper.Init();
        playtime = 0;
        score = 0;
        combo = 0;
    }

    public void OnUpdate()
    {
        // 게임오버거나 퍼즈 상태가 아니면 실행
        playtime += Time.deltaTime;
    }

    /// <summary>
    /// 점수를 계산하고 저장함
    /// </summary>
    /// <param name="mainMatchedCount">메인 매칭 개수</param>
    /// <param name="bonusMatchedCount">추가 매칭 개수</param>
    /// <param name="itemMatchedCount">아이템 매칭 개수</param>
    public void SetScore(int mainMatchedCount, int bonusMatchedCount, int itemMatchedCount)
    {
        if (mainMatchedCount > 0) combo++;
        else combo = 0;

        int mainPoint = mainMatchedCount * 100;
        int bonusPoint = bonusMatchedCount * 60;
        score += (mainPoint + bonusPoint) * (0 + combo) * (int)(1 + 0.1 * levelKeeper.level);

        int itemPoint = itemMatchedCount * 60;
        score += itemPoint;
        levelKeeper.SetLevel(score);
    }

    public int GetLevel()
    {
        return levelKeeper.level;
    }
}
