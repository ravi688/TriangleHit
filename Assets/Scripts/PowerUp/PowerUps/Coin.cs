

public class Coin : PowerUp<ICoinCollectable>
{
    private int coin_credit;
    public Coin(CoinPowerUpSettings settings) : base(settings) { coin_credit = settings.num_coins_credit; }

    protected override void OnMissed()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Coin Missed");
#endif
    }
    protected override void OnPopUp()
    {
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Coin PopUps");
#endif
    }
    protected override bool OnGrabbed(ICoinCollectable entered_object)
    {
        if (entered_object == null)
            return false;
        #if DEBUG_MODE
        GameManager.GetLogManager().Log("Coin Grabbed");
#endif
        entered_object.CollectCoins(coin_credit);
        return true;
    }
}
