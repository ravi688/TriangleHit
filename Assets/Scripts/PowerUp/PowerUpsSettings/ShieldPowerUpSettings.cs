using UnityEngine;

[CreateAssetMenu(fileName = "ShieldPowerUpSettings", menuName = "Custom/PowerUpSettings/ShieldPowerUpSettings")]
public class ShieldPowerUpSettings : PowerUpSettings
{
    public float shield_duration = 10; //seconds

    private void Awake()
    {
        name = GameConstants.PowerUpNames.SHIELD;
    }
}