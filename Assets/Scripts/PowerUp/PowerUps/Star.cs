
public class Star : PowerUp<IStarCollectable>
{
    private int star_credit;
    public Star(StarPowerUpSettings settings) : base(settings) { star_credit = settings.num_stars_credit; }


    protected override void OnMissed()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Star Missed");
#endif
    }
    protected override void OnPopUp()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Star PopUps");
#endif
    }
    protected override bool OnGrabbed(IStarCollectable entered_object)
    {
        if (entered_object == null)
            return false;
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Star Grabbed");
#endif
        entered_object.CollectStars(star_credit);
        return true;
    }

}