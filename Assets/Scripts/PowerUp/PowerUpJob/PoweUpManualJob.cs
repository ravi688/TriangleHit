
using UnityEngine;

public class PowerUpManualJob : PowerUpJob
{
    private Vector3 spawn_position;
    public PowerUpManualJob(CreatePowerUpDelegate create_power_up) : base(create_power_up) { }

    public void SetSpawnPosition(Vector3 spawn_position)
    {
        this.spawn_position = spawn_position;
    }
    public override void Execute()
    {
        PowerUp power_up = GetPowerUpFromPool();
        power_up.OnInActive = (bool is_grabbed) => { ReturnPowerUpToPool(power_up); };
        power_up.Activate(spawn_position);
    }
    public override void Stop()
    {
        //Do Nothing
    }
}