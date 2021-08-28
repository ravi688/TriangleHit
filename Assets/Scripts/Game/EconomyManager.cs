using System.IO;

public enum EconomyType
{
    Coin,
    Star
}

public class EconomyManager : IDiskLoadable, IDiskSavable
{
    private int[] economy_data;
    public int StarCount
    {
        get { return economy_data[0]; }
        private set
        {
            economy_data[0] = value;
            is_memory_data_modified = true;
        }
    }
    public int CoinCount
    {
        get { return economy_data[1]; }
        private set
        {
            economy_data[1] = value;
            is_memory_data_modified = true;
        }
    }

    private bool is_disk_data_modified;
    private bool is_memory_data_modified;
    public EconomyManager(int default_start_count = 10, int default_coin_count = 10)
    {
        economy_data = new int[2];
        StarCount = default_start_count;
        CoinCount = default_coin_count;
        is_memory_data_modified = false;
        is_disk_data_modified = true;
    }

    public void Load()
    {
        if (!is_disk_data_modified) return;
        if (!File.Exists(GameConstants.DiskFilePaths.ECONOMY_MANAGER))
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning(string.Format("{0} doesn't exists setting EconomyManager::Data to default", GameConstants.DiskFilePaths.ECONOMY_MANAGER));
#endif
            economy_data[0] = 200;
            economy_data[1] = 200;
            is_disk_data_modified = false;
            return;
        }
        economy_data.LoadFrom(out economy_data, GameConstants.DiskFilePaths.ECONOMY_MANAGER);
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Economy Data is Loaded", LogSystem.Misc);
#endif
        is_disk_data_modified = false;
    }
    public void Save()
    {
        if (!is_memory_data_modified) return;
        economy_data.SaveTo(GameConstants.DiskFilePaths.ECONOMY_MANAGER);
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Economy Data is Saved", LogSystem.Misc);
#endif
        is_disk_data_modified = true;
        is_memory_data_modified = false;
    }
    public void CreditStars(int star_count)
    {
        StarCount += star_count;
    }
    public void DebitStars(int star_count)
    {
        StarCount -= star_count;
        if (StarCount < 0)
            StarCount = 0;
    }
    public void CreditCoins(int coin_count)
    {
        CoinCount += coin_count;
    }
    public void DebitCoins(int coin_count)
    {
        CoinCount -= coin_count;
        if (CoinCount < 0)
            CoinCount = 0;
    }
}