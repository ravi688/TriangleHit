#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class ExitMenu : Menu
{
    [SerializeField]
    private Button CloseButton;
    [SerializeField]
    private Button YesButton;
    [SerializeField]
    private Button NoButton;

    protected override void Start()
    {
        base.Start();
        CloseButton.onClick.AddListener(() => { Deactivate(); });
        YesButton.onClick.AddListener(() => { Deactivate(GameManager.QuitGame); });
        NoButton.onClick.AddListener(() => { Deactivate(); });
    }
}
