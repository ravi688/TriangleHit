using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;

public class LevelLoadManager : IManualUpdatable
{
    public bool IsRunning { get { return black_image_fade_controller.IsRunning; } }

    private Image black_image_prefab;
    private Image black_image;
    private float fade_duration;
    private AlphaAdapter<Image> black_image_alpha_adapter;
    private FadeController black_image_fade_controller;
    private bool is_loaded = false;
    public void LoadBlackImage()
    {
        if (is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("BlackImage is already loaded, but you are still trying to load it");
#endif
            return;
        }
        black_image_prefab = GameManager.LoadResource<Image>(GameConstants.ResourceFilePaths.BLACK_IMAGE);
        is_loaded = true;
    }
    public void UnloadBlackImage()
    {
        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("BlackImage is not loaded, but you are still trying to unload it");
#endif
            return;
        }
        GameManager.UnloadResource<Image>(black_image_prefab);
        black_image_prefab = null; //zero reference count
        is_loaded = false;
    }
    public LevelLoadManager()
    {
        Initialize();
    }
    public void Initialize()
    {
        SceneManager.activeSceneChanged += (current, next) => GameLogic.OnPostAwakeLoadLevel(StaticMemory.CurrentLevelBuildIndex);
        
        fade_duration = GameConstants.Config.BLACK_FADE_DURATION;
        black_image_alpha_adapter = new ImageAlphaAdapter();
        black_image_fade_controller = new FadeController(black_image_alpha_adapter, fade_duration);
    }
    private void RecoverData()
    {
        Transform transform;
        Canvas canvas = GameManager.GetCanvas();
        transform = UnityEngine.Object.Instantiate(black_image_prefab).transform;
        transform.SetParent(canvas.transform);
        black_image = transform.GetComponent<Image>();
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        black_image_alpha_adapter.SetTarget(black_image);
    }

    //must be called when the active scene is changed
    public void HideScene(Action on_fade_in = null)
    {
        black_image_fade_controller.OnFadeOut = null;
        black_image_fade_controller.InstantFadeOut();
        black_image.gameObject.SetActive(true);
        black_image_fade_controller.OnFadeIn = on_fade_in;
        black_image_fade_controller.FadeIn();
    }
    public void RevealScene()
    {
        if (black_image == null)
            RecoverData();
        black_image.gameObject.SetActive(true);

        black_image_fade_controller.OnFadeIn = null;
        black_image_fade_controller.InstantFadeIn();
        black_image_fade_controller.OnFadeOut = () => { black_image.gameObject.SetActive(false); };
        black_image_fade_controller.FadeOut();
    }

    public void LoadLevel(int BuildIndex)
    {
        GameLogic.OnPreLoadLevel(BuildIndex);
        HideScene(() =>
        {
            GameLogic.OnPostLoadLevel(BuildIndex);
            SceneManager.LoadScene(BuildIndex);
            StaticMemory.CurrentLevelBuildIndex = BuildIndex;
        });
    }

    public void Update()
    {
        black_image_fade_controller.Update();
    }
}