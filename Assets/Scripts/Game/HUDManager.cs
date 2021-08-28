#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : IManualUpdatable
{
    public bool IsRunning { get { return true; } }
    public Slider HealthSlider;
    public Slider DashSlider;
    public Sprite LivesMeterSprite;
    public Sprite EnemyDisposeMeterSprite;
    public Text ScoreText;
    public StandardAnimation ScoreTextAnim;
    public StandardAnimation GameOverTextAnim;
    public Image YouLooseTextImage;
    public Image YouWinTextImage;

    private SpriteMeter EnemyDisposeMeter;
    private SpriteMeter PlayerLivesMeter;
    private SliderAutoHideController healthSliderAutoHideController;
    private SliderAutoHideController dashSliderAutoHideController;
    private StandardAnimationPlayer score_text_anim_player;
    private StandardAnimationPlayer game_over_text_anim_player;
    private Vector3 score_text_initial_position;
    private Vector3 score_text_initial_scale;

    private bool is_loaded = false;

    Text score_text_prefab;
    Slider health_bar_prefab;
    Slider dash_bar_prefab;
    Image you_loose_prefab;
    Image you_win_prefab;

    Sprite live_meter_sprite;
    Sprite enemy_dispose_meter_sprite;
    StandardAnimation score_text_anim;
    StandardAnimation game_over_text_anim;

    public void LoadAllHUDElements()
    {
#if DEBUG_MODE
        if (is_loaded)
        {
            GameManager.GetLogManager().LogWarning("All HUD Elements are already loaded, but you are still trying to load those");
            return;
        }
#endif
        score_text_prefab = GameManager.LoadResource<Text>(GameConstants.ResourceFilePaths.SCORE_TEXT);
        health_bar_prefab = GameManager.LoadResource<Slider>(GameConstants.ResourceFilePaths.HEALTH_BAR);
        dash_bar_prefab = GameManager.LoadResource<Slider>(GameConstants.ResourceFilePaths.DASH_BAR);
        you_loose_prefab = GameManager.LoadResource<Image>(GameConstants.ResourceFilePaths.YOU_LOOSE);
        you_win_prefab = GameManager.LoadResource<Image>(GameConstants.ResourceFilePaths.YOU_WIN);
        live_meter_sprite = GameManager.LoadResource<Sprite>(GameConstants.ResourceFilePaths.LIVE_METER_SPRITE);
        enemy_dispose_meter_sprite = GameManager.LoadResource<Sprite>(GameConstants.ResourceFilePaths.ENEMY_DISPOSE_METER_SPRITE);
        score_text_anim = GameManager.LoadResource<StandardAnimation>(GameConstants.ResourceFilePaths.SCORE_TEXT_ANIM);
        game_over_text_anim = GameManager.LoadResource<StandardAnimation>(GameConstants.ResourceFilePaths.GAME_OVER_TEXT_ANIM);
        is_loaded = true;
    }

    public void UnloadAllHUDElements()
    {
#if DEBUG_MODE
        if (!is_loaded)
        {
            GameManager.GetLogManager().LogWarning("All HUD Elements are not loaded, but you are still trying to unload those");
            return;
        }
#endif
        GameManager.UnloadResource<Text>(score_text_prefab);
        GameManager.UnloadResource<Slider>(health_bar_prefab);
        GameManager.UnloadResource<Slider>(dash_bar_prefab);
        GameManager.UnloadResource<Image>(you_loose_prefab);
        GameManager.UnloadResource<Image>(you_win_prefab);
        GameManager.UnloadResource<Sprite>(live_meter_sprite);
        GameManager.UnloadResource<Sprite>(enemy_dispose_meter_sprite);
        GameManager.UnloadResource<StandardAnimation>(score_text_anim);
        GameManager.UnloadResource<StandardAnimation>(game_over_text_anim);

        score_text_prefab = null;
        health_bar_prefab = null;
        dash_bar_prefab = null;
        you_loose_prefab = null;
        you_win_prefab = null;
        live_meter_sprite = null;
        enemy_dispose_meter_sprite = null;
        score_text_anim = null;
        game_over_text_anim = null;

        is_loaded = false;
    }

    public void Initialize()
    {
        Canvas canvas = GameManager.GetCanvas();
        RectTransform rectTransform;
        Transform transform;
        transform = Object.Instantiate(score_text_prefab).transform;
        transform.SetParent(canvas.transform);
        rectTransform = transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector3.zero;
        ScoreText = transform.GetComponent<Text>();

        transform = Object.Instantiate(health_bar_prefab).transform;
        transform.SetParent(canvas.transform);
        HealthSlider = transform.GetComponent<Slider>();
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(490.8f, 359.7f);

        transform = Object.Instantiate(dash_bar_prefab).transform;
        transform.SetParent(canvas.transform);
        DashSlider = transform.GetComponent<Slider>();
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(490.8f, 340.1f);

        YouLooseTextImage = Object.Instantiate(you_loose_prefab).GetComponent<Image>();
        YouLooseTextImage.transform.SetParent(canvas.transform);
        YouLooseTextImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 673);

        YouWinTextImage = Object.Instantiate(you_win_prefab).GetComponent<Image>();
        YouWinTextImage.transform.SetParent(canvas.transform);
        YouWinTextImage.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 673);

        LivesMeterSprite = live_meter_sprite;
        EnemyDisposeMeterSprite = enemy_dispose_meter_sprite;
        ScoreTextAnim = score_text_anim;
        GameOverTextAnim = game_over_text_anim;

        healthSliderAutoHideController = new SliderAutoHideController(HealthSlider);
        dashSliderAutoHideController = new SliderAutoHideController(DashSlider);
        game_over_text_anim_player = new StandardAnimationPlayer();
        score_text_anim_player = new StandardAnimationPlayer();

#warning PlayerCore.MaxLives in HUDmanager 

        PlayerLivesMeter = new SpriteMeter(new Rect(850, 30, 200, 35), 40, LivesMeterSprite, SpriteAlignment.LeftCenter, null, PlayerCore.MaxLives);
        Debug.Log(PlayerCore.MaxLives);
        EnemyDisposeMeter = new SpriteMeter(new Rect(60, 30, 150, 35), 40, EnemyDisposeMeterSprite, SpriteAlignment.LeftCenter, null, StaticMemory.MaxEnemyCount);
        EnemyDisposeMeter.Value = 0;

        score_text_initial_position = ScoreText.transform.localPosition;
        score_text_initial_scale = ScoreText.transform.localScale;
        score_text_anim_player.InitializeWith(null, ScoreText.transform, ScoreTextAnim);
        ScoreText.text = "Score : " + (0).ToString();
    }
    public void Update()
    {
        GameLoop.Update(healthSliderAutoHideController);
        GameLoop.Update(dashSliderAutoHideController);
        GameLoop.Update(game_over_text_anim_player);
        GameLoop.Update(score_text_anim_player);
    }
    public void SetScore(int score)
    {
        score_text_anim_player.PlayAtPosition(score_text_initial_position, score_text_initial_scale, Quaternion.identity);
        ScoreText.text = string.Format("Score : {0}", score);
    }
    public void ShowLoose()
    {
        YouLooseTextImage.gameObject.SetActive(true);
        game_over_text_anim_player.InitializeWith(null, YouLooseTextImage.transform, GameOverTextAnim);
        game_over_text_anim_player.PlayAtPosition(YouLooseTextImage.transform.localPosition, Vector3.zero, Quaternion.identity);
    }
    public void ShowWin()
    {
        YouWinTextImage.gameObject.SetActive(true);
        game_over_text_anim_player.InitializeWith(null, YouWinTextImage.transform, GameOverTextAnim);
        game_over_text_anim_player.PlayAtPosition(YouWinTextImage.transform.localPosition, Vector3.zero, Quaternion.identity);
    }
    public void SetPlayerHealth(float value)
    {
        HealthSlider.value = value;
    }
    public void SetPlayerDashValue(float value)
    {
        DashSlider.value = value;
    }
    public void SetEnemyDesposeMeterValue(int value)
    {
        EnemyDisposeMeter.Value = value;
    }
    public void SetPlayerLivesMeterValue(int value)
    {
        PlayerLivesMeter.Value = value;
    }
}
