
#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesMenu : Menu
{
    [Header("Buttons")]
    [SerializeField]
    private Button back_button;
    [SerializeField]
    private Button boost0_button;       //for duration
    [SerializeField]
    private Button boost1_button;       //for frequency
    [SerializeField]
    private Button boost2_button;       //for value; like health-credit, live-credit, shield-duration-credit etc

    [Header("Config")]
    [SerializeField]
    private RectTransform scroll_rect;
    [SerializeField]
    private Transform container;
    [SerializeField]
    private Image preview;

    [Header("Preview Sprites")]
    [SerializeField]
    private Sprite preview_coin_sprite;
    [SerializeField]
    private Sprite preview_hurt_sprite;
    [SerializeField]
    private Sprite preview_medipack_sprite;
    [SerializeField]
    private Sprite preview_shield_sprite;
    [SerializeField]
    private Sprite preview_star_sprite;

    [Header("Dialogue Texts")]
    [SerializeField, Multiline]
    private string dialoge_coin_text;
    [SerializeField, Multiline]
    private string dialoge_hurt_text;
    [SerializeField, Multiline]
    private string dialoge_medipack_text;
    [SerializeField, Multiline]
    private string dialoge_shield_text;
    [SerializeField, Multiline]
    private string dialoge_star_text;

    private ScrollingSystem scrolling_system;
    private List<Clickable<PowerUpType>> powerup_cards;
    private int powerup_card_count;
    private PurchaseManager purchase_manager;
    private UpgradesManager upgrade_manager;


    private Dictionary<PowerUpType, PowerUpUpgradesMask> upgrade_buttons_mask;
    private PowerUpType binded_powerup_type;

    protected override void Start()
    {
        base.Start();
        purchase_manager = GameManager.GetPurchaseManager();
        upgrade_manager = GameManager.GetUpgradesManager();
        purchase_manager.Load(); //for latest updates
        upgrade_manager.Load(); //for latest updates, NOTE: upgrade manager loads some resources if needed so always unload resource by calling UnloadResources method
        powerup_card_count = container.childCount;
        SetupPowerUpsUpgradesMask();
        SetupButtonCallbacks();
        SetupPowerUpCards();
        scrolling_system = new ScrollingSystem(scroll_rect, container, cardPixelSize: 150, offset: 50,
            damping: 2, deactivateOffset: 100, axis: ScrollAxis.Vertical);
        OnActiveCall += () => scrolling_system.IsRunning = true;
        OnInactive += () => scrolling_system.IsRunning = false;
        Bind(PowerUpType.Medipack);
    }

    private void SetupPowerUpsUpgradesMask()
    {
        upgrade_buttons_mask = new Dictionary<PowerUpType, PowerUpUpgradesMask>(powerup_card_count);
        upgrade_buttons_mask.Add(PowerUpType.Coin, upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Coin));
        upgrade_buttons_mask.Add(PowerUpType.Star, upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Star));
        upgrade_buttons_mask.Add(PowerUpType.Shield, upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Shield));
        upgrade_buttons_mask.Add(PowerUpType.Hurt, upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Hurt));
        upgrade_buttons_mask.Add(PowerUpType.Medipack, upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Medipack));
    }
    private void UpdatePowerUpsUpgradesMask()
    {
        upgrade_buttons_mask[PowerUpType.Coin] = upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Coin);
        upgrade_buttons_mask[PowerUpType.Star] = upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Star);
        upgrade_buttons_mask[PowerUpType.Shield] = upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Shield);
        upgrade_buttons_mask[PowerUpType.Hurt] = upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Hurt);
        upgrade_buttons_mask[PowerUpType.Medipack] = upgrade_manager.GetPowerUpUpgradesMask(PowerUpType.Medipack);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        upgrade_manager.Save();
        purchase_manager.Save();
        upgrade_manager.UnloadResources();
    }
    protected override void Update()
    {
        base.Update();
        if (!IsActivated)
            return;
        GameLoop.Update(scrolling_system);
        for (int i = 0; i < powerup_card_count; i++)
            GameLoop.Update(powerup_cards[i]);
    }

    private void SetupPowerUpCards()
    {
        powerup_cards = new List<Clickable<PowerUpType>>(powerup_card_count);
        string[] powerup_names = new string[]
        {
            //Order Matters
            //Because
            /* 
             * PowerUpType
             * 
             * Coin = 0
             * Hurt = 1
             * Medipack = 2
             * Shield = 3
             * Star = 4
             */
            GameConstants.PowerUpNames.COIN,
            GameConstants.PowerUpNames.HURT,
            GameConstants.PowerUpNames.MEDIPACK,
            GameConstants.PowerUpNames.SHIELD,
            GameConstants.PowerUpNames.STAR
        };
        for (int i = 0; i < powerup_card_count; i++)
        {
            //transform.name : this name must be one of GameConstants.PowerUpNames.MEDIPACK ...
            Transform transform = container.GetChild(i);
            Clickable<PowerUpType> card = new Clickable<PowerUpType>(transform);
            int index = Array.FindIndex<string>(powerup_names, (name) => { return name == transform.name; });
#if DEBUG_MODE
            if (index == -1)
                GameManager.GetLogManager().LogError(string.Format("Could not find the PowerUp Name {0} in GameConstants.PowerUpNames", transform.name));
#endif
            card.args = (PowerUpType)index;
            card.OnClick = (PowerUpType powerup_type) =>
            {
                Bind(powerup_type);
            };
            powerup_cards.Add(card);
        }
    }
    private void Bind(PowerUpType powerup_type)
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log(string.Format("Binded PowerUp : {0}", powerup_type.ToString()));
#endif
        ShowPreviewImage(powerup_type);
        ActivatedAppropriateBoostButtons(powerup_type);
        binded_powerup_type = powerup_type;
    }

    private void ActivatedAppropriateBoostButtons(PowerUpType powerup_type)
    {
        PowerUpUpgradesMask mask = upgrade_buttons_mask[powerup_type];
        boost0_button.gameObject.SetActive((mask & PowerUpUpgradesMask.DURATION) == PowerUpUpgradesMask.DURATION);
        boost1_button.gameObject.SetActive((mask & PowerUpUpgradesMask.FREQUENCY) == PowerUpUpgradesMask.FREQUENCY);
        boost2_button.gameObject.SetActive((mask & PowerUpUpgradesMask.VALUE) == PowerUpUpgradesMask.VALUE);
    }

    private void ShowPreviewImage(PowerUpType powerup_type)
    {
        switch (powerup_type)
        {
            case PowerUpType.Medipack:
                preview.sprite = preview_medipack_sprite;
                break;
            case PowerUpType.Coin:
                preview.sprite = preview_coin_sprite;
                break;
            case PowerUpType.Hurt:
                preview.sprite = preview_hurt_sprite;
                break;
            case PowerUpType.Shield:
                preview.sprite = preview_shield_sprite;
                break;
            case PowerUpType.Star:
                preview.sprite = preview_star_sprite;
                break;
#if DEBUG_MODE
            default:
                GameManager.GetLogManager().LogError(string.Format("There is no preview image with name {0}", name));
                break;
#endif
        }
    }
    private void SetupButtonCallbacks()
    {
        back_button.onClick.AddListener(() => Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.START_MENU).Activate()));
        boost0_button.onClick.AddListener(() =>
        {
            upgrade_manager.UpgradePowerUp(binded_powerup_type, PowerUpUpgradeType.Duration);
            UpdatePowerUpsUpgradesMask();
            ActivatedAppropriateBoostButtons(binded_powerup_type);
        });
        boost1_button.onClick.AddListener(() =>
        {
            upgrade_manager.UpgradePowerUp(binded_powerup_type, PowerUpUpgradeType.Frequency);
            UpdatePowerUpsUpgradesMask();
            ActivatedAppropriateBoostButtons(binded_powerup_type);
        });
        boost2_button.onClick.AddListener(() =>
        {
            upgrade_manager.UpgradePowerUp(binded_powerup_type, PowerUpUpgradeType.Value);
            UpdatePowerUpsUpgradesMask();
            ActivatedAppropriateBoostButtons(binded_powerup_type);
        });
    }
}
