#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelMenu : Menu
{

    [SerializeField]
    private Button BackButton;
    [SerializeField]
    private Transform level_card_container;
    [SerializeField]
    private RectTransform scroll_rect;

    private ScrollingSystem scroll_menu;
    private int level_card_count;
    private List<LevelCard<int>> level_cards;
    Level[] levels;
    protected override void Start()
    {
        base.Start();
#if DEBUG_MODE
        GameManager.GetLogManager().Log("LevelMenu::Start is called");
#endif
        BackButton.onClick.AddListener(() =>
        {
            Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.START_MENU).Activate());
        });
        SetupLeveCards();
        OnActive += () =>
        {
            for(int i = 0; i < level_card_count; i++)
                level_cards[i].IsRunning = true;
        };
        OnActiveCall += () => scroll_menu.IsRunning = true;
        OnInactive += () => scroll_menu.IsRunning = false;
        OnInactiveCall += () =>
        {
            for (int i = 0; i < level_card_count; i++)
                level_cards[i].IsRunning = false;
        };
    }

    protected override void OnDestroy()
    {
        GameManager.UnloadResourcesAll<Level>(levels);
        levels = null;  //zero reference count;
    }
    private void SetupLeveCards()
    {
        levels = GameManager.GetLevels();
        Array.Sort(levels, (left, right) => { return left.BuildIndex - right.BuildIndex; });
        PurchaseManager purchase_manager = GameManager.GetPurchaseManager();
        level_card_count = levels.Length;

#if DEBUG_MODE
        GameManager.GetLogManager().Log("Level Card Count: " + level_card_count.ToString());
#endif
        level_cards = new List<LevelCard<int>>(level_card_count);
        GameObject level_card_prefab = GameManager.LoadResource<GameObject>(GameConstants.ResourceFilePaths.LEVEL_CARD);
        for (int i = 0; i < level_card_count; i++)
        {
            Transform child_transform = Instantiate(level_card_prefab).GetComponent<Transform>();
            child_transform.SetParent(level_card_container);
            LevelCard<int> card = new LevelCard<int>(child_transform);
            card.args = GameConstants.LevelBuildIndices.LEVEL0 + i;
            card.OnClick = (build_index) =>
            {
                if (!card.IsLocked)
                {
                    StaticMemory.CurrentLevelBuildIndex = build_index;
                    StaticMemory.CurrentGameplayLevelBuildIndex = build_index;
                    Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.PLAYER_SELECTION_MENU).Activate());
                }
                else
                {
#if DEBUG_MODE
                    GameManager.GetLogManager().LogWarning("Level is locked! Cost is " + levels[build_index - GameConstants.LevelBuildIndices.LEVEL0].Cost);
#endif
                }
            };

            Level level = levels[i];
            card.Thumbnail = level.ThumbnailSprite;
            card.IsLocked = !purchase_manager.IsPurchased(level);
            level_cards.Add(card);
        }
        scroll_menu = new ScrollingSystem(scroll_rect, level_card_container, cardPixelSize: 300, offset: 50,
                                  sensitivity: 1, damping: 2, clampStiffness: 10, deactivateOffset: 110, axis: ScrollAxis.Horizontal);
    }

    protected override void Update()
    {
        base.Update();
        if (!IsActivated)
            return;
        for (int i = 0; i < level_card_count; i++)
            GameLoop.Update(level_cards[i]);
        GameLoop.Update(scroll_menu);
    }

}
