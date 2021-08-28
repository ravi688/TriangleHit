
using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Player : PlayerCore,
                                ILivable,
                                IDamageable,
                                IKillable,
                                IBindable,
                                IShieldable,
                                IHealable,
                                IStarCollectable,
                                ICoinCollectable
{
    private GameObject shield_child;

    protected override void Awake()
    {
        base.Awake();
        BindedObjects = new List<IBindable>();
        shield_child = transform.GetChild(1).gameObject;
        shield_child.SetActive(false);
        IsKilled = false;
    }
    protected override void Start()
    {
        base.Start();
        GameLogic.OnPlayerAlive();
        //Not to consider this Player for Touch Bind Input
        GameManager.GetBindManager().RegisterBindable(this, GameConstants.TouchIDs.ENEMY);
        CreateCheats();
    }
    private void OnDestroy()
    {
        //Not to consider this Player for Touch Bind Input
        GameManager.GetBindManager().UnregisterBindable(this);
    }
    protected void LateUpdate()
    {
        if (!IsKilled)
            TakeInput();
    }
    protected virtual void Update()
    {
        if (ShieldTimer != null)
            GameLoop.Update(ShieldTimer);
        GameLoop.Update(thrustMarkController);
        GameLoop.Update(playerFadeController);
    }

    //NOTE: Every GameManager.GetSomeManager() must be called after the awake function
    public int StarCount { get { return GameManager.GetEconomyManager().StarCount; } }
    public int CoinCount { get { return GameManager.GetEconomyManager().CoinCount; } }

    public void CollectStars(int star_count)
    {
        StaticMemory.CollectedStarCount += star_count;
        GameManager.GetEconomyManager().CreditStars(star_count);
    }
    public void CollectCoins(int coin_count) 
    {
        StaticMemory.CollectedCoinCount += coin_count;
        GameManager.GetEconomyManager().CreditCoins(coin_count);
    }

    public new int LiveCount { get { return PlayerCore.LiveCount; } protected set { PlayerCore.LiveCount = value; } }
    public new int MaxLives { get { return PlayerCore.MaxLives; } }
    public bool IsKilled { get; private set; }
    public List<IBindable> BindedObjects { get; set; }
    public bool IsBinded { get { return BindedObjects.Count != 0; } }
    public bool IsShielded { get; private set; }
    public BarMeter Health { get { return PlayerCore.HealthMeter; } }
    public Timer ShieldTimer { get; private set; }
    public void Heal(float amount)
    {
        HealthAmount += (int)amount;
    }
    public void TakeDamage(float amount)
    {
        amount = (int)amount;
        if (IsKilled || amount == 0) return;
        HealthAmount -= (int)amount;
    }
    public void Kill()
    {
        playerFadeController.OnFadeOut = () =>
        {
            instance = null;
            Destroy(gameObject);
        };
        playerFadeController.FadeOut();
        thrustMarkController.FadeOut();
        KilledPosition = transform.position;
        IsKilled = true;
        LiveCount -= 1;
    }
    public void Bind(IBindable bindable) { }

    public void CreditLives(int live_count)
    {
        LiveCount += live_count;
    }
    public void Shield(float duration)
    {
        IsShielded = true;
        ShieldTimer = null;
        ShieldTimer = new Timer(0, duration, 1.0f);
        ShieldTimer.AddListner(delegate ()
        {
            IsShielded = false;
            shield_child.SetActive(false);
        }, OnTimer.End);
        ShieldTimer.AddListner(delegate ()
        {
            shield_child.SetActive(true);
        }, OnTimer.Start);
        ShieldTimer.Start();
    }

    protected override void OnHealthZero()
    {
        if (!IsKilled)
            Kill();
    }

    private void CreateCheats()
    {
        GameManager.GetCheatManager().OnCheatHandler += delegate (CheatType cheat_type)
        {
            if (cheat_type == CheatType.FullPlayerHealth)
            {
                HealthAmount = 100;
            }
        };
        GameManager.GetCheatManager().OnCheatHandler += delegate (CheatType cheat_type)
        {
            if (cheat_type == CheatType.FullPlayerLives)
            {
                LiveCount = MaxLives;
            }
        };
        Action<CheatType> On_Zero_Live_Cheat = null;
        On_Zero_Live_Cheat = delegate (CheatType cheat_type)
        {
            if (!IsKilled && cheat_type == CheatType.ZeroPlayerLives)
            {
                //Order Matters
                LiveCount = 0;
                HealthAmount = 0;
                GameManager.GetCheatManager().OnCheatHandler -= On_Zero_Live_Cheat;
            }
        };
        GameManager.GetCheatManager().OnCheatHandler += On_Zero_Live_Cheat;
        GameManager.GetCheatManager().OnCheatHandler += delegate (CheatType cheat_type)
        {
            if (cheat_type == CheatType.ZeroPlayerHeatlh)
            {
                HealthAmount = 0;
            }
        };
    }
}
