using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "HurtUpgrades", menuName = "Custom/PowerUpUpgrades/HurtUpgrades")]
public class HurtUpgrades : PowerUpUpgrades
{
    public override Upgrade value_upgrade { get => lives_credit_upgrade; set => lives_credit_upgrade = value; }
    public Upgrade lives_credit_upgrade;
    public new string name = GameConstants.PowerUpNames.HURT;
    protected override void OnEnable()
    {
        base.OnEnable();
        lives_credit_upgrade = new Upgrade();
        lives_credit_upgrade.cost_function = GameLogic.CostFunctions.CalculateLiveCreditUpgradeCost;
        lives_credit_upgrade.name = GameConstants.UpgradeNames.POWERUP_LIVE_CREDIT;
    }
    public override void LoadFromStream(Stream stream)
    {
        base.LoadFromStream(stream);
        lives_credit_upgrade.LoadFromStream(stream);
    }
    public override void SaveToStream(Stream stream)
    {
        base.SaveToStream(stream);
        lives_credit_upgrade.SaveToStream(stream);
    }
}
