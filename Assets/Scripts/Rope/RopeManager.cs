
using UnityEngine;
using System.Collections.Generic;
using System;

public class RopeManager : IManualUpdatable
{
    public bool IsRunning { get { return active_ropes.Count != 0; } }
    public RopeSettings RopeSettings
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif

    private int active_rope_count = 0;
    private List<Rope> active_ropes       //TODO: replace the list with ObjectPool<Rope> something like that to improve the performance
#if !RELEASE_MODE
    {
        get
        {
            if (_active_ropes == null)
                _active_ropes = new List<Rope>();
            return _active_ropes;
        }
        set { _active_ropes = value; }
    }
    private List<Rope> _active_ropes;
#else
        ;
#endif
    private Queue<Rope> inactive_ropes  //Pool of ropes
#if !RELEASE_MODE
    {
        get
        {
            if (_inactive_ropes == null)
                _inactive_ropes = new Queue<Rope>();
            return _inactive_ropes;
        }
        set
        {
            _inactive_ropes = value; 
        }
    }
    private Queue<Rope> _inactive_ropes;  //Pool of ropes
#else
;
#endif
    private bool is_loaded = false;
    private void ReturnRopeToPool(Rope rope)
    {
        inactive_ropes.Enqueue(rope);
        active_rope_count--;
    }

    private Rope GetRopeFromPool()
    {
        Rope rope;
        if (inactive_ropes.Count == 0)
        {
            rope = new Rope(RopeSettings);
            rope.OnHide = () => ReturnRopeToPool(rope);
        }
        else
            rope = inactive_ropes.Dequeue();
        active_rope_count++;
        return rope;
    }
    public void UnloadRopeSettings()
    {
        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("RopeSettings is not loaded, but you are still trying to unload it");
#endif
            return;
        }
        GameManager.UnloadResource<RopeSettings>(RopeSettings);
        RopeSettings = null;
        is_loaded = false;
    }
    public void LoadRopeSettings()
    {
        if (is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("RopeSettings is already loaded, but you are still trying to load it");
#endif
            return;
        }
        RopeSettings = GameManager.LoadResource<RopeSettings>(GameConstants.ResourceFilePaths.ROPE_SETTINGS);
        is_loaded = true;
    }
    public void Initialize()
    {
            active_ropes = new List<Rope>();
            inactive_ropes = new Queue<Rope>();
    }

    //Must be called in LateUpdate
    public void Update()
    {
        for(int i = 0; i < active_rope_count; i++)
            GameLoop.Update(active_ropes[i]);
    }
    private void OnDestroy()
    {
        for(int i = 0; i < active_rope_count; i++)
            active_ropes[i].OnDestroy();
    }
    public void UnBind(IBindable object1, IBindable object2)
    {
        for (int i = 0; i < active_rope_count; i++)
            if (active_ropes[i].IsBinds(object1, object2))
            {
                active_ropes[i].UnBind();
                object1.BindedObjects.Remove(object2);
                object2.BindedObjects.Remove(object1);
            }
    }

    public void Bind(IBindable object1, IBindable object2)
    {
        if (object1 == object2)
            return;
        if (object1.BindedObjects.Contains(object2))
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log(string.Format("Object {0} is already binded with Object {1}", (object1 as MonoBehaviour).name, (object2 as MonoBehaviour).name));
#endif
            return;
        }
        Rope rope = GetRopeFromPool();
        Rigidbody2D body1 = (object1 as MonoBehaviour).GetComponent<Rigidbody2D>();
        Rigidbody2D body2 = (object2 as MonoBehaviour).GetComponent<Rigidbody2D>();
        if (body1 == null)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log(string.Format("Object {0} doesn't have Rigidbody2D component attached", (object1 as MonoBehaviour).name));
#endif
            return;
        }
        if (body2 == null)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().Log(string.Format("Object {0} doesn't have Rigidbody2D component attached", (object2 as MonoBehaviour).name));
#endif
            return;
        }

        rope.Bind(body1, body2);
        active_ropes.Add(rope);

        object1.BindedObjects.Add(object2);
        object2.BindedObjects.Add(object1);
    }
}