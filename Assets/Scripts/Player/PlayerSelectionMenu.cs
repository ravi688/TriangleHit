#if DISABLE_WARNINGS
#pragma warning disable
#endif


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PlayerSelectionMenu : Menu
{
    [Header("Objects")]
    [SerializeField]
    private RectTransform scroll_rect;
    [SerializeField]
    private Transform player_container;
    [SerializeField]
    private Transform preview_image_transform;
    [SerializeField]
    private GameObject lock_object;

    [Header("Buttons")]
    [SerializeField]
    private Button select_button;
    [SerializeField]
    private Button purchase_button;
    [SerializeField]
    private Button back_button;

    [Header("Sliders")]
    [SerializeField]
    private Slider thrust_recovery_rate_slider;
    [SerializeField]
    private Slider energy_consumption_rate_slider;
    [SerializeField]
    private Slider max_hit_amount_slider;
    [SerializeField]
    private Slider max_live_count_slider;

    [Header("Config")]
    [SerializeField]
    private float angular_speed;

    private ScrollingSystem scroll_menu;
    private List<Clickable<int>> player_cards;
    private int player_count;
    private Sprite[] preview_sprites;
    private Sprite[] scroll_menu_sprites;
    private PlayerCoreSettings[] players;
    private int current_player_index;
    private bool[] purchased_mask;
    private bool is_rotate;
    protected override void Start()
    {
        base.Start();
        is_rotate = false;
        //Order Matters
        LoadResources();
        LoadPersistentData();

        SetupPlayerScrollMenu();
        SetupButtonCallback();

        //Order Matters
        SetupPurchasedMask();
        OnActiveCall += () =>
        {
            ShowPlayer(current_player_index,
            Array.FindIndex(preview_sprites, (sprite) => { return sprite.name.Equals(players[current_player_index].name); }));
            is_rotate = true;
        };
        OnActive += () => scroll_menu.IsRunning = true;
        OnInactiveCall += () => scroll_menu.IsRunning = false;
        OnInactive += () => is_rotate = false;
    }

    private void LoadPersistentData()
    {
        PlayerManager player_manager = GameManager.GetPlayerManager();
        player_manager.Load();      //for latest updates
#if DEBUG_MODE
         GameManager.GetLogManager().Log(player_manager.PlayerName);
#endif
        current_player_index = Array.FindIndex(players, (x) => { return x.name.Equals(player_manager.PlayerName); });
#if DEBUG_MODE
        if (current_player_index == -1)
            GameManager.GetLogManager().LogError("current_player_index == -1");
#endif
    }

    private void ShowLock()
    {
        if (!lock_object.activeSelf)
            lock_object.SetActive(true);
    }
    private void HideLock()
    {
        if (lock_object.activeSelf)
            lock_object.SetActive(false);
    }
    private void ShowPurchaseButton()
    {
        if (select_button.gameObject.activeSelf)
            select_button.gameObject.SetActive(false);
        if (!purchase_button.gameObject.activeSelf)
            purchase_button.gameObject.SetActive(true);
    }
    private void ShowSelectButton()
    {
        if (purchase_button.gameObject.activeSelf)
            purchase_button.gameObject.SetActive(false);
        if (!select_button.gameObject.activeSelf)
            select_button.gameObject.SetActive(true);
    }

    private void SetupPurchasedMask()
    {
        purchased_mask = new bool[player_count];
        PurchaseManager purchase_manager = GameManager.GetPurchaseManager();
        purchase_manager.Load(); //for latest updates
        for (int i = 0; i < player_count; i++)
            purchased_mask[i] = purchase_manager.IsPurchased(players[current_player_index]);
    }

    private void SetupButtonCallback()
    {
        back_button.onClick.AddListener(() => Deactivate(() => GameManager.GetMenuManager().GetMenuByName(GameConstants.MenuNames.START_MENU).Activate()));
        purchase_button.onClick.AddListener(() =>
        {
            PurchaseManager purchase_manager = GameManager.GetPurchaseManager();
            purchase_manager.Load();    //for latest updates
            if (!purchased_mask[current_player_index]) //can be replaced with !purchase_manager.IsPurchased(players[current_player_index])
            {
                bool is_purchase_success = purchase_manager.TryPurchase(players[current_player_index]);
                if (!is_purchase_success)
                {
                    //Show some visual warning
#if DEBUG_MODE
                    GameManager.GetLogManager().LogWarning(
                        string.Format("You don't have enough coins to purchase {0}, Coin count: {1}",
                        players[current_player_index].name, GameManager.GetEconomyManager().CoinCount));
#endif
                }
                else
                {
                    //update the purchased mask
                    purchased_mask[current_player_index] = true; //purchased
                    HideLock();
                    ShowSelectButton();
                    GameManager.GetPurchaseManager().Save(); //save the purchase data
                }
            }
        });
        select_button.onClick.AddListener(() =>
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log("Select Clicked");
#endif
            GameManager.GetPlayerManager().PlayerName = players[current_player_index].name;
            //Load the level selected in the level menu, i.e. StaticMemory.CurrentGameplayLevelBuildIndex
            Deactivate(() => GameManager.GetLevelLoadManager().LoadLevel(StaticMemory.CurrentGameplayLevelBuildIndex));
        });
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //Unload the resources on destroy
        GameManager.GetPlayerManager().UnloadAllPlayersSettings();
    }

    private void LoadResources()
    {
        //Load the required resources
        preview_sprites = GameManager.LoadResourceAll<Sprite>(GameConstants.ResourceFilePaths.PLAYERS_NATIVE_SPRITES);
        scroll_menu_sprites = GameManager.LoadResourceAll<Sprite>(GameConstants.ResourceFilePaths.PLAYERS_HALF_SPRITES);
        players = new PlayerCoreSettings[4];
        PlayerManager player_manager = GameManager.GetPlayerManager(); 
        player_manager.LoadAllPlayersSettings();
        players[0] = player_manager.GetPlayerSettings<TrianglePlayerSettings>(GameConstants.PlayerNames.TRIANGLE);
        players[1] = player_manager.GetPlayerSettings<SquarePlayerSettings>(GameConstants.PlayerNames.SQUARE);
        players[2] = player_manager.GetPlayerSettings<PentagonPlayerSettings>(GameConstants.PlayerNames.PENTAGON);
        players[3] = player_manager.GetPlayerSettings<HexagonPlayerSettings>(GameConstants.PlayerNames.HEXAGON);

#if DEBUG_MODE
        GameManager.GetLogManager().Log("Loaded preview_sprites::Count = " + preview_sprites.Length);
        GameManager.GetLogManager().Log("Loaded scroll_menu_sprites::Count = " + scroll_menu_sprites.Length);
        GameManager.GetLogManager().Log("Loaded players::Count = " + players.Length);
        //foreach (PlayerCoreSettings player in players)
        //    GameManager.GetLogManager().Log(player.name);
#endif
    }

    private void SetupPlayerScrollMenu()
    {
#if DEBUG_MODE
        if (preview_sprites.Length != scroll_menu_sprites.Length)
            GameManager.GetLogManager().LogError("PLAYERS_NATIVE_SPRITES::Length != PLAYERS_HALF_SPRITES::Length");
#endif

        player_count = preview_sprites.Length;
        player_cards = new List<Clickable<int>>(player_count);
        for (int i = 0; i < player_count; i++)
        {
            string matched_name = scroll_menu_sprites[i].name;
            int player_index = Array.FindIndex(players, (settings) => { return settings.name.Equals(matched_name); });
            int preview_image_index = Array.FindIndex(preview_sprites, (sprite) => { return sprite.name.Equals(matched_name); });
#if DEBUG_MODE
            if (player_index == -1)
                GameManager.GetLogManager().LogError(string.Format("{0}/{1} is not matched with any PlayerCoreSettings::name", GameConstants.ResourceFilePaths.PLAYERS_HALF_SPRITES, matched_name));
            if (preview_image_index == -1)
                GameManager.GetLogManager().LogError(string.Format("{0}/{1} is not matched with any native sprites in {2}", GameConstants.ResourceFilePaths.PLAYERS_HALF_SPRITES, matched_name, GameConstants.ResourceFilePaths.PLAYERS_NATIVE_SPRITES));
#endif
            Transform child_transform = new GameObject(matched_name).GetComponent<Transform>();
            child_transform.SetParent(player_container);
            child_transform.localPosition = Vector3.zero;
            Image image = child_transform.gameObject.AddComponent<Image>();
            image.sprite = scroll_menu_sprites[i];
            image.SetNativeSize();
       
            Clickable<int> player_card = new Clickable<int>(image.transform, radius: 100);
            player_card.args = player_index;
            player_card.OnClick = (index) =>
            {
                ShowPlayer(index, preview_image_index);
            };
            player_cards.Add(player_card);
        }
        scroll_menu = new ScrollingSystem(scroll_rect, player_container, cardPixelSize: 100, offset: 50,
                                 sensitivity: 1, damping: 2, clampStiffness: 10, deactivateOffset: 110, axis: ScrollAxis.Vertical);
    }

    protected override void Update()
    {
        base.Update();
        if (!IsActivated)
            return;
        GameLoop.Update(scroll_menu);
        for(int i = 0; i < player_count; i++)
            GameLoop.Update(player_cards[i]);
        preview_image_transform.Rotate(Vector3.forward * angular_speed * Time.deltaTime);
    }

    private void ShowPlayer(int player_index, int preview_image_index)
    {
        if (purchased_mask[player_index])
        {
            HideLock();
            ShowSelectButton();
        }
        else
        {
            ShowLock();
            ShowPurchaseButton();
        }
        current_player_index = player_index;
        preview_image_transform.GetComponent<Image>().sprite = scroll_menu_sprites[preview_image_index];
    }

}
