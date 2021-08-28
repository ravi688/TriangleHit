
using UnityEngine; 


public class BindableEntity
{
    public bool is_overlay_touch;
#if ANDROID
    public TouchEvent<BindableEntity> input_event;
#endif
    public Collider2D collider;
    public Transform transform;
    public IBindable bindable;
}