using UnityEngine;
using System.Collections.Generic;
public class BindManager
#if WINDOWS
    : IManualUpdatable
#endif
{
#if WINDOWS
    public bool IsRunning { get { return bindableEntities.Count != 0; } }
#endif
#if WINDOWS
    private int bindable_entity_count = 0;
#endif
    private List<BindableEntity> bindableEntities
#if !RELEASE_MODE
    {
        get
        {
            //if (bindable_entites == null)
            //    bindable_entites = new List<BindableEntity>();
            return bindable_entites;
        }
        set { bindable_entites = value; }
    }
    private List<BindableEntity> bindable_entites;
#else
;
#endif

    public void UnregisterBindable(IBindable bindable)
    {
        for (int i = 0; i < bindableEntities.Count; i++)
            if (bindableEntities[i].bindable == bindable)
            {
#if ANDROID
                if (bindableEntities[i].input_event != null)
                    GameManager.GetTouchManager().UnregisterEvent(bindableEntities[i].input_event);
#endif
                while (bindable.BindedObjects.Count > 0)
                    GameManager.GetRopeManager().UnBind(bindable, bindable.BindedObjects[0]);
                bindableEntities.RemoveAt(i);
#if WINDOWS
                bindable_entity_count = bindableEntities.Count;
#endif
                break;
            }

    }

    public void RegisterBindable(IBindable bindable, int touch_id)
    {
        BindableEntity entity = new BindableEntity();
        entity.collider = (bindable as MonoBehaviour).GetComponent<Collider2D>();
        entity.transform = (bindable as MonoBehaviour).GetComponent<Transform>();
        entity.bindable = bindable;
#if ANDROID
        entity.input_event = new TouchEvent<BindableEntity>(entity);
        entity.input_event.LayerID = touch_id;
        entity.input_event.condition = IsInBounds;
        entity.input_event.on_began = HandleBindAndUnBind;
        GameManager.GetTouchManager().RegisterEvent(entity.input_event);
#endif
        bindableEntities.Add(entity);
#if WINDOWS
        bindable_entity_count  = bindableEntities.Count;
#endif
    }
    public void Initialize()
    {
        bindableEntities = new List<BindableEntity>();
    }

#if WINDOWS
    public void Update()
    {
        for (int i = 0; i < bindable_entity_count; i++)
            HandleInputForEntity(bindableEntities[i]);
    }

    private void HandleInputForEntity(BindableEntity entity)
    {
        if (Input.GetMouseButtonDown(0) && Conditions.IsInBounds(entity.collider.bounds, entity.transform.position, GameManager.GetCamera().ScreenToWorldPoint(Input.mousePosition)))
            HandleBindAndUnBind(entity);
        //CDebug.DrawBounds(entity.collider.bounds, entity.transform.position, Color.white); 
    }
#endif
    private void HandleBindAndUnBind(BindableEntity entity)
    {
        if (!PlayerCore.instance && !entity.bindable.IsBinded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("There is no player at all");
#endif
            return;
        }
        if (!entity.bindable.IsBinded && (entity.transform.position - PlayerCore.instance.transform.position).sqrMagnitude < PlayerCore.BindableRegionDistance * PlayerCore.BindableRegionDistance)
            GameManager.GetRopeManager().Bind(entity.bindable, PlayerCore.instance.GetComponent<IBindable>());
        else
            GameManager.GetRopeManager().UnBind(entity.bindable, PlayerCore.instance.GetComponent<IBindable>());
    }
#if ANDROID
    private bool IsInBounds(Vector2 pos, BindableEntity entity)
    {
        pos = GameManager.GetCamera().ScreenToWorldPoint(pos + DeviceUtility.screen_size_half);
        return Conditions.IsInBounds(entity.collider.bounds, entity.transform.position, pos);
    }
#endif
}