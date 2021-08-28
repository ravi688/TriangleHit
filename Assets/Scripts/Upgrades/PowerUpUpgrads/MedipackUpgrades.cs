using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "MedipackUpgrades", menuName = "Custom/PowerUpUpgrades/MedipackUpgrades")]
public class MedipackUpgrades : PowerUpUpgrades
{
    public override Upgrade value_upgrade { get => health_credit_upgrade; set => health_credit_upgrade = value; }
    public Upgrade health_credit_upgrade; 
    public new string name = GameConstants.PowerUpNames.MEDIPACK;
    protected override void OnEnable()
    {
        base.OnEnable();
        health_credit_upgrade = new Upgrade();
        health_credit_upgrade.cost_function = GameLogic.CostFunctions.CalculateHealthCreditUpgradeCost;
        health_credit_upgrade.name = GameConstants.UpgradeNames.POWERUP_HEALTH_CREDIT;
    }
    public override void LoadFromStream(Stream stream)
    {
        base.LoadFromStream(stream);
        health_credit_upgrade.LoadFromStream(stream);
    }
    public override void SaveToStream(Stream stream)
    {
        base.SaveToStream(stream);
        health_credit_upgrade.SaveToStream(stream);
    }
}
