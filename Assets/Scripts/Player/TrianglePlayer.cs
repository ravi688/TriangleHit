

using UnityEngine;

public class TrianglePlayer : Player
{
    protected override void OnLoadPlayerSettings()
    {
        GameManager.GetPlayerManager().LoadPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);
        player_settings = GameManager.GetPlayerManager().GetPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);

    }
    protected override void OnHealthZero()
    {
        base.OnHealthZero();
        if (LiveCount == 0)
            GameManager.GetPlayerManager().UnloadPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);
    }
}