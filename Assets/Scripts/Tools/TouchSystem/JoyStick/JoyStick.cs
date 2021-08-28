using UnityEngine;
using UnityEngine.UI;
public enum HandConfiugartion
{
    LeftHanded,
    RightHanded
}


public class JoyStick : IController
{
    public bool IsRunning { get; set; }
    public Vector2 Position
    {
        get
        {
            return Body.localPosition;
        }
        set
        {
            Handle.localPosition = value;
            Body.localPosition = value;
        }
    }
    public JoystickSettings Settings { get; private set; }
    public Vector2 Axis { get { return GetAxis(); } set { } }

    public RectTransform Handle { get; set; }
    public RectTransform Body { get; set; }
    Vector2 InitialHandlePos;
    Vector2 RadiusVector;
    Vector2 InputPos;

    float HandleRadius;
    TouchEvent touchEvent;
    private bool return_to_original_position =false;

    public static JoyStick CreateJoyStick(JoystickSettings Settings, int touchLayerID = 0)
    {
        #region Joystick Runtime Instantiation
        GameObject joystick = new GameObject("Joystick");
        GameObject handle = new GameObject("Handle");
        handle.AddComponent<Image>().sprite = Settings.HandleSprite;
        GameObject body = new GameObject("Body");
        body.AddComponent<Image>().sprite = Settings.BodySprite;
        RectTransform HandleRectTr = handle.GetComponent<RectTransform>();
        RectTransform BodyRectTr = body.GetComponent<RectTransform>();
        HandleRectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Settings.HandleSize.x);
        HandleRectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Settings.HandleSize.y);
        BodyRectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Settings.BodySize.x);
        BodyRectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Settings.BodySize.y);

        Canvas canvas = null;
        if ((canvas = GameObject.FindObjectOfType<Canvas>()) == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("To Use Joystick you must have at least one Canvas,<bold> Otherwise the Joystick will be a NULL object</bold>");
#endif
            return null;
        }
        joystick.transform.SetParent(canvas.transform);
        joystick.transform.localPosition = Vector2.zero;
        HandleRectTr.SetParent(joystick.transform);
        BodyRectTr.SetParent(joystick.transform);

        Vector2 position = Vector2.zero;
        Settings.GetCalculatedPosition(ref position);
        HandleRectTr.localPosition = position;
        BodyRectTr.localPosition = position;
        //This is To be Remembered that 
        //the position of the joystick must be the position saved
        //that is SettingsManager.CurrentSettings.position

        #endregion
        return new JoyStick(HandleRectTr, BodyRectTr, Settings, touchLayerID);
    }
    private JoyStick() { }
    private JoyStick(RectTransform Handle, RectTransform Body, JoystickSettings settings, int touchLayerID = 0)
    {
        touchEvent = new TouchEvent();
        touchEvent.LayerID = touchLayerID;
        touchEvent.Condition = IsInsideOfHandle;
        touchEvent.OnBegan = delegate () { return_to_original_position = false; };
        touchEvent.OnMoved = SyncHandleWithTouch;
        touchEvent.OnEnded = delegate() { return_to_original_position = true; };
        GameManager.GetTouchManager().OnInitialize += delegate
        {
            GameManager.GetTouchManager().RegisterEvent(touchEvent);
        };

        Settings = settings;
        this.Handle = Handle;
        this.Body = Body;
        HandleRadius = settings.HandleSize.x * 0.5f;
        InitialHandlePos = Handle.localPosition;
        IsRunning = true;
    }
    public void Dispose()
    {
        GameManager.GetTouchManager().UnregisterEvent(touchEvent);
    }
    public void Update()
    {
        if (return_to_original_position && Settings.IsInteractive)
            ReturnHandleToOriginalPosition();
    }
    private void SyncHandleWithTouch()
    {
        if (!Settings.IsInteractive) return;
        InputPos = touchEvent.touch.position - DeviceUtility.screen_size * 0.5f;
        RadiusVector = InputPos - InitialHandlePos;  //initial handle pos has origin at the center of the screen
        if (RadiusVector.sqrMagnitude > Settings.BodySize.x * Settings.BodySize.x * 0.25f)
            Handle.localPosition = RadiusVector.normalized * Settings.BodySize.x * 0.5f + InitialHandlePos;
        else
            Handle.localPosition = InputPos;
    }
    private void ReturnHandleToOriginalPosition()
    {
        Handle.localPosition = Vector2.Lerp(Handle.localPosition, InitialHandlePos, Time.deltaTime * Settings.HandleReturnSpeed);
        if (((Vector2)Handle.localPosition - InitialHandlePos).sqrMagnitude <= 0.01f)
            return_to_original_position = false;
    }
    private bool IsInsideOfHandle(Vector2 input_pos)
    {
        RadiusVector = input_pos - InitialHandlePos;
        bool result = RadiusVector.sqrMagnitude <= HandleRadius * HandleRadius;

        return result;
    }
    private Vector2 GetAxis()
    {
        if (((Vector2)Handle.localPosition - InitialHandlePos).sqrMagnitude < Settings.Threshold_distance * Settings.Threshold_distance)
            return Vector2.zero;
        else
            return ((Vector2)Handle.localPosition - InitialHandlePos) * Settings.Sensitivity / (Settings.BodySize.x * 0.5f);
    }
}