using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "StarUpgrades", menuName = "Custom/PowerUpUpgrades/StarUpgrades")]
public class StarUpgrades : PowerUpUpgrades
{
    public override Upgrade value_upgrade { get => star_credit_upgrade; set => star_credit_upgrade = value; }
    public Upgrade star_credit_upgrade;
    public new string name = GameConstants.PowerUpNames.STAR;
    protected override void OnEnable()
    {
        base.OnEnable();
        star_credit_upgrade = new Upgrade();
        star_credit_upgrade.cost_function = GameLogic.CostFunctions.CalculateStarCreditUpgradeCost;
        star_credit_upgrade.name = GameConstants.UpgradeNames.POWERUP_STAR_CREDIT;
    }
    public override void LoadFromStream(Stream stream)
    {
        base.LoadFromStream(stream);
        star_credit_upgrade.LoadFromStream(stream);
    }
    public override void SaveToStream(Stream stream)
    {
        base.SaveToStream(stream);
        star_credit_upgrade.SaveToStream(stream);
    }
}
