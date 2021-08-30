#if DISABLE_WARNINGS
#pragma warning disable
#endif

#if ANDROID && WINDOWS
#warning [Platform Error] Both ANDROID and WINDOWS are defined! please define one of them.
#endif

#if !ANDROID && !WINDOWS
#warning [Platform Error] Neither ANDROID nor WINDOWS are defined! please define either of them.
#endif

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
public enum GameManagementMode
{
    GamePlay,
    GameEnd,
    GameStart,
    None
}

/*NOTE:
 * Every method of GameManager must be called in Start or Update but not Awake or OnEnable
 */
public class GameManager : MonoBehaviour
#if FPS
    ,IGUIScript
#endif

{ 
    public static GameManager Instance {  get { return _instance; } }
    private static GameManager _instance;
    public static Camera GetCamera()
    {
        if (Instance.game_camera == null)
        {
            Camera game_camera = GameObject.FindObjectOfType<Camera>();
            if (game_camera == null)
            {
                GameObject camera_obj = LoadResource<GameObject>(GameConstants.ResourceFilePaths.CAMERA);
                Instance.game_camera = Instantiate(camera_obj).GetComponent<Camera>();
            }
            else
                Instance.game_camera = game_camera;
        }
        return Instance.game_camera;
    }
    public static Canvas GetCanvas()
    {
        if (Instance.canvas == null)
        {
            Instance.canvas = GameObject.FindObjectOfType<Canvas>();
            if (Instance.canvas == null)
                Instance.canvas = new GameObject("Canvas").GetComponent<Canvas>();
        }
        return Instance.canvas;
    }
    public static GameManagementMode GetGameManagementMode() { return Instance.private_management_mode; }
    public static Level[] GetLevels()
    {
        if(levels == null)
            levels = GameManager.LoadResourceAll<Level>(GameConstants.ResourceFilePaths.LEVELS);
        return levels;
    }
    public static PowerUpManager GetPowerUpManager()
    {
        if (Instance.power_up_manager == null)
            Instance.power_up_manager = new PowerUpManager();
        return Instance.power_up_manager;
    }
    public static EconomyManager GetEconomyManager()
    {
        if (Instance.economy_manager == null)
            Instance.economy_manager = new EconomyManager();
        return Instance.economy_manager;
    }
    public static UpgradesManager GetUpgradesManager()
    {
        if(Instance.upgrades_manager == null)
            Instance.upgrades_manager = new UpgradesManager();
        return Instance.upgrades_manager;
    }
    public static PurchaseManager GetPurchaseManager()
    {
        if (Instance.purchase_manager == null)
            Instance.purchase_manager = new PurchaseManager();
        return Instance.purchase_manager;
    }
    public static PlayerManager GetPlayerManager()
    {
        if (Instance.player_manager == null)
            Instance.player_manager = new PlayerManager();
        return Instance.player_manager;
    }
    public static HUDManager GetHUDManager()
    {
        if (Instance.hud_manager == null)
            Instance.hud_manager = new HUDManager();
        return Instance.hud_manager;
    }
    public static Timer GetStatsScreenTimer()
    {
        if (Instance.load_stats_screen_timer == null)
            Instance.load_stats_screen_timer = new Timer(0, 2, 1);
        return Instance.load_stats_screen_timer;
    }
    public static AgainAliveManager GetAgainAliveManager()
    {
        if (Instance.again_alive_manager == null)
            Instance.again_alive_manager = new AgainAliveManager();
        return Instance.again_alive_manager;
    }
    public static CameraController GetCameraController()
    {
        return GetCamera().GetComponent<CameraController>();
    }
    public static BindManager GetBindManager()
    {
        if (Instance.bind_manager == null)
            Instance.bind_manager = new BindManager();
        return Instance.bind_manager;
    }
    public static RopeManager GetRopeManager()
    {
        if (Instance.rope_manager == null)
            Instance.rope_manager = new RopeManager();
        return Instance.rope_manager;
    }
    public static PointManager GetPointManager()
    {
        if (Instance.point_manager == null)
            Instance.point_manager = new PointManager();
        return Instance.point_manager;
    }
    public static TouchManager GetTouchManager()
    {
        if (Instance.touch_manager == null)
            Instance.touch_manager = new TouchManager();
        return Instance.touch_manager;
    }
#if DEBUG_MODE
    public static LogManager GetLogManager()
    {
        return LogManager.Instance;
    }
#endif
    public static CheatManager GetCheatManager()
    {
        if (Instance.cheat_manager == null)
            Instance.cheat_manager = new GameObject("CheatManager").AddComponent<CheatManager>();
        return Instance.cheat_manager;
    }
    public static MenuManager GetMenuManager()
    {
        if (menu_manager == null)
            menu_manager = new MenuManager();
        return menu_manager;
    }
    public static LevelLoadManager GetLevelLoadManager()
    {
        if (Instance.level_load_manager == null)
            Instance.level_load_manager = new LevelLoadManager();
        return Instance.level_load_manager;
    }
    public static ExitMenu GetExitMenu()
    {
        if (Instance.exit_menu == null)
        {
            ExitMenu prefab = LoadResource<ExitMenu>(GameConstants.ResourceFilePaths.EXIT_MENU);
            RectTransform rectTransform;
            Transform transform;
            Canvas canvas = GetCanvas();
            transform = Instantiate(prefab).transform;
            transform.SetParent(canvas.transform);
            rectTransform = transform.gameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;
            Instance.exit_menu = rectTransform.GetComponent<ExitMenu>();
            Instance.exit_menu_prefab = prefab;
        }
        return Instance.exit_menu;
    }
    public static PauseMenu GetPauseMenu()
    {
        if (Instance.pause_menu == null)
        {
            PauseMenu prefab = LoadResource<PauseMenu>(GameConstants.ResourceFilePaths.PAUSE_MENU);

            RectTransform rectTransform;
            Transform transform;
            Canvas canvas = GetCanvas();

            transform = Instantiate(prefab).transform;
            transform.SetParent(canvas.transform);
            rectTransform = transform.gameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector3.zero;
            Instance.pause_menu = rectTransform.GetComponent<PauseMenu>();
            Instance.pause_menu_prefab = prefab;
        }
        return Instance.pause_menu;
    }
    public static IControlSystem GetGameControlSystem()
    {
        if (Instance.game_control_system == null)
        {
#if ANDROID
            JoystickSettings joystick_settings = Resources.Load<JoystickSettings>(GameConstants.ResourceFilePaths.JOYSTICK_SETTINGS);
            Instance.game_control_system = new AndroidControlSystem(joystick_settings, GameConstants.TouchIDs.THRUST, GameConstants.TouchIDs.JOYSTICK);
#endif
#if WINDOWS
            Instance.game_control_system = new WindowsControlSystem();
#endif
        }
        return Instance.game_control_system;
    }

    private Image black_image;
    private AlphaAdapter black_image_alpha_adapter;
    private FadeController black_image_fade_controller;
    private Timer load_stats_screen_timer;
    private Canvas canvas;
    private static Level[] levels;
    private HUDManager hud_manager;
    private AgainAliveManager again_alive_manager;
    private BindManager bind_manager;
    private RopeManager rope_manager;
    private PointManager point_manager;
    private TouchManager touch_manager;
    private CheatManager cheat_manager;
    private PowerUpManager power_up_manager;
    private EconomyManager economy_manager;
    private PurchaseManager purchase_manager;
    private UpgradesManager upgrades_manager;
    private static MenuManager menu_manager;
    private LevelLoadManager level_load_manager;
    private PlayerManager player_manager;
    private IControlSystem game_control_system;
    private Camera game_camera;
    private PauseMenu pause_menu;
    private ExitMenu exit_menu;
    private PauseMenu pause_menu_prefab;
    private ExitMenu exit_menu_prefab;

    [SerializeField]
    private GameManagementMode management_mode = GameManagementMode.None;
    private GameManagementMode private_management_mode = GameManagementMode.None;
    private bool is_update_management_mode = false;

#if FPS
    private Rect fps_rect;
    private GUIStyle fps_style;
    private int fps;
    private Timer fps_timer;

    public void OnGUIStart()
    {
        fps_timer.IsLoop = true;
        fps_rect = new Rect(20, 20, 400, 400);
        fps_style = new GUIStyle(GUI.skin.label);
        fps_style.fontStyle = FontStyle.Bold;
        fps_style.fontSize = 50;
    }
    public void OnGUIUpdate()
    {
        GUI.Label(fps_rect,  fps.ToString(), fps_style);
    }
#endif

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
#if ANDROID && WINDOWS
        Debug.LogWarning("[Platform Warning] You have defined ANDROID and WINDOWS both, please define either one of those");
#endif

#if !ANDROID && !WINDOWS
        Debug.LogError("[Platform Error] You haven't defined neither ANDROID nor WINDOWS, please define either one of those");
        Debug.Break();
        return;
#endif

#if FPS
        fps_timer = new Timer(0, 60, 0.5f);
        fps_timer.AddListner(() => fps = (int)(1 / Time.deltaTime), OnTimer.Update | OnTimer.Start);
        fps_timer.Start();
        GUIScriptManager.Register(this);
#endif

        SetupStaticMemory();
#if DEBUG_MODE
        if (management_mode == GameManagementMode.None)
            GetLogManager().LogWarning("GameManager::GameManagementMode is set to None");
        else
#endif
        if (management_mode != GameManagementMode.None)
            SetGameManagementMode(management_mode);
        switch (management_mode)
        {
            case GameManagementMode.GameStart:
                GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.START_MENU);
                break;
            case GameManagementMode.GameEnd:
                GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.STATS_MENU);
                break;
        }
#if DEBUG_MODE
        GetLogManager().Log("GameManager's Awake is called");
#endif
    }
    private void Update()
    {
#if FPS
        fps_timer.Update();
#endif
        GameLoop.Update(GetLevelLoadManager());
        GameLoop.Update(GetMenuManager());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (private_management_mode)
            {
                case GameManagementMode.GameEnd:
                    //GameEnd, i.e. stats screen will have the Exit Menu but not Pause Menu
                    if (!GetExitMenu().IsActivated)
                        GetExitMenu().Activate();
                    else
                        GetExitMenu().Deactivate();
                    break;
                case GameManagementMode.GamePlay:
                    //GamePlay, i.e. gameplay level will have the PauseMenu but not Exit Menu
                    if (!GetPauseMenu().IsActivated)
                        GetPauseMenu().Activate();
                    else
                        GetPauseMenu().Deactivate();
                    break;
                case GameManagementMode.GameStart:
                    //GameStart, i.e. start screen will have the Exit but not Pause Menu
                    if (!GetExitMenu().IsActivated)
                        GetExitMenu().Activate();
                    else
                        GetExitMenu().Deactivate();
                    break;
            }
        }

        if (is_update_management_mode)
            switch (private_management_mode)
            {
                case GameManagementMode.GameEnd:
                    break;
                case GameManagementMode.GamePlay:
                    GameLoop.Update(GetPowerUpManager());
                    GameLoop.Update(GetStatsScreenTimer());
                    GameLoop.Update(GetHUDManager());
                    GameLoop.Update(GetAgainAliveManager());
                    GameLoop.Update(GetGameControlSystem());
                    GameLoop.Update(GetRopeManager());
                    GameLoop.Update(GetPointManager());
#if WINDOWS
                    GameLoop.Update(GetBindManager());
#endif
#if ANDROID
                    GameLoop.Update(GetTouchManager());
#endif
                    break;
                case GameManagementMode.GameStart:
                    break;
            }
    }
    private void SetupGameOver()
    {
        GetStatsScreenTimer().AddListner(() =>
        {
            GameManager.GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.STATS_MENU);
            GameManager.GetLevelLoadManager().LoadLevel(GameConstants.LevelBuildIndices.STATS_SCREEN);
        }, OnTimer.End);
    }
    private void SetupGamePlayManagers()
    {
        GetHUDManager().LoadAllHUDElements();
        GetRopeManager().LoadRopeSettings();
        GetAgainAliveManager().LoadCounterText();
        GetPowerUpManager().LoadAllPowerUpSettings();                   //load all the power up settings
        GetPointManager().LoadPointSprite();
        //Order Matters
        GetPlayerManager().Load();  //for latest updates
        GetPlayerManager().LoadPlayer(GetPlayerManager().PlayerName);   //load the player prefab

        GameObject spawn_points_container = GameObject.FindGameObjectWithTag(GameConstants.Tags.SPAWN_POINTS_CONTAINER);
        List<Vector3> spawn_points = new List<Vector3>();
        if (spawn_points_container == null)
        {
#if DEBUG_MODE
            GetLogManager().LogWarning("No GameObject is found with Tag: " + GameConstants.Tags.SPAWN_POINTS_CONTAINER);
#endif
            spawn_points.Add(new Vector3(0, 0, 0));
        }
        else
        {
            int count = spawn_points_container.transform.childCount;
            for (int i = 0; i < count; i++)
                spawn_points.Add(spawn_points_container.transform.GetChild(i).position);
        }

        GetAgainAliveManager().AgainInstantiateTime = GameConstants.Config.AGAIN_INSTANTIATE_TIME;
        GetAgainAliveManager().PlayerPrefab = GetPlayerManager().GetPlayer<Player>(GetPlayerManager().PlayerName).gameObject;
        GetPowerUpManager().Points = spawn_points.ToArray();

        GetAgainAliveManager().Initialize();


        GetBindManager().Initialize();
        GetRopeManager().Initialize();
        GetPointManager().Initialize();

        GetAgainAliveManager().Spawn(new Vector3(7.3f, 0, 0));
        //Both PowerUpManager and HUDManager must be initialized after the Spawn of the Player
        GetPowerUpManager().Initialize();
        GetHUDManager().Initialize();
        GetPowerUpManager().Start();
    }

    //Static Methods for Managing Memory
    public static void SetupStaticMemory()
    {
        SceneManager.activeSceneChanged += (previous, next) => GetLevelLoadManager().RevealScene();
        SceneManager.activeSceneChanged += (scene, scene2) =>
        {
#if DEBUG_MODE
            GetLogManager().Log("Switching the GameManagement mode");
#endif
            Instance.is_update_management_mode = true;
            StaticMemory.US_OnActiveSceneChanged();
        };
        GetLevelLoadManager().LoadBlackImage();
        GetLevelLoadManager().Initialize();
    }

    //Static Methods For Managing Game
    public static void ForceSwitchMode()
    {
        if (StaticMemory.US_OnActiveSceneChanged != null) StaticMemory.US_OnActiveSceneChanged();
    }
    public static void UnloadGameplay(Scene scene)
    {
        Instance.hud_manager = null;
        Instance.bind_manager = null;
        Instance.rope_manager = null;
        Instance.again_alive_manager = null;
        Instance.power_up_manager = null;
        Instance.point_manager = null;
        Instance.game_control_system = null;
        Instance.pause_menu = null;
        Instance.touch_manager = null;
        GetHUDManager().UnloadAllHUDElements();
        GetRopeManager().UnloadRopeSettings();
        GetPointManager().UnloadPointSprite();
        GetPowerUpManager().UnloadPowerUpSettings();
        GetPlayerManager().UnloadAllPlayers();
        GetPlayerManager().UnloadAllPlayersSettings();
        GetAgainAliveManager().UnloadCounterText();
        UnloadResource(Instance.pause_menu_prefab);
        Instance.pause_menu_prefab = null;
        SceneManager.sceneUnloaded -= UnloadGameplay;
    }
    public static void SetGameManagementMode(GameManagementMode mode)
    {
        if (mode == Instance.private_management_mode)
        {
#if DEBUG_MODE
            GetLogManager().LogWarning(string.Format("GameManagementMode is already set to {0}", mode.ToString()));
#endif
            return;
        }
        //Unload the previous loaded resources
        switch (Instance.private_management_mode)
        {
            case GameManagementMode.GamePlay:
                SceneManager.sceneUnloaded += UnloadGameplay;
                break;
            case GameManagementMode.GameEnd:
                Instance.exit_menu = null;
                UnloadResource(Instance.exit_menu_prefab);
                Instance.exit_menu_prefab = null;
                break;
            case GameManagementMode.GameStart:
                Instance.exit_menu = null;
                UnloadResource(Instance.exit_menu_prefab);
                Instance.exit_menu_prefab = null;
                break;
        }
        //     Resources.UnloadUnusedAssets();
        switch (mode)
        {
            case GameManagementMode.GameStart:
                StaticMemory.US_OnActiveSceneChanged = () =>
                {
#if DEBUG_MODE
                    GetLogManager().Log("Game Management Mode is set to GameManagementMode.GameStart");
#endif
                    GetExitMenu();
                    StaticMemory.CurrentLevelBuildIndex = GameConstants.LevelBuildIndices.START_SCREEN;
                };
                break;
            case GameManagementMode.GamePlay:
                StaticMemory.US_OnActiveSceneChanged = () =>
                {
#if DEBUG_MODE
                    GetLogManager().Log("Game Management Mode is set to GameManagementMode.GamePlay");
#endif
                    GetPauseMenu();
                    Instance.SetupGamePlayManagers();
                    Instance.SetupGameOver();
                    //No need to consider StaticMemory.CurrentLevelBuildIndex here because it is automatically updated
                    //in the LevelLoadManager when we call LevelLoadManager::LoadLevel(build_index)
                };
                break;
            case GameManagementMode.GameEnd:
                StaticMemory.US_OnActiveSceneChanged = () =>
                {
#if DEBUG_MODE
                    GetLogManager().Log("Game Management Mode is set to GameManagementMode.GameEnd");
#endif
                    GetExitMenu();
                    StaticMemory.CurrentLevelBuildIndex = GameConstants.LevelBuildIndices.STATS_SCREEN;
                };
                break;
        }
        Instance.private_management_mode = mode;
        Instance.is_update_management_mode = false;
    }
    public static void PauseGame()
    {
#if DEBUG_MODE
        GetLogManager().Log("Game Paused");
        
#endif
        Time.timeScale = 0;
    }
    public static void ResumeGame()
    {
#if DEBUG_MODE
        GetLogManager().Log("Game Resumed");
#endif
        Time.timeScale = 1;
    }
    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void UnloadResource<T>(T asset) where T : UnityEngine.Object
    {
#if UNLOAD_ASSETS
        Resources.UnloadAsset(asset);
#endif
    }
    public static void UnloadResourcesAll<T>(T[] array) where T : UnityEngine.Object
    {
#if UNLOAD_ASSETS
        foreach (T obj in array)
            Resources.UnloadAsset(obj);
#endif
    }
    public static T[] LoadResourceAll<T>(string path) where T : UnityEngine.Object
    {
        T[] reference = Resources.LoadAll<T>(path);
#if DEBUG_MODE
        if (reference == null)
            GetLogManager().LogError("Failed to load Resource at " + path);
#endif
        return reference;
    }
    public static T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        T reference = Resources.Load<T>(path);
#if DEBUG_MODE
        if (reference == null)
            GetLogManager().LogError("Failed to load Resource at " + path);
#endif
        return reference;
    }
}