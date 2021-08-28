using UnityEngine;
using System;

public class TVScreenPage : Menu
{
    [SerializeField]
    protected TVScreenPage next_page;

    protected override void Awake()
    {
        base.Awake();
        base.OnActive = OnActive;
        base.OnInactive = OnInactive;
    }

    protected new virtual void OnActive() { }
    protected new virtual void OnInactive() { }
}


//using UnityEngine;
//using System.Collections;
//using System;

//public class TVScreenPage : MonoBehaviour
//{
//    [SerializeField]
//    private StandardAnimation show_animation;
//    [SerializeField]
//    private StandardAnimation hide_animation;

//    public TVScreenPage next_page;


//    public bool IsActive
//    {
//        get
//        {
//            return gameObject.activeSelf;
//        }
//        set
//        {
//            if (value == true && !IsActive)
//            {
//                // if (!_is_initialized)
//                //     Initialize();
//                gameObject.SetActive(true);
//                _anim_player.InitializeWith(_alpha_adapter, transform, show_animation);
//                _anim_player.PlayAtPosition(transform.localPosition, Vector3.zero, Quaternion.identity);
//                // _anim_player.OnEnd = null ; 
//                _anim_player.OnEnd += OnActivate;
//                _anim_player.OnEnd += OnActivateHandler;
//            }
//            else if (value == false && IsActive)
//            {
//                //   if (!_is_initialized)
//                //       Initialize(); 
//                _anim_player.InitializeWith(_alpha_adapter, transform, hide_animation);
//                _anim_player.PlayAtPosition(transform.localPosition, Vector3.zero, Quaternion.identity);
//                _anim_player.OnEnd = delegate
//                {
//                    gameObject.SetActive(false);
//                    ShowNextPage();
//                    if (OnDeactivateHandler != null)
//                        OnDeactivateHandler();
//                    OnDeactivate();
//                };
//            }
//        }
//    }
//    private StandardAnimationPlayer _anim_player;
//    private CanvasGroup _alpha_renderer;
//    private AlphaAdapter _alpha_adapter;
//    public event Action OnActivateHandler;
//    public event Action OnDeactivateHandler;
//    private void Awake()
//    {
//        _alpha_renderer = GetComponent<CanvasGroup>();
//        _anim_player = new StandardAnimationPlayer();
//        _alpha_adapter = new CanvasGroupAlphaAdapter(_alpha_renderer);
//        OnAwake(); 
//    }

//    private void Update()
//    {
//        if (_anim_player.IsRunning)
//            _anim_player.Update();
//        OnUpdate();
//    }
//    protected virtual void OnAwake() { }
//    protected virtual void OnUpdate() { }
//    protected virtual void OnDeactivate() { }
//    protected virtual void OnActivate() { }
//    protected void ShowNextPage()
//    {
//        if (next_page != null)
//            next_page.IsActive = true;

//    }

//}
