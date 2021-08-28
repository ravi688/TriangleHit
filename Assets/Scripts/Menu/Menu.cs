#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using System;


public class Menu : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        name = gameObject.name;
    }
#endif

    public string Name { get { return name; } }
    [Header("Menu Config")]
    [SerializeField]
    private new string name;
    public bool IsActivateOnAwake = true;
    public bool IsAnimation = false;
    [SerializeField]
    private StandardAnimation show_anim;
    [SerializeField]
    private StandardAnimation hide_anim;

    protected Action OnActive;
    protected Action OnInactive;
    protected Action OnActiveCall;
    protected Action OnInactiveCall;

    private StandardAnimationPlayer anim_player;
    private AlphaAdapter alpha_adapter;
    private Transform root;
    public bool IsActivated
#if !RELEASE_MODE
    {
        get;
        private set;
    }
#else
        ;
#endif
    public void Activate(Action on_end = null)
    {
        root.gameObject.SetActive(true);
        anim_player.OnEnd = on_end;
        if (OnActive != null)
            anim_player.OnEnd += OnActive;
        anim_player.InitializeWith(alpha_adapter, root, show_anim);
        anim_player.PlayAtPosition(root.localPosition, root.localScale, root.rotation);
        IsActivated = true;
        if (OnActiveCall != null)
            OnActiveCall();
    }
    public void Deactivate(Action on_end = null)
    {
        anim_player.OnEnd = () => { root.gameObject.SetActive(false); IsActivated = false; };
        if (on_end != null)
            anim_player.OnEnd += on_end;
        if (OnInactive != null)
            anim_player.OnEnd += OnInactive;
        anim_player.InitializeWith(alpha_adapter, root, hide_anim);
        anim_player.PlayAtPosition(root.localPosition, root.localScale, root.rotation);
        if (OnInactiveCall != null)
            OnInactiveCall();
    }
    public void ActivateWithoutTransition()
    {
        if (alpha_adapter != null)
            alpha_adapter.SetAlpha(1);
        root.gameObject.SetActive(true);
        IsActivated = true;
    }
    public void DeactivateWithoutTransition()
    {
        if (alpha_adapter != null)
            alpha_adapter.SetAlpha(0);
        root.gameObject.SetActive(false);
        IsActivated = false;
    }
    protected virtual void Awake()
    {
        root = transform.GetChild(0);
        if (show_anim.IsAnimationAlpha || hide_anim.IsAnimationAlpha)
            if (root.GetComponent<CanvasGroup>() == null)
                alpha_adapter = new CanvasGroupAlphaAdapter(root.gameObject.AddComponent<CanvasGroup>());
            else
                alpha_adapter = new CanvasGroupAlphaAdapter(root.GetComponent<CanvasGroup>());
        anim_player = new StandardAnimationPlayer();
        anim_player.IsUnscaledTime = true;
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Added: " + this.name);
#endif
        GameManager.GetMenuManager().Menus.Add(this);
        show_anim.AnimationDuration = GameConstants.Config.MENU_FADE_DURATION;
        hide_anim.AnimationDuration = GameConstants.Config.MENU_FADE_DURATION;
    }
    protected virtual void Start()
    {
        if (IsActivateOnAwake)
            if (IsAnimation) Activate(); else ActivateWithoutTransition();
        else
          if (IsAnimation) Deactivate(); else DeactivateWithoutTransition();
    }
    protected virtual void OnDestroy()
    {
#if DEBUG_MODE
        GameManager.GetLogManager().Log("Removed: " + this.name);
#endif
        GameManager.GetMenuManager().Menus.Remove(this);
    }
    protected virtual void Update()
    {
        if (IsActivated)
            GameLoop.Update(anim_player);
    }
}