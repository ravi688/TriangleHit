

public class Shield : PowerUp<IShieldable>
{
    private float shield_duration;
    public Shield(ShieldPowerUpSettings settings) : base(settings) { shield_duration = settings.shield_duration; }

    protected override void OnMissed()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Shield Missed"); 
#endif
    }
    protected override void OnPopUp()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Shield PopUps");
#endif
    }
    protected override bool OnGrabbed(IShieldable entered_object)
    {
        if (entered_object == null)
            return false;
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Shield Grabbed");
#endif
        entered_object.Shield(shield_duration);
        return true;
    }
}