using UnityEngine;

public class AndroidControlSystem : IControlSystem
{
    public bool IsRunning { get; private set; }
    public IController AxisController { get; private set; }

    public bool DashButton { get; private set; }
    public Vector2 TouchLocation { get { return dashTouchEvent.touch.position - DeviceUtility.screen_size_half; } }

    private Vector2 joystickPos;
    private TouchEvent dashTouchEvent;
    private float sqr_joystick_body_radius;
    public AndroidControlSystem(JoystickSettings joyStickSettings, int dashTouchLayerId, int joystickTouchLayerId)
    {
        JoyStick joystick = JoyStick.CreateJoyStick(joyStickSettings, joystickTouchLayerId);
        IsRunning = true;
        AxisController = joystick;
        int joystick_body_radius;
        joystick_body_radius = (int)(joystick.Settings.BodySize.x * 0.5f);
        joystick_body_radius += 100;
        sqr_joystick_body_radius = joystick_body_radius * joystick_body_radius;
        DashButton = false;
        joystickPos = joystick.Settings.position;

        dashTouchEvent = new TouchEvent();
        dashTouchEvent.LayerID = dashTouchLayerId;
        dashTouchEvent.Condition = IsOutOfJoystickBody;
        dashTouchEvent.OnBegan = delegate () { DashButton = true;  };
        dashTouchEvent.OnEnded = delegate () { DashButton = false; };
        GameManager.GetTouchManager().OnInitialize += delegate
        {
            GameManager.GetTouchManager().RegisterEvent(dashTouchEvent);
        };
    }
    public void Dispose()
    {
        GameManager.GetTouchManager().UnregisterEvent(dashTouchEvent);
    }
    public void Update()
    {
        GameLoop.Update(AxisController);
    }
    private bool IsOutOfJoystickBody(Vector2 input_pos)
    {
        bool result = ((input_pos - joystickPos)).sqrMagnitude > sqr_joystick_body_radius;
        return result;
    }
}
