
public class HexagonPlayer : Player
{
    protected override void OnLoadPlayerSettings()
    {
        GameManager.GetPlayerManager().LoadPlayerSettings<HexagonPlayerSettings>(GameConstants.PlayerNames.HEXAGON);

    }
    protected override void OnHealthZero()
    {
        base.OnHealthZero();
        if (LiveCount == 0)
            GameManager.GetPlayerManager().UnloadPlayerSettings<HexagonPlayerSettings>(GameConstants.PlayerNames.HEXAGON);
    }
}