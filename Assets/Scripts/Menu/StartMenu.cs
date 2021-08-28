#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class StartMenu : Menu
{

    [SerializeField]
    private Button PlayButton;
    [SerializeField]
    private Button MoreButton;
    [SerializeField]
    private Button WatchAdsButton;
    [SerializeField]
    private Button SettingsButton;
    [SerializeField]
    private Button UpgradesButton;
    [SerializeField]
    private Button HelpButton;


    protected override void Start()
    {
        base.Start();
        PlayButton.onClick.AddListener(() =>
        {
            Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.LEVELS_MENU).Activate());
        });
        MoreButton.onClick.AddListener(() =>
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Entered into More Menu");
#endif
            //Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.MORE_MENU).Activate());
        });
        WatchAdsButton.onClick.AddListener(() =>
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Watching Ads");
#endif
        });
        SettingsButton.onClick.AddListener(() =>
        {
            Deactivate(() =>
            {
                GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.SETTINGS_MENU).Activate();
                //  SettingsManager.instance.EnterSettingsMode();
            });
        });
        UpgradesButton.onClick.AddListener(() =>
        {
            Deactivate(() =>
            {
                GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.UPGRADES_MENU).Activate();
            });
        });
        HelpButton.onClick.AddListener(() =>
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Entered into Help Menu");
#endif
            //Deactivate(() =>
            //{
            //    GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.HELP_MENU).Activate();
            //});
        });
    }
}
