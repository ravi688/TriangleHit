using UnityEngine;
using System;

public abstract class PowerUp : IManualUpdatable
{
    public bool IsRunning { get; private set; }

    public Transform transform { get { return parent; } private set { parent = value; } }
    public static Transform target;

    public Action<bool> OnInActive { get; set; }
    public Action OnActive { get; set; }

    private Transform parent;
    private float detect_radius;

    private StandardAnimation pop_up_anim;
    private StandardAnimation missed_anim;
    private StandardAnimation glowing_anim;

    private StandardAnimationPlayer missed_and_pop_up_anim_player;
    private StandardAnimationPlayer glow_anim_player;

    protected Timer timer;
    private bool is_grabbing;
    private Vector3 position;

    protected PowerUp(PowerUpSettings settings)
    {
        is_grabbing = false;

        glowing_anim = settings.glow_anim;
        missed_anim = settings.missed_anim;
        pop_up_anim = settings.pop_up_anim;
        detect_radius = settings.detect_radius;

        GameObject glow_object = new GameObject("Glow");
        GameObject body_object = new GameObject("Body");
        GameObject parent_object = new GameObject(settings.name);
        glow_object.transform.SetParent(parent_object.transform);
        body_object.transform.SetParent(parent_object.transform);
        transform = parent_object.transform;
        SpriteRenderer glow_renderer = glow_object.AddComponent<SpriteRenderer>();
        glow_renderer.sprite = settings.glow_sprite;
        SpriteRenderer body_renderer = body_object.AddComponent<SpriteRenderer>();
        body_renderer.sprite = settings.body_sprite;
        glow_renderer.sortingLayerName = settings.sorting_layer_name;
        glow_renderer.sortingOrder = settings.sorting_order;
        body_renderer.sortingLayerName = settings.sorting_layer_name;
        body_renderer.sortingOrder = settings.sorting_order + 1;

        AlphaAdapter adapter = new SpriteRendererAlphaAdapter(glow_renderer);
        timer = new Timer(0, settings.duration, 1);

        missed_and_pop_up_anim_player = new StandardAnimationPlayer();
        glow_anim_player = new StandardAnimationPlayer();
        glow_anim_player.InitializeWith(adapter, null, glowing_anim);

        position = transform.position;
        timer.AddListner(
       delegate ()
       {
           missed_and_pop_up_anim_player.InitializeWith(null, transform, missed_anim);
           missed_and_pop_up_anim_player.PlayAtPosition(position, Vector3.zero, Quaternion.identity);
       },
       OnTimer.End
       );

        parent_object.SetActive(false);
        IsRunning = false;
    }
    public void ClearCallBacks()
    {
        OnActive = null;
        OnInActive = null;
    }
    public void Activate(Vector3 position)
    {
        this.position = position;
        transform.position = position;
        missed_and_pop_up_anim_player.InitializeWith(null, transform, pop_up_anim);
        missed_and_pop_up_anim_player.PlayAtPosition(position, Vector3.zero, Quaternion.identity);
        glow_anim_player.PlayAtPosition(Vector3.zero, Vector3.zero, Quaternion.identity);
        timer.Start();
        IsRunning = true;
        is_grabbing = false;
        parent.gameObject.SetActive(true);
        if (OnActive != null) OnActive();
        OnPopUp();
    }
    public void Update()
    {
        GameLoop.Update(missed_and_pop_up_anim_player);
        GameLoop.Update(glow_anim_player);
        GameLoop.Update(timer);

        if (!timer.IsRunning && !missed_and_pop_up_anim_player.IsRunning)
        {
            IsRunning = false;
            parent.gameObject.SetActive(false);
            if (OnInActive != null) OnInActive(false);
            OnMissed();
        }
        bool is_in_range;
        if (target != null)
            is_in_range = (position - target.position).sqrMagnitude < detect_radius * detect_radius;
        else
            is_in_range = false;
        if (!is_grabbing && is_in_range)
        {

            DoPowerUp(target.GetComponent<MonoBehaviour>());
            is_grabbing = true;
        }
        else if (!is_in_range && is_grabbing)
            is_grabbing = false;
    }
    protected abstract bool OnGrabbed(MonoBehaviour entered_object);
    protected abstract void OnPopUp();
    protected abstract void OnMissed();
    private bool DoPowerUp(MonoBehaviour entered_object)
    {
        if (entered_object != null)
        {
            if (!OnGrabbed(entered_object))
                return false;
            if (OnInActive != null) OnInActive(true); //call this only if the target has power upped ,otherwise not
            IsRunning = false;
            parent.gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}

public abstract class PowerUp<T> : PowerUp
{
    public PowerUp(PowerUpSettings settings) : base(settings) { }
    sealed protected override bool OnGrabbed(MonoBehaviour mono) { return OnGrabbed(mono.GetComponent<T>()); }
    //Derived class must override it
    protected abstract bool OnGrabbed(T entered_object);
}
