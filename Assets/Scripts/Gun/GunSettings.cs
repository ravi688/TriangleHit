
using UnityEngine; 

[CreateAssetMenu]
public class GunSettings : ScriptableObject
{
    public Vector2 nozel_offset = new Vector2(2.0f, 0.6f);
    public Sprite hit_sprite;
    public Sprite bullet_sprite;

    public float hit_mark_scale_factor = 1.5f;
    public float bullet_scale_factor = 1.2f;

    public float ray_cast_wait_time = 0.1f;
    public float hit_spark_stay_time = 0.1f;
    public float recoil_duration = 0.15f;
    public float recoil_amplitude = 0.3f;
    public float hit_force = 50.0f;
    public float force_offset = 0.5f;
    public float momenumForCameraShake = 20f; 
    public float damage_amount = 5.0f;
    public float CanDamageAmount = 0.5f;

    public bool isApplyForceOnHit = true;
    public bool isCameraShakeOnHit = true;
    public bool isHideOnAwake = true;
    public bool canDamage = true;
    
}
