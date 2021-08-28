

public class SquarePlayer : Player
{
    protected override void OnLoadPlayerSettings()
    {
        GameManager.GetPlayerManager().LoadPlayerSettings<SquarePlayerSettings>(GameConstants.PlayerNames.SQUARE);

    }
    protected override void OnHealthZero()
    {
        base.OnHealthZero();
        if (LiveCount == 0)
            GameManager.GetPlayerManager().UnloadPlayerSettings<SquarePlayerSettings>(GameConstants.PlayerNames.SQUARE);
    }
}