
using UnityEngine;

public class WindowsControlSystem : IControlSystem
{
    public bool IsRunning { get; private set; }
    public IController AxisController { get; private set; }
    public bool DashButton { get; private set; }
    public Vector2 TouchLocation { get; private set; }
    public WindowsControlSystem()
    {
        AxisController = new WindowsCardinalController() as IController;
        IsRunning = true;
    }
    public void Update()
    {
        if (AxisController.IsRunning)
            AxisController.Update();
        DashButton = Input.GetKey(KeyCode.Y);
        TouchLocation = (Vector2)Input.mousePosition - DeviceUtility.screen_size * 0.5f;
    }

}