using UnityEngine;
using System.Collections.Generic;

public class PointManager : IManualUpdatable
{
    public bool IsRunning { get { return true; } }
    public Sprite PointSprite
#if !RELEASE_MODE
    { get; set; }
#else 
        ;
#endif

    private int pointable_entity_count = 0;
    private int active_pointer_count = 0;
    private float sqr_bindable_distance;
    private List<PointableEntity> pointableEntities
#if !RELEASE_MODE
    {
        get
        {
            if (pointable_entities == null)
                pointable_entities = new List<PointableEntity>();
            return pointable_entities;
        }
        set
        {
            pointable_entities = value;
        }
    }
    private List<PointableEntity> pointable_entities;
#else
        ;
#endif
    private List<Pointer> active_pointers
#if !RELEASE_MODE
    {
        get
        {
            if (_active_pointers == null)
                _active_pointers = new List<Pointer>();
            return _active_pointers;
        }
        set { _active_pointers = value; }
    }
    private List<Pointer> _active_pointers;
#else
        ;
#endif
    private Queue<Pointer> inactive_pointers        //pool of pointers
#if !RELEASE_MODE
    {
        get
        {
            if (_inactive_pointers == null)
                _inactive_pointers = new Queue<Pointer>();
            return _inactive_pointers;
        }
        set
        {
            _inactive_pointers = value;
        }
    }
    private Queue<Pointer> _inactive_pointers;  
#else
        ;
#endif
    private bool is_loaded = false;
    public void ReturnPointerToPool(Pointer pointer)
    {
        inactive_pointers.Enqueue(pointer);
    }
    private Pointer GetPointerFromPool()
    {
        Pointer pointer;
        if (inactive_pointers.Count == 0)
        {
            pointer = new Pointer(PointSprite, 0.2f, true);
            pointer.OnHide = () => { ReturnPointerToPool(pointer); active_pointer_count--; };
        }
        else
            pointer = inactive_pointers.Dequeue();
        active_pointer_count++;
        return pointer;
    }

    public void UnregisterPointable(IPointable pointable)
    {
        int pointer_count = active_pointers.Count;
        for (int i = 0; i < pointableEntities.Count; i++)
            if (pointableEntities[i].pointable == pointable)
            {
                for (int k = 0; k < pointer_count; k++)
                    if (active_pointers[k].IsPoints(pointableEntities[i].pointable))
                    {
                        ReturnPointerToPool(active_pointers[k]);
                        active_pointers.RemoveAt(k);
                        break;
                    }
                pointableEntities.RemoveAt(i);
                pointable_entity_count = pointableEntities.Count;
                break;
            }

    }

    public void RegisterPointable(IPointable pointable)
    {
        PointableEntity entity = new PointableEntity();
        entity.upward_offset = (pointable as MonoBehaviour).GetComponent<Collider2D>().bounds.extents.y;
        entity.transform = (pointable as MonoBehaviour).GetComponent<Transform>();
        entity.pointable = pointable;
        entity.bindable = (pointable as MonoBehaviour).GetComponent<IBindable>();
        pointableEntities.Add(entity);
        pointable_entity_count = pointableEntities.Count;
    }

    public void UnloadPointSprite()
    {
        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("PointSprite is not loaded, but you are still trying to unload it");
#endif
            return;
        }
        GameManager.UnloadResource<Sprite>(PointSprite);
        PointSprite = null;
        is_loaded = false;
    }
    public void LoadPointSprite()
    {
        if (is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("PointSprite is already loaded, but you are still trying to load it");
#endif
            return;
        }
        PointSprite = GameManager.LoadResource<Sprite>(GameConstants.ResourceFilePaths.POINT_SPRITE);
        is_loaded = true;
    }

    public void Initialize()
    {
        if (pointableEntities == null)
            pointableEntities = new List<PointableEntity>();
        if (active_pointers == null)
            active_pointers = new List<Pointer>();
        if (inactive_pointers == null)
            inactive_pointers = new Queue<Pointer>();
        sqr_bindable_distance = PlayerCore.BindableRegionDistance * PlayerCore.BindableRegionDistance;
    }

    public void Update()
    {
        for (int i = 0; i < pointable_entity_count; i++)
        {
            PointableEntity entity = pointableEntities[i];
            bool in_range = IsPlayerIsInRange(entity);
            if (!entity.pointable.IsPointed && in_range)
            {
                Point(entity);
                entity.pointable.IsPointed = true;
            }
            else if ((!in_range || entity.bindable.IsBinded) && entity.pointable.IsPointed)
            {
                Unpoint(entity);
                entity.pointable.IsPointed = false;
            }
        }
        for (int i = 0; i < active_pointer_count; i++)
            if (active_pointers[i].IsRunning)
                active_pointers[i].Update();
    }

    private void Unpoint(PointableEntity entity)
    {
        foreach (Pointer pointer in active_pointers)
            if (pointer.IsPoints(entity.pointable))
                pointer.Hide();
    }
    private void Point(PointableEntity entity)
    {
        if ((entity.bindable == null) || !entity.bindable.IsBinded)
        {
            Pointer pointer = GetPointerFromPool();
            pointer.SetTarget(entity.transform);
            pointer.SetSortingLayer(entity.transform.GetComponentInChildren<SpriteRenderer>().sortingLayerID, 0);
            active_pointers.Add(pointer);
            pointer.Show(Vector3.up * (entity.upward_offset + entity.pointable.PointOffset));
        }
    }
    private bool IsPlayerIsInRange(PointableEntity entity)
    {
        return PlayerCore.instance != null && (PlayerCore.instance.transform.position - entity.transform.position).sqrMagnitude < sqr_bindable_distance;
    }
}