
using UnityEngine;

[CreateAssetMenu(fileName = "CoinPowerUpSettings", menuName = "Custom/PowerUpSettings/CoinPowerUpSettings")]
public class CoinPowerUpSettings : PowerUpSettings
{
    public int num_coins_credit = 1;
    private void Awake()
    {
        name = GameConstants.PowerUpNames.COIN;
    }
}