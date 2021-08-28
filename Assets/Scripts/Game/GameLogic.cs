using UnityEngine;

public class GameLogic
{
    public static void OnPurchase(string purchase_id)
    {
#if DEBUG_MODE
        string[] tokens = purchase_id.Split('-');
        string log = string.Format("Purchased: {0} {1} Cost: {2}", tokens[2], tokens[3], int.Parse(tokens[1]));
        GameManager.GetLogManager().Log(log);
#endif
    }
    //star_count is the number of collected stars during the current gameplay level, it is not the sum of all the levels
    public static void OnCollectStar(int star_count)
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Collected Stars: " + star_count);
#endif
    }

    //star_count is the number of collected stars during the current gameplay level, it is not the sum of all the levels
    public static void OnCollectCoin(int coin_count)
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Collected Coins: " + coin_count);
#endif
    }

    public static void OnUpdateCurrentScore(int score)
    {
        GameManager.GetHUDManager().SetScore(score);
    }
    //Callback when StaticMemory.CurrentLevelBuildIndex is modified
    //StaticMemory.CurrentLevelBuildIndex is useful to preserve the selected level build index in the level menu
    public static void OnUpdateCurrentLevelBuildIndex(int build_index)
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("StaticMemory.CurrentLevelBuildIndex is set to: " + build_index);
#endif

    }
    public static void OnUpdateCurrentGameplayLevelBuildIndex(int build_index)
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("StaticMemory.CurrentGameplayLevelBuildIndex is set to: " + build_index);
#endif
    }
    //Callback when someone has just given a command to change the level
    public static void OnPreLoadLevel(int build_index)
    {
        if (build_index == GameConstants.LevelBuildIndices.START_SCREEN)
            GameManager.SetGameManagementMode(GameManagementMode.GameStart);
        else if (build_index == GameConstants.LevelBuildIndices.STATS_SCREEN)
            GameManager.SetGameManagementMode(GameManagementMode.GameEnd);
        else
            GameManager.SetGameManagementMode(GameManagementMode.GamePlay);
    }
    //Callback when we have hidden the current scene and then begin loading the next scene
    public static void OnPostLoadLevel(int build_index)
    {

    }
    //Callback when we are in the new level loaded level, after all the awake functions have been called
    public static void OnPostAwakeLoadLevel(int build_index)
    {
        if ((build_index != GameConstants.LevelBuildIndices.START_SCREEN) && (build_index != GameConstants.LevelBuildIndices.STATS_SCREEN))
        {
            //Reset all the non-persitent game session data 
            StaticMemory.CurrentScore = 0;
            StaticMemory.DisposedCount = 0;
            StaticMemory.EnemyCount = 0;
            StaticMemory.CollectedCoinCount = 0;
            StaticMemory.CollectedStarCount = 0;
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Non-Persistent Game Session Data Reset Successful");
#endif
        }
    }

    public static void OnUpdateHighScore(int new_high_score) { }
    public static void OnEnemyInstantitate(int max_enemy_count) { }
    public static void OnEnemyDisposed(int enemy_disposed_count)
    {
        GameManager.GetHUDManager().SetEnemyDesposeMeterValue(enemy_disposed_count);
    }
    public static void OnEnemyKilled(int enemy_killed_count) { }
    public static void OnAllEnemyKilled() { }
    public static void OnGameOver(int enemy_count, int enemy_disposed)
    {
        if (enemy_count > enemy_disposed)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("You Loose!");
#endif
            GameManager.GetHUDManager().ShowLoose();
        }
        else
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("You Win!");
#endif
            GameManager.GetHUDManager().ShowWin();
        }

#if DEBUG_MODE
        GameManager.GetLogManager().Log("Enemy Count: " + enemy_count); 
        GameManager.GetLogManager().Log("Enemy Disposed: " + enemy_disposed);
#endif
        GameManager.GetStatsScreenTimer().Start();
    }
    public static void OnAllEnemyDisposed() { }
    public static void OnPlayerHealthFull() { }
    public static void OnPlayerLivesFull() { }
    public static void OnUpdatePlayerLiveCount(int live_count)
    {
        Debug.Log("Live Count: " + live_count);
        GameManager.GetHUDManager().SetPlayerLivesMeterValue(live_count);
    }
    public static void OnPlayerAlive()
    {
        PowerUp.target = PlayerCore.instance.GetComponent<Transform>();
#if DEBUG_MODE
        GameManager.GetLogManager().Log("PowerUp::target is set to Player::instance");
#endif
        GameManager.GetCameraController().Follow(GameManager.GetAgainAliveManager().GetCurrent().GetComponent<Transform>());
        GameManager.GetCameraController().ZoomIn();
    }
    public static void OnPlayerKilled(int live_count)
    {
        if (live_count > 0)
            GameManager.GetAgainAliveManager().BeginRespawn();
        GameManager.GetCameraController().ZoomOut();
        GameManager.GetCameraController().StopFollow();
    }
    public static void OnUpdateThrust(float amount)
    {
        GameManager.GetHUDManager().SetPlayerDashValue(amount);
    }
    public static void OnThrustExhausted() { }
    public static void OnThrustFull() { }
    public static void OnUpdatePlayerHealth(float health)
    {
        GameManager.GetHUDManager().SetPlayerHealth(health);
    }

    public static class CostFunctions
    {

        //Cost Function: used by Upgrade Objects
        public static int CalculateDurationUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(100 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
        public static int CalculateFrequencyUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(70 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }

        public static int CalculateHealthCreditUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(120 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
        public static int CalculateLiveCreditUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(70 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
        public static int CalculateStarCreditUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(50 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
        public static int CalculateCoinCreditUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(50 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
        public static int CalculateShieldDurationUpgradeCost(int upgraded_steps, int max_upgrade_steps)
        {
            return (int)(50 * (float)(upgraded_steps + 1) / max_upgrade_steps);
        }
    }
}