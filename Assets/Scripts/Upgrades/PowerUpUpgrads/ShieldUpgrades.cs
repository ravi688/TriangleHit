
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "ShieldUpgrades", menuName = "Custom/PowerUpUpgrades/ShieldUpgrades")]
public class ShieldUpgrades : PowerUpUpgrades
{
    public override Upgrade value_upgrade { get => shield_duration_upgrade; set => shield_duration_upgrade = value; }
    public Upgrade shield_duration_upgrade;
    public new string name = GameConstants.PowerUpNames.SHIELD;
    protected override void OnEnable()
    {
        base.OnEnable();
        shield_duration_upgrade = new Upgrade();
        shield_duration_upgrade.cost_function = GameLogic.CostFunctions.CalculateShieldDurationUpgradeCost;
        shield_duration_upgrade.name = GameConstants.UpgradeNames.POWERUP_SHIELD_DURATION;
    }
    public override void LoadFromStream(Stream stream)
    {
        base.LoadFromStream(stream);
        shield_duration_upgrade.LoadFromStream(stream);
    }
    public override void SaveToStream(Stream stream)
    {
        base.SaveToStream(stream);
        shield_duration_upgrade.SaveToStream(stream);
    }
}