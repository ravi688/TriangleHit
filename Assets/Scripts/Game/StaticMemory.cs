#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

public static class StaticMemory
{
    //Usage Restricted: Dedicated to GameManager.SetManagementMode
    public static Action US_OnActiveSceneChanged { get; set; }
   
    public static BarMeter LivesMeter { get; set; }

    /*NOTE: 
    * GameManager.GetEconomyManager().StarCount is the total collected stars throughout all the levels, i.e. the data saved in the disk
    * GameManager.GetEconomyManager().CoinCount is the total collected coins throughout all the levels, i.e. the data saved in the disk
    * 
    * CollectedStars is the number of collected stars during the current game session i.e. gameplay level and it is not the sum of all the levels
    * CollectedCoins is the number of collected coins during the current game session i.e. gameplay level and it is not the sum of all the levels
     */
    public static int CollectedStarCount
    {
        get
        {
            return collected_star_count;
        }
        set
        {
            collected_star_count = value;
            GameLogic.OnCollectStar(collected_star_count);
        }
    }

    public static int CollectedCoinCount
    {
        get
        {
            return collected_coin_count;
        }
        set
        {
            collected_coin_count = value;
            GameLogic.OnCollectCoin(collected_coin_count);
        }
    }
    public static int DisposedCount
    {
        get
        {
            return disposed_count;
        }
        set
        {
            disposed_count = value;
            if (disposed_count == MaxEnemyCount)
            {
                GameLogic.OnAllEnemyDisposed();
                GameLogic.OnGameOver(disposed_count, disposed_count);
            }
            GameLogic.OnEnemyDisposed(disposed_count);
        }
    }

    public static int MaxEnemyCount
    {
        get
        {
            return max_enemy_count;
        }
        set
        {
            max_enemy_count = value;
            GameLogic.OnEnemyInstantitate(max_enemy_count);
        }
    }
    public static int EnemyCount
    {
        get
        {
            return enemy_count;
        }
        set
        {
            enemy_count = value;
            GameLogic.OnEnemyKilled(enemy_count);
            if (enemy_count == 0)
                GameLogic.OnAllEnemyKilled();
        }
    }

    public static int HighScore
    {
        get
        {
            return high_score;
        }
        set
        {
            high_score = value;
            GameLogic.OnUpdateHighScore(high_score);
        }
    }
    public static int CurrentScore
    {
        get
        {
            return current_score;
        }
        set
        {
            current_score = value;
            GameLogic.OnUpdateCurrentScore(current_score);
        }
    }
    public static int CurrentGameplayLevelBuildIndex
    {
        get{
            return current_gameplay_level_build_index;
        }
        set
        {
            current_gameplay_level_build_index = value;
            GameLogic.OnUpdateCurrentGameplayLevelBuildIndex(current_gameplay_level_build_index);
        }
    }
    public static int CurrentLevelBuildIndex
    {
        get
        {
            return current_level_build_index;
        }
        set
        {
            current_level_build_index = value;
            GameLogic.OnUpdateCurrentLevelBuildIndex(current_level_build_index);
        }
    }
    public static bool IsNextLevelUnLocked { get; set; }

    private static int max_enemy_count;
    private static int enemy_count;
    private static int disposed_count;
    private static int high_score;
    private static int current_score;
    private static int collected_star_count;
    private static int collected_coin_count;
    private static int current_level_build_index;
    private static int current_gameplay_level_build_index;
}