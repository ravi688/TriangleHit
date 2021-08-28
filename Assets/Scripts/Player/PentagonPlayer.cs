

public class PentagonPlayer : Player
{
    protected override void OnLoadPlayerSettings()
    {
        GameManager.GetPlayerManager().LoadPlayerSettings<PentagonPlayerSettings>(GameConstants.PlayerNames.PENTAGON);

    }
    protected override void OnHealthZero()
    {
        base.OnHealthZero();
        if (LiveCount == 0)
            GameManager.GetPlayerManager().UnloadPlayerSettings<PentagonPlayerSettings>(GameConstants.PlayerNames.PENTAGON);
    }
}