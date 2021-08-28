
using UnityEngine;

[CreateAssetMenu(fileName = "LivePowerUpSettings", menuName = "Custom/PowerUpSettings/LivePowerUpSettings")]
public class LivePowerUpSettings : PowerUpSettings
{ 
    public int num_lives_credit = 1;

    private void Awake()
    {
        name = GameConstants.PowerUpNames.HURT;
    }
}