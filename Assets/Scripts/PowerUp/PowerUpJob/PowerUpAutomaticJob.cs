

using System.Collections.Generic;
using UnityEngine;

public class PowerUpAutomaticJob<S> : PowerUpJob
{
    public override bool IsRunning { get { return timer.IsRunning; } }
    public override void Update() { base.Update(); GameLoop.Update(timer); }

    public delegate bool SpawnPositionAllocator(out Vector3 position, out S args);  //returns true if allocation is successfull, otherwise false
    public delegate void SpawnPositionDeallocator(S args);

    private Timer timer;
    private Queue<PowerUp> waiting_power_ups;

    public PowerUpAutomaticJob(CreatePowerUpDelegate create_power_up, SpawnPositionAllocator allocate_position, SpawnPositionDeallocator deallocate_position, float time_interval) : base(create_power_up)
    {
        waiting_power_ups = new Queue<PowerUp>();
        timer = new Timer(0, 60, time_interval);
        timer.AddListner(delegate ()
        {
            Vector3 position;
            PowerUp power_up = GetPowerUpFromPool();
            S args;
            if (allocate_position(out position, out args))
            {
                power_up.OnInActive = (bool is_grabbed) => { deallocate_position(args); ReturnPowerUpToPool(power_up); };
                if (waiting_power_ups.Count > 0)
                {
                    #if DEBUG_MODE
                    GameManager.GetLogManager().LogWarning("Available");
#endif
                    waiting_power_ups.Dequeue().Activate(position);
                }
                else
                    power_up.Activate(position);
            }
            else
            {
                #if DEBUG_MODE
                GameManager.GetLogManager().LogWarning("Not available");
#endif
                waiting_power_ups.Enqueue(power_up);
            }

        }, OnTimer.Update);
        timer.IsLoop = true;
    }
    public override void Execute()
    {
        timer.Start();
    }
    public override void Stop()
    {
        timer.Pause();
    }
}