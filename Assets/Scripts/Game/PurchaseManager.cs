
using System.Collections.Generic;
using System.IO;

public class PurchaseManager : IDiskLoadable, IDiskSavable
{
    private List<string> purchased_ids;
    private bool is_memory_data_modified;
    private bool is_disk_data_modified;
    public PurchaseManager()
    {
        purchased_ids = new List<string>();
        is_memory_data_modified = false;
        is_disk_data_modified = true;
    }
    public bool IsPurchased(string purchase_id)
    {
        return ContainsPurchaseID(purchase_id);
    }
    public bool IsPurchased(IPurchasable purchasable)
    {
        return ContainsPurchaseID(purchasable.PurchaseID);
    }
    public bool TryPurchase(string purchase_id, int cost, EconomyType economy_type)
    {
        switch (economy_type)
        {
            case EconomyType.Coin:
                if (cost > GameManager.GetEconomyManager().CoinCount)
                    return false;
                GameManager.GetEconomyManager().DebitCoins(cost);
                break;
            case EconomyType.Star:
                if (cost > GameManager.GetEconomyManager().StarCount)
                    return false;
                GameManager.GetEconomyManager().DebitStars(cost);
                break;
        }
        AddPurchaseID(purchase_id);
        GameLogic.OnPurchase(purchase_id);
        return true;
    }
    public bool CanPurchase(IPurchasable purchasable)
    {
        switch (purchasable.EconomyType)
        {
            case EconomyType.Coin:
                return purchasable.Cost <= GameManager.GetEconomyManager().CoinCount;
            case EconomyType.Star:
                return purchasable.Cost <= GameManager.GetEconomyManager().StarCount;
        }
#if DEBUG_MODE
        GameManager.GetLogManager().LogError(string.Format("EconomyType with value {0} is not allowed", (int)purchasable.EconomyType));
#endif
        return false;
    }
    public bool TryPurchase(IPurchasable purchasable)
    {
        return TryPurchase(purchasable.PurchaseID, purchasable.Cost, purchasable.EconomyType);
    }
    private bool ContainsPurchaseID(string purchase_id)
    {
        return purchased_ids.Contains(purchase_id);
    }
    private void AddPurchaseID(string id)
    {
        purchased_ids.Add(id);
        is_memory_data_modified = true;
    }
    public void Load()
    {
        if (!is_disk_data_modified) return;
        if (!File.Exists(GameConstants.DiskFilePaths.PURCHASE_MANAGER))
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning(string.Format("{0} doesn't exists setting PurchaseManager::Data to default", GameConstants.DiskFilePaths.PURCHASE_MANAGER));
#endif
            //By Dependency Rule
            GameManager.GetEconomyManager().Load();
            AddPurchaseID(GameConstants.DefaultPurchasedIDs.LEVEL_1);   //purchase the Level 1 Level
            AddPurchaseID(GameConstants.DefaultPurchasedIDs.TRIANGLE_PLAYER);   //purchase the Triangle Player
            is_disk_data_modified = false;
            return;
        }
        //By Dependency Rule
        GameManager.GetEconomyManager().Load();
        purchased_ids.LoadFrom(out purchased_ids, GameConstants.DiskFilePaths.PURCHASE_MANAGER);
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Purchase Data is Loaded", LogSystem.Misc);
#endif
        is_disk_data_modified = false;
    }
    public void Save()
    {
        if (!is_memory_data_modified) return;
        purchased_ids.SaveTo(GameConstants.DiskFilePaths.PURCHASE_MANAGER);
        //By Dependency Rule
        GameManager.GetEconomyManager().Save();
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Purchase Data is Saved", LogSystem.Misc);
#endif
        is_disk_data_modified = true;
        is_memory_data_modified = false;
    }
}