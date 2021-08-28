#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class JoystickConfigurMenu : Menu
{

    [SerializeField]
    private Button SetButton;

    protected override void Start()
    {
        base.Start();
        SetButton.onClick.AddListener(() =>
        {
            //SettingsManager.instance.IsConfiguringJoystick = false;
            Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.SETTINGS_MENU).Activate());
        });
    }
}
