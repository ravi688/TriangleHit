using System;
using System.IO;
using UnityEngine;


public enum PowerUpType
{
    Coin = 0,
    Hurt = 1,
    Medipack = 2,
    Shield = 3,
    Star = 4
}
[Flags]
public enum PowerUpUpgradeType
{
    Duration = 1 << 0,
    Frequency = 1 << 1,
    Value = 1 << 2
}

public class UpgradesManager : IDiskLoadable, IDiskSavable
{
    private CoinUpgrades CoinUpgrades;
    private HurtUpgrades HurtUpgrades;
    private MedipackUpgrades MedipackUpgrades;
    private ShieldUpgrades ShieldUpgrades;
    private StarUpgrades StarUpgrades;

    private bool is_resource_loaded;
    private bool is_disk_data_modified;
    private bool is_memory_data_modified;
    public UpgradesManager()
    {
        is_resource_loaded = false;
        is_memory_data_modified = false;
        is_disk_data_modified = true;
    }
    public int GetPowerUpUpgradeStep(PowerUpType powerup_type, PowerUpUpgradeType upgrade_type)
    {
        switch (powerup_type)
        {
            case PowerUpType.Coin:
                return CoinUpgrades.GetUpgradeStep(upgrade_type);
            case PowerUpType.Hurt:
                return HurtUpgrades.GetUpgradeStep(upgrade_type);
            case PowerUpType.Medipack:
                return MedipackUpgrades.GetUpgradeStep(upgrade_type);
            case PowerUpType.Shield:
                return ShieldUpgrades.GetUpgradeStep(upgrade_type);
            case PowerUpType.Star:
                return StarUpgrades.GetUpgradeStep(upgrade_type);
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("PowerUpType with value {0} is not allowed", (int)powerup_type));
        return 0;
#endif
    }
    public PowerUpUpgradesMask GetPowerUpUpgradesMask(PowerUpType powerup_type)
    {
        switch (powerup_type)
        {
            case PowerUpType.Coin:
                return CoinUpgrades.GetUpgradeMask();
            case PowerUpType.Hurt:
                return HurtUpgrades.GetUpgradeMask();
            case PowerUpType.Medipack:
                return MedipackUpgrades.GetUpgradeMask();
            case PowerUpType.Shield:
                return ShieldUpgrades.GetUpgradeMask();
            case PowerUpType.Star:
                return StarUpgrades.GetUpgradeMask();
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("PowerUpType with value {0} is not allowed", (int)powerup_type));
        return 0;
#endif 
    }
    public bool CanUpgradePowerUp(PowerUpType powerup_type, PowerUpUpgradeType upgrade_type)
    {
        switch (powerup_type)
        {
            case PowerUpType.Coin:
                return CoinUpgrades.CanUpgrade(upgrade_type);
            case PowerUpType.Hurt:
                return HurtUpgrades.CanUpgrade(upgrade_type);
            case PowerUpType.Medipack:
                return MedipackUpgrades.CanUpgrade(upgrade_type);
            case PowerUpType.Shield:
                return ShieldUpgrades.CanUpgrade(upgrade_type);
            case PowerUpType.Star:
                return StarUpgrades.CanUpgrade(upgrade_type);
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("PowerUpType with value {0} is not allowed", (int)powerup_type));
        return false;
#endif  
    }
    public void UpgradePowerUp(PowerUpType powerup_type, PowerUpUpgradeType upgrade_type)
    {
        switch (powerup_type)
        {
            case PowerUpType.Coin:
                CoinUpgrades.Upgrade(upgrade_type);
                break;
            case PowerUpType.Hurt:
                HurtUpgrades.Upgrade(upgrade_type);
                break;
            case PowerUpType.Medipack:
                MedipackUpgrades.Upgrade(upgrade_type);
                break;
            case PowerUpType.Shield:
                ShieldUpgrades.Upgrade(upgrade_type);
                break;
            case PowerUpType.Star:
                StarUpgrades.Upgrade(upgrade_type);
                break;
        }
        is_memory_data_modified = true;
    }
    private void LoadResources()
    {
        if (is_resource_loaded)
            return;
        CoinUpgrades = GameManager.LoadResource<CoinUpgrades>(GameConstants.ResourceFilePaths.COIN_UPGRADES_SETTINGS);
        HurtUpgrades = GameManager.LoadResource<HurtUpgrades>(GameConstants.ResourceFilePaths.LIVE_UPGRADES_SETTINGS);
        MedipackUpgrades = GameManager.LoadResource<MedipackUpgrades>(GameConstants.ResourceFilePaths.MEDIPACK_UPGRADES_SETTINGS);
        ShieldUpgrades = GameManager.LoadResource<ShieldUpgrades>(GameConstants.ResourceFilePaths.SHIELD_UPGRADES_SETTINGS);
        StarUpgrades = GameManager.LoadResource<StarUpgrades>(GameConstants.ResourceFilePaths.STAR_UPGRADES_SETTINGS);
        is_resource_loaded = true;
    }

    public void UnloadResources()
    {
        if (!is_resource_loaded)
            return;
        GameManager.UnloadResource<CoinUpgrades>(CoinUpgrades);
        GameManager.UnloadResource<HurtUpgrades>(HurtUpgrades);
        GameManager.UnloadResource<MedipackUpgrades>(MedipackUpgrades);
        GameManager.UnloadResource<ShieldUpgrades>(ShieldUpgrades);
        GameManager.UnloadResource<StarUpgrades>(StarUpgrades);
        CoinUpgrades = null;
        HurtUpgrades = null;
        MedipackUpgrades = null;
        ShieldUpgrades = null;
        StarUpgrades = null;
        is_resource_loaded = false;
    }

    //Loads the saved upgrade data
    public void Load()
    {
        if (!is_disk_data_modified) return;
        if (!File.Exists(GameConstants.DiskFilePaths.UPGRADE_MANAGER))
        {
            //Create Default Data
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning(string.Format("Upgrade Manager data at {0} doesn't exists; creating default data", GameConstants.DiskFilePaths.UPGRADE_MANAGER));
#endif
            LoadResources();
            //Upgrade default
            CoinUpgrades.UpgradeAll();
            HurtUpgrades.UpgradeAll();
            MedipackUpgrades.UpgradeAll();
            ShieldUpgrades.UpgradeAll();
            StarUpgrades.UpgradeAll();
            is_disk_data_modified = false;
            is_memory_data_modified = true;
            return;
        }
        //Avoid reading from disk, create instance in memory instead
        CoinUpgrades = CoinUpgrades.CreateInstance<CoinUpgrades>();
        HurtUpgrades = HurtUpgrades.CreateInstance<HurtUpgrades>();
        MedipackUpgrades = MedipackUpgrades.CreateInstance<MedipackUpgrades>();
        ShieldUpgrades = ShieldUpgrades.CreateInstance<ShieldUpgrades>();
        StarUpgrades = StarUpgrades.CreateInstance<StarUpgrades>();
        using (FileStream file_stream = File.OpenRead(GameConstants.DiskFilePaths.UPGRADE_MANAGER))
        {
            //Order Matters: the order of loading must match with the order of saving
            CoinUpgrades.LoadFromStream(file_stream);
            HurtUpgrades.LoadFromStream(file_stream);
            MedipackUpgrades.LoadFromStream(file_stream);
            ShieldUpgrades.LoadFromStream(file_stream);
            StarUpgrades.LoadFromStream(file_stream);
            file_stream.Close();
        }
        is_disk_data_modified = false;
        is_memory_data_modified = true;
    }
    //Saves the new upgrade data
    public void Save()
    {
        if (!is_memory_data_modified) return;
        using (FileStream file_stream = File.OpenWrite(GameConstants.DiskFilePaths.UPGRADE_MANAGER))
        {
            //Order Matters: the order of saving must match with the order of loading
            CoinUpgrades.SaveToStream(file_stream);
            HurtUpgrades.SaveToStream(file_stream);
            MedipackUpgrades.SaveToStream(file_stream);
            ShieldUpgrades.SaveToStream(file_stream);
            StarUpgrades.SaveToStream(file_stream);
            file_stream.Close();
        }
        is_disk_data_modified = true;
        is_memory_data_modified = false;
#if DEBUG_MODE
        GameManager.GetLogManager().Log(string.Format("UpgradeManager data is saved to {0}", GameConstants.DiskFilePaths.UPGRADE_MANAGER));
#endif
    }
}