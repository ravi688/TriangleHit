using UnityEngine;


public abstract class PlayerCoreSettings : ScriptableObject, IPurchasable
{
    public EconomyType EconomyType {  get { return EconomyType.Coin;  } }
    public int Cost {  get { return cost;  } }
    public string PurchaseID {  get { return string.Format("purchase-{0}-player-{1}", cost, name);  } }

    public new abstract string name { get; }
    public float thrust_bar_fillup_rate = 10.0f;
    public float thrust_bar_useup_rate = 20.0f;
    public float thrust_speed = 20.0f;
    public float move_speed = 10.0f;
    public float hit_factor = 1.0f;
    public int max_lives = 3;
    [SerializeField]
    private int cost = 10;
}