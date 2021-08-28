public class Medipack : PowerUp<IHealable>
{
    private float health_credit;
    public Medipack(MedipackPowerUpSettings settings) : base(settings) { health_credit = settings.health_credit; }

    protected override void OnMissed()
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Medipack Missed");
#endif
    }
    protected override void OnPopUp()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Medipack PopUps");
#endif
    }
    protected override bool OnGrabbed(IHealable entered_object)
    {
        if (entered_object == null)
        {
            return false;
        }
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Medipack Grabbed");
#endif
        entered_object.Heal(health_credit);  
        return true;
    }
}