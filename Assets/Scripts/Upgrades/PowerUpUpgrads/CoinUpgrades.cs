using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinUpgrades", menuName = "Custom/PowerUpUpgrades/CoinUpgrades")]
public class CoinUpgrades : PowerUpUpgrades
{
    public override Upgrade value_upgrade { get => coin_credit_upgrade; set => coin_credit_upgrade = value; }
    public Upgrade coin_credit_upgrade;
    public new string name = GameConstants.PowerUpNames.COIN;
    //called when this resource is loaded into the memory
    protected override void OnEnable()
    {
        base.OnEnable();
        coin_credit_upgrade = new Upgrade();
        coin_credit_upgrade.cost_function = GameLogic.CostFunctions.CalculateCoinCreditUpgradeCost;
        coin_credit_upgrade.name = GameConstants.UpgradeNames.POWERUP_COIN_CREDIT;
    }
    public override void LoadFromStream(Stream stream)
    {
        base.LoadFromStream(stream);
        coin_credit_upgrade.LoadFromStream(stream);
    }
    public override void SaveToStream(Stream stream)
    {
        base.SaveToStream(stream);
        coin_credit_upgrade.SaveToStream(stream);
    }
}
