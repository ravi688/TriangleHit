public class Live : PowerUp<ILivable>
{
    private int lives_credit;
    public Live(LivePowerUpSettings settings) : base(settings) { lives_credit = settings.num_lives_credit; }

    protected override void OnMissed()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Live Missed");
#endif
    }
    protected override void OnPopUp()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Live PopUps");
#endif
    }
    protected override bool OnGrabbed(ILivable entered_object)
    {
        if (entered_object == null)
            return false;
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Live Grabbed");
#endif
        entered_object.CreditLives(lives_credit);
        return true;
    }
}