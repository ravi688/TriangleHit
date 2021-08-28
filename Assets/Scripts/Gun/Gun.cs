using UnityEngine.Events;
using UnityEngine;



public class Gun : MonoBehaviour
{
    public GunSettings gunSettings;

    public bool IsActive
    {
        get { return this.gameObject.activeSelf; }
        set
        {
            if (value == true && !IsActive)
            {
                this.gameObject.SetActive(true);
                fader.FadeIn();
            }
            if (value == false && IsActive)
            {
                fader.FadeOut();
            }
        }
    }

    private bool isRecoiling = false;
    private bool isBulletFired = false;
    private bool isSomethingHitted = false;
    private float recoil_timing = 0;
    private float timing = 0;
    private Vector2 gun_hold_position;
    private Vector2 aimed_direction;
    private Vector2 final_bullet_pos;
    private Vector2 initial_bullet_pos;
    private FadeController fader;
    private RaycastHit2D hit_info;
    private UnityAction<RaycastHit2D> on_hit;
    [HideInInspector]
    public GameObject bullet;
    [HideInInspector]
    public GameObject hitMark;
    private AlphaAdapter adapter;
    private float inverse_raycast_hit_time;
    private float inverse_recoil_duration;
    private float ray_cast_wait_time;
    private float hit_spark_stay_time;
    private float recoil_amplitude;
    void Awake()
    {
        inverse_raycast_hit_time = (float)1/ gunSettings.ray_cast_wait_time;
        inverse_recoil_duration = (float) 1/ gunSettings.recoil_duration;
        hit_spark_stay_time = gunSettings.hit_spark_stay_time;
        recoil_amplitude = gunSettings.recoil_amplitude;
        ray_cast_wait_time = gunSettings.ray_cast_wait_time;

        adapter = new SpriteRendererAlphaAdapter(GetComponent<SpriteRenderer>());
        if (gunSettings.isHideOnAwake)
        {
            adapter.SetAlpha(0);
            IsActive = false;
        }
        else
        {
            adapter.SetAlpha(1);
            IsActive = true;
        }
        fader = new FadeController(adapter);
        fader.OnFadeOut = () => this.gameObject.SetActive(false);
        //  Physics2D.queriesStartInColliders = false;
        bullet = new GameObject("Bullet");
        SpriteRenderer bulletRenderer = bullet.AddComponent<SpriteRenderer>();
        bulletRenderer.sprite = gunSettings.bullet_sprite;
        bulletRenderer.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        bulletRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
        hitMark = new GameObject("HitMark");
        SpriteRenderer hitMarkRenderer = hitMark.AddComponent<SpriteRenderer>();
        hitMarkRenderer.sprite = gunSettings.hit_sprite;
        hitMarkRenderer.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        hitMarkRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;

        bullet.transform.localScale *= gunSettings.bullet_scale_factor;
        hitMark.transform.localScale *= gunSettings.hit_mark_scale_factor;

        bullet.SetActive(false);
        hitMark.SetActive(false);
        bullet.transform.SetParent(this.transform);
        if (gunSettings.isApplyForceOnHit)
            on_hit += applyForce;
        if (gunSettings.isCameraShakeOnHit)
            on_hit += DoCameraShake;
        aimed_direction = transform.right;
    }
    void Update()
    {
        GameLoop.Update(fader);
        if (isRecoiling)
            Recoil();

        //This is Executed while the Bullet going to hit something
        if (isBulletFired)
        {
            float _01t = (Time.time - timing) * inverse_raycast_hit_time;
            Vector2 beam_pos = Vector2.Lerp(initial_bullet_pos, final_bullet_pos, Mathf.Clamp01(_01t));
            bullet.transform.position = beam_pos;
        }
        //This is Executed At the Time the Bullet will Hit Something
        if (isBulletFired && Time.time - timing > ray_cast_wait_time)
        {
            if (on_hit != null)
                on_hit(hit_info);
            isBulletFired = false;
            bullet.SetActive(false);
            if (hit_info.collider != null)
            {
                hitMark.SetActive(true);
                hitMark.transform.position = hit_info.point;
                isSomethingHitted = true;
                timing = Time.time;
                if (hit_info.collider.GetComponent<MonoBehaviour>() is IDamageable)
                {
                    IDamageable damageable = hit_info.collider.GetComponent<MonoBehaviour>() as IDamageable;
                    float distance = hit_info.distance;
                    if (distance != 0)
                        damageable.TakeDamage(gunSettings.CanDamageAmount);
                }
            }
        }
        //This is Executed After The Bullet have hitted Something
        if (isSomethingHitted && Time.time - timing > hit_spark_stay_time)
        {
            hitMark.SetActive(false);
            isSomethingHitted = false;
        }
    }

    public void AimAt(Vector2 position)
    {
        aimed_direction = position - (Vector2)transform.position;
        float rotate_angle = VectorUtility.GetSignedAngle(transform.right, aimed_direction);
        transform.Rotate(Vector3.forward, rotate_angle, Space.World);
    }
    private void ToggleRecoil()
    {
        if (isRecoiling) return;
        isRecoiling = true;
        recoil_timing = Time.time;
        gun_hold_position = transform.localPosition;
    }
    private void Recoil()
    {
        float _01 = (Time.time - recoil_timing) * inverse_recoil_duration;
        Vector2 recoil_direction;
        if (transform.parent != null)
            recoil_direction = VectorUtility.Rotate(aimed_direction, transform.parent.rotation.eulerAngles.z);
        else
            recoil_direction = aimed_direction;
        Vector2 recoil_offset = -Mathf.Sin(_01 * Mathf.PI) * recoil_direction.normalized * recoil_amplitude;
        transform.localPosition = gun_hold_position + recoil_offset;
        if (_01 >= 1.0f)
        {
            isRecoiling = false;
            transform.localPosition = gun_hold_position;
        }
    }
    public void OnHit(UnityAction<RaycastHit2D> _call_)
    {
        on_hit += _call_;
    }
    public void Fire(float range)
    {
        if (!IsActive) return;
        initial_bullet_pos = transform.position + transform.up * gunSettings.nozel_offset.y + transform.right * gunSettings.nozel_offset.x;
        hit_info = Physics2D.Raycast(initial_bullet_pos, aimed_direction, range);
        ToggleRecoil();
        isBulletFired = true;
        timing = Time.time;
        bullet.SetActive(true);
        bullet.transform.localPosition = gunSettings.nozel_offset;
        if (hit_info.collider)
            final_bullet_pos = hit_info.point;
        else
            final_bullet_pos = initial_bullet_pos + range * (Vector2)transform.right;
    }
    private void applyForce(RaycastHit2D _hit_info)
    {
        if (_hit_info.rigidbody != null)
        {
            Vector2 application_point = ((Vector2)_hit_info.transform.position - _hit_info.point) * gunSettings.force_offset + _hit_info.point;
            _hit_info.rigidbody.AddForceAtPosition(aimed_direction * gunSettings.hit_force, application_point, ForceMode2D.Impulse);
        }

    }
    private void DoCameraShake(RaycastHit2D hit)
    {
    }
}
