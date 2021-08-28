
using UnityEngine;
using System.IO;
using System;

[Flags]
public enum PowerUpUpgradesMask
{
    DURATION = 1 << 0,
    FREQUENCY = 1 << 1,
    VALUE = 1 << 2
}

public abstract class PowerUpUpgrades : ScriptableObject, IStreamLoadable, IStreamSavable
{
    public Upgrade duration_upgrade;
    public Upgrade frequency_upgrade;
    public virtual Upgrade value_upgrade { get; set; }
    protected virtual void OnEnable()
    {
        duration_upgrade = new Upgrade();
        frequency_upgrade = new Upgrade();
        duration_upgrade.cost_function = GameLogic.CostFunctions.CalculateDurationUpgradeCost;
        frequency_upgrade.cost_function = GameLogic.CostFunctions.CalculateFrequencyUpgradeCost;
        duration_upgrade.name = GameConstants.UpgradeNames.POWERUP_DURATION;
        frequency_upgrade.name = GameConstants.UpgradeNames.POWERUP_FREQUENCY;
    }
    public int GetUpgradeStep(PowerUpUpgradeType upgrade_type)
    {
        switch (upgrade_type)
        {
            case PowerUpUpgradeType.Duration:
                return duration_upgrade.upgraded_steps;
            case PowerUpUpgradeType.Frequency:
                return frequency_upgrade.upgraded_steps;
            case PowerUpUpgradeType.Value:
                return value_upgrade.upgraded_steps;
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("PowerUpUpgradeType with value {0} is not allowed", (int)upgrade_type));
        return 0;
#endif
    }
    public PowerUpUpgradesMask GetUpgradeMask()
    {
        PowerUpUpgradesMask mask = 0;
        if (CanUpgrade(PowerUpUpgradeType.Duration))
            mask |= PowerUpUpgradesMask.DURATION;
        if (CanUpgrade(PowerUpUpgradeType.Frequency))
            mask |= PowerUpUpgradesMask.FREQUENCY;
        if (CanUpgrade(PowerUpUpgradeType.Value))
            mask |= PowerUpUpgradesMask.VALUE;
        return mask;
    }
    public bool CanUpgrade(PowerUpUpgradeType upgrade_type)
    {
        switch (upgrade_type)
        {
            case PowerUpUpgradeType.Duration:
                return GameManager.GetPurchaseManager().CanPurchase(duration_upgrade);
            case PowerUpUpgradeType.Frequency:
                return GameManager.GetPurchaseManager().CanPurchase(frequency_upgrade);
            case PowerUpUpgradeType.Value:
                return GameManager.GetPurchaseManager().CanPurchase(value_upgrade);
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("PowerUpUpgradeType with value {0} is not allowed", (int)upgrade_type));
        return false;
#endif
    }
    public virtual void Upgrade(PowerUpUpgradeType upgrade_type)
    {
        switch(upgrade_type)
        {
            case PowerUpUpgradeType.Duration:
                Upgrade(0);
                break;
            case PowerUpUpgradeType.Frequency:
                Upgrade(1);
                break;
            case PowerUpUpgradeType.Value:
                Upgrade(2);
                break;
        }
    }
    public void UpgradeAll()
    {
        Upgrade(0);
        Upgrade(1);
        Upgrade(2);
    }
    public virtual void Upgrade(int upgrade_index)
    {
        bool result;
        PurchaseManager purchaseManager = GameManager.GetPurchaseManager();
        purchaseManager.Load(); //for latest updates
        switch(upgrade_index)
        {
            case 0:
                result = purchaseManager.TryPurchase(duration_upgrade);
#if DEBUG_MODE
                if (!result)
                    GameManager.GetLogManager().LogWarning(string.Format("Failed to upgrade {0}, cost: {1}, balance: {2}", duration_upgrade.name, duration_upgrade.Cost, GameManager.GetEconomyManager().StarCount));
#endif
                if (result)
                    duration_upgrade.upgraded_steps++;
                break;
            case 1:
                result = purchaseManager.TryPurchase(frequency_upgrade);
#if DEBUG_MODE
                if (!result)
                    GameManager.GetLogManager().LogWarning(string.Format("Failed to upgrade {0}, cost: {1}, balance: {2}", frequency_upgrade.name, frequency_upgrade.Cost, GameManager.GetEconomyManager().StarCount));
#endif

                if (result)
                    frequency_upgrade.upgraded_steps++;
                break;
            default:
                result = purchaseManager.TryPurchase(value_upgrade);
#if DEBUG_MODE
                if (!result)
                    GameManager.GetLogManager().LogWarning(string.Format("Failed to upgrade {0}, cost: {1}, balance: {2}", value_upgrade.name, value_upgrade.Cost, GameManager.GetEconomyManager().StarCount));
#endif
                if (result)
                    value_upgrade.upgraded_steps++;
                break;
        }
    }
    public virtual void LoadFromStream(Stream stream)
    {
        duration_upgrade.LoadFromStream(stream);
        frequency_upgrade.LoadFromStream(stream);
    }
    public virtual void SaveToStream(Stream stream)
    {
        duration_upgrade.SaveToStream(stream);
        frequency_upgrade.SaveToStream(stream);
    }
}
