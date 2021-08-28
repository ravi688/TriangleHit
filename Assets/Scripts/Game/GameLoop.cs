
using UnityEngine;


public class GameLoop
{
    private GameLoop() { }
    public static void Update(IManualUpdatable updatable)
    {

        if (updatable == null)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogError("IManualUpdatable is null");
#endif
            return;
        }

        if (updatable.IsRunning)
            updatable.Update();
    }
}