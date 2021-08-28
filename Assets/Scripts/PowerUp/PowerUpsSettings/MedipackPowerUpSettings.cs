
using UnityEngine;

[CreateAssetMenu(fileName = "MedipackPowerUpSettings", menuName = "Custom/PowerUpSettings/MedipackPowerUpSettings")]
public class MedipackPowerUpSettings : PowerUpSettings
{
    public float health_credit = 10.0f;

    private void Awake()
    {
        name = GameConstants.PowerUpNames.MEDIPACK;
    }
}