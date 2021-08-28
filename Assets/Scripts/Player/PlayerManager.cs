using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class PlayerManager : IDiskLoadable, IDiskSavable
{
    //Current selected player name
    public string PlayerName
    {
        get { return data.player_name; }
        set
        {
            data.player_name = value;
            is_memory_data_modified = true;
        }
    }
    //Current selected player index
    public int PlayerIndex
    {
        get { return data.player_index; }
        set
        {
            data.player_index = value;
            is_memory_data_modified = true;
        }
    }

    private bool is_memory_data_modified;
    private bool is_disk_data_modified;

    private struct Data
    {
        public string player_name;     //prefab name in the dir Resources/Prefabs/Players
        public int player_index;       //index when all the PlayerCoreSettings SOs are loaded in sequence, not recommended to use
    }
    private Data data;
    private Dictionary<string, PlayerCoreSettings> players_settings;
    private Dictionary<string, Player> players;

    public PlayerManager()
    {
        is_disk_data_modified = true;
        is_memory_data_modified = false;
        players_settings = new Dictionary<string, PlayerCoreSettings>();
        players = new Dictionary<string, Player>();
    }

    public void Load()
    {
        if (!is_disk_data_modified) return;
        if (!File.Exists(GameConstants.DiskFilePaths.PLAYER_MANAGER))
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning(string.Format("{0} doesn't exists setting PlayerManager::Data to default", GameConstants.DiskFilePaths.PLAYER_MANAGER));
#endif
            PurchaseManager purchaseManager = GameManager.GetPurchaseManager();
            purchaseManager.Load(); //for latest updates
            if (!purchaseManager.IsPurchased(GameConstants.DefaultPurchasedIDs.TRIANGLE_PLAYER))
            {
                bool result = purchaseManager.TryPurchase(GameConstants.DefaultPurchasedIDs.TRIANGLE_PLAYER, 10, EconomyType.Coin);
#if DEBUG_MODE
                if (!result)
                    GameManager.GetLogManager().LogError("Failed to purchase Triangle");
#endif
            }

            PlayerName = GameConstants.PlayerNames.TRIANGLE;
            is_disk_data_modified = false;
            return;
        }
        using (FileStream file_stream = File.OpenRead(GameConstants.DiskFilePaths.PLAYER_MANAGER))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            data = (Data)binaryFormatter.Deserialize(file_stream);
            file_stream.Close();
        }
        is_disk_data_modified = false;
    }
    public void Save()
    {
        if (!is_memory_data_modified) return;
        using (FileStream file_stream = File.OpenWrite(GameConstants.DiskFilePaths.PLAYER_MANAGER))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file_stream, data);
            file_stream.Close();
        }
        is_disk_data_modified = true;
        is_memory_data_modified = false;
    }

    //player_name must be equal to PlayerCoreSettings::name
    public T GetPlayerSettings<T>(string player_name) where T : PlayerCoreSettings
    {
        if (players_settings == null)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogError(string.Format("Resource PlayerCoreSettings::name with name {0} is not loaded", player_name));
#endif
            return null;
        }
        return (T)players_settings[player_name];
    }

    //player_name must be equal to Prefab name, and Prefab name must be equal to PlayerCoreSettings::name
    public T GetPlayer<T>(string player_name) where T : Player
    {
        if (players == null)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogError(string.Format("Resource PlayerCore with name {0} is not loaded", player_name));
#endif
            return null;
        }
        return (T)players[player_name];
    }

    public void LoadAllPlayersSettings()
    {
        LoadPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);
        LoadPlayerSettings<SquarePlayerSettings>(GameConstants.PlayerNames.SQUARE);
        LoadPlayerSettings<PentagonPlayerSettings>(GameConstants.PlayerNames.PENTAGON);
        LoadPlayerSettings<HexagonPlayerSettings>(GameConstants.PlayerNames.HEXAGON);
    }
    public void UnloadAllPlayersSettings()
    {
        UnloadPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);
        UnloadPlayerSettings<SquarePlayerSettings>(GameConstants.PlayerNames.SQUARE);
        UnloadPlayerSettings<PentagonPlayerSettings>(GameConstants.PlayerNames.PENTAGON);
        UnloadPlayerSettings<HexagonPlayerSettings>(GameConstants.PlayerNames.HEXAGON);
    }
    //player_name must be equal to the PlayerCoreSettings::name and Player prefab name
    public void LoadPlayerSettings<T>(string player_name) where T : PlayerCoreSettings
    {
        if (!players_settings.ContainsKey(player_name))
            players_settings.Add(player_name, GameManager.LoadResource<T>(Path.Combine(GameConstants.ResourceFilePaths.PLAYERS_SETTINGS, player_name)));
    }

    public void UnloadPlayerSettings<T>(string player_name) where T : PlayerCoreSettings
    {
        if (!players_settings.ContainsKey(player_name))
            return;
        GameManager.UnloadResource<T>(players_settings[player_name] as T);
        players_settings.Remove(player_name);
    }

    //player_name must be equal to Prefab name, and Prefab name must be equal to PlayerCoreSettings::name
    public void LoadPlayer<T>(string player_name) where T : Player
    {
        if (!players.ContainsKey(player_name))
            players.Add(player_name, GameManager.LoadResource<T>(Path.Combine(GameConstants.ResourceFilePaths.PLAYERS, player_name)));
    }
    public void LoadPlayer(string player_name)
    {
        if (player_name.Equals(GameConstants.PlayerNames.TRIANGLE))
            LoadPlayer<TrianglePlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.SQUARE))
            LoadPlayer<SquarePlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.PENTAGON))
            LoadPlayer<PentagonPlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.HEXAGON))
            LoadPlayer<HexagonPlayer>(player_name);
#if DEBUG_MODE
        else
            GameManager.GetLogManager().LogError("Not matched Player name with " + player_name);
#endif
    }
    public void UnloadPlayer(string player_name)
    {
        if (!players.ContainsKey(player_name))
            return;
        if (player_name.Equals(GameConstants.PlayerNames.TRIANGLE))
            UnloadPlayer<TrianglePlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.SQUARE))
            UnloadPlayer<SquarePlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.PENTAGON))
            UnloadPlayer<PentagonPlayer>(player_name);
        else if (player_name.Equals(GameConstants.PlayerNames.HEXAGON))
            UnloadPlayer<HexagonPlayer>(player_name);
    }
    public void UnloadPlayer<T>(string player_name) where T : Player
    {
        if (!players.ContainsKey(player_name))
            return;
        GameManager.UnloadResource<T>(players[player_name] as T);
        players.Remove(player_name);
    }

    public void UnloadAllPlayers()
    {
        UnloadPlayer<TrianglePlayer>(GameConstants.PlayerNames.TRIANGLE);
        UnloadPlayer<SquarePlayer>(GameConstants.PlayerNames.SQUARE);
        UnloadPlayer<PentagonPlayer>(GameConstants.PlayerNames.PENTAGON);
        UnloadPlayer<HexagonPlayer>(GameConstants.PlayerNames.HEXAGON);
    }
    public void LoadAllPlayers()
    {
        LoadPlayer<TrianglePlayer>(GameConstants.PlayerNames.TRIANGLE);
        LoadPlayer<SquarePlayer>(GameConstants.PlayerNames.SQUARE);
        LoadPlayer<PentagonPlayer>(GameConstants.PlayerNames.PENTAGON);
        LoadPlayer<HexagonPlayer>(GameConstants.PlayerNames.HEXAGON);
    }
}