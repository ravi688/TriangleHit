#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class StatsPage : TVScreenPage
{
    private enum ButtonMenuType
    {
        NextLevel,
        NotNextLevel
    }
    private enum ButtonEventType
    {
        LoadMenu,
        LoadNextLevel,
        Replay
    }

    [Header("Discription texts")]
    [SerializeField]
    private Text star_collected_text;
    [SerializeField]
    private Text coins_collected_text;
    [SerializeField]
    private Text your_score_text;
    [SerializeField]
    private Text high_score_text;
    [Space]
    [Header("Value texts")]
    [SerializeField]
    private Text star_collected_value_text;
    [SerializeField]
    private Text coins_collected_value_text;
    [SerializeField]
    private Text your_score_value_text;
    [SerializeField]
    private Text high_score_value_text;
    [Space]
    [Header("Configuration values")]
    [SerializeField]
    private int write_speed = 4000;                    //4000 characters per minute

    [SerializeField]
    private StandardAnimation sin_alpha_anim;
    private StandardAnimationPlayer anim_player;

    [SerializeField]
    private Button menu_button;
    [SerializeField]
    private Button next_button;
    [SerializeField]
    private Button replay_button;

    private TypeWritter[] type_writters;
    protected override void Awake()
    {
        base.Awake();
        type_writters = new TypeWritter[4];
        anim_player = new StandardAnimationPlayer();
        //Order matters
        InitializeTypeWriterEffect();
        InitializeAnimation();
        LoadStatsValues();
        InitializeButtonCallbacks();
    }
    protected override void Update()
    {
        base.Update();
        GameLoop.Update(anim_player);
        for (int i = 0; i < 4; i++)
            GameLoop.Update(type_writters[i]);
    }
    protected override void Start()
    {
        base.Start();
        PurchaseManager purchaseManager = GameManager.GetPurchaseManager();
        purchaseManager.Load();
        Level next_level = GameManager.GetLevels()[StaticMemory.CurrentGameplayLevelBuildIndex - GameConstants.LevelBuildIndices.LEVEL0 + 1];
        if (!purchaseManager.IsPurchased(next_level))
            if (purchaseManager.TryPurchase(next_level))
                //next level is unlocked
                next_button.gameObject.SetActive(true);
            else
                //insufficient balance to unlock the next level
                next_button.gameObject.SetActive(false);
        else
            //next level is already purchased/unlocked
            next_button.gameObject.SetActive(true);

        ShowTypeWritterEffect();
    }
    private void InitializeAnimation()
    {
        type_writters[0].OnEnd = delegate
        {
            AlphaAdapter _adapter = new UITextAlphaAdapter(your_score_text);
            anim_player.InitializeWith(_adapter, null, sin_alpha_anim);
            anim_player.PlayAtPosition(Vector2.zero, Vector3.zero, Quaternion.identity);
        };
    }
    private void ShowTypeWritterEffect()
    {
        type_writters[0].Write("Stars collected");
        type_writters[1].Write("Coins collected");
        type_writters[2].Write("Your score");
        type_writters[3].Write("High score");
    }
    private void InitializeTypeWriterEffect()
    {
        type_writters[0] = new TypeWritter(star_collected_text);
        type_writters[1] = new TypeWritter(coins_collected_text);
        type_writters[2] = new TypeWritter(your_score_text);
        type_writters[3] = new TypeWritter(high_score_text);
        for (int i = 0; i < 4; i++)
        {
            type_writters[i].speed = write_speed;
        }
    }
    private void InitializeButtonCallbacks()
    {
        menu_button.onClick.AddListener(() =>
        {
            GameManager.GetMenuManager().ActivateMenuOnLoad(GameConstants.MenuNames.START_MENU);
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(GameConstants.LevelBuildIndices.START_SCREEN));
        });
        next_button.onClick.AddListener(() =>
        {
            StaticMemory.CurrentGameplayLevelBuildIndex += 1;
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(StaticMemory.CurrentGameplayLevelBuildIndex));
        });
        replay_button.onClick.AddListener(() =>
        {
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(StaticMemory.CurrentGameplayLevelBuildIndex));
        });
    }
    public void LoadStatsValues()
    {
        high_score_value_text.text = StaticMemory.HighScore.ToString();
        coins_collected_value_text.text = StaticMemory.CollectedCoinCount.ToString();
        star_collected_value_text.text = StaticMemory.CollectedStarCount.ToString();
        your_score_value_text.text = StaticMemory.CurrentScore.ToString();
    }
}
