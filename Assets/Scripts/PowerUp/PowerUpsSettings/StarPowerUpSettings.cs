
using UnityEngine;

[CreateAssetMenu(fileName = "StarPowerUpSettings", menuName = "Custom/PowerUpSettings/StarPowerUpSettings")]
public class StarPowerUpSettings : PowerUpSettings
{
    public int num_stars_credit = 1;
    private void Awake()
    {
        name = GameConstants.PowerUpNames.STAR;
    }
}