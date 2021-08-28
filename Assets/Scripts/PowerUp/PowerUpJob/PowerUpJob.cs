using System.Collections.Generic;

public abstract class PowerUpJob : IManualUpdatable
{
    public virtual bool IsRunning {  get { return active_power_up_count != 0; } }
    private int active_power_up_count = 0;
    public virtual void Update()
    {
        for(int i = 0; i < active_power_up_count; i++)
            GameLoop.Update(active_power_ups[i]);
    }

    public delegate PowerUp CreatePowerUpDelegate();

    private CreatePowerUpDelegate create_power_up;
    private Queue<PowerUp> inactive_power_ups;
    private List<PowerUp> active_power_ups;
    public PowerUpJob(CreatePowerUpDelegate create_power_up)
    {
        this.create_power_up = create_power_up;
        active_power_ups = new List<PowerUp>();
        inactive_power_ups = new Queue<PowerUp>();
    }
    protected void ReturnPowerUpToPool(PowerUp power_up)
    {
        inactive_power_ups.Enqueue(power_up);
        active_power_up_count--;
    }
    protected PowerUp GetPowerUpFromPool()
    {
        PowerUp power_up;

        if (inactive_power_ups.Count > 0)
            power_up = inactive_power_ups.Dequeue();
        else
            power_up = create_power_up();
        active_power_ups.Add(power_up);
        active_power_up_count++;
        return power_up;
    }
    public abstract void Execute();
    public abstract void Stop();
}