#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    [SerializeField]
    private Button CloseButton;
    [SerializeField]
    private Button LeaveButton;
    [SerializeField]
    private Button MainMenuButton;

    protected override void Start()
    {
        base.Start();
        CloseButton.onClick.AddListener(() => { Deactivate(); });
        OnActiveCall += () => GameManager.PauseGame();
        OnInactive += () => GameManager.ResumeGame();
        MainMenuButton.onClick.AddListener(() =>
        {
            GameManager.GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.START_MENU);
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(GameConstants.LevelBuildIndices.START_SCREEN));
        });
        LeaveButton.onClick.AddListener(() =>
        {
            GameManager.GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.LEVELS_MENU);
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(GameConstants.LevelBuildIndices.START_SCREEN));
        });
    }

}
