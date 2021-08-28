#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu
{

    [SerializeField]
    private Button applyButton;
    [SerializeField]
    private Button configureJoystickButton;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button defaultButton;

    protected override void Start()
    {
        base.Start();
        //defaultButton.onClick.AddListener(() => SettingsManager.instance.MakeDafaultSettings());
        //applyButton.onClick.AddListener(() => SettingsManager.instance.SaveRecordedSettings());

        configureJoystickButton.onClick.AddListener(() =>
        {
            Deactivate(() =>
            {
               //SettingsManager.instance.IsConfiguringJoystick = true;
               GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.JOSTICK_CONFIG_MENU).Activate();
            });
        });
        backButton.onClick.AddListener(() =>
        {
            Deactivate(() =>
            {
                //SettingsManager.instance.ExitSettingsMode();
                GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.START_MENU).Activate();
            });
        });
    }
}
