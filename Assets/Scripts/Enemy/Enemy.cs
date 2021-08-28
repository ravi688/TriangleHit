#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using System.Collections.Generic;

public class Enemy : EnemyCore, IDustbinDisposable, IDamageable, IBindable, IPointable, IKillable
{
    public bool IsPointed { get; set; }
    [SerializeField]
    private float pointOffset;
    public float PointOffset { get { return pointOffset; } }

    public bool IsDisposed { get; set; }
    public void Dispose()
    {
        if (!IsKilled)
            Kill();
        IsDisposed = true;
        Destroy(gameObject);
    }

    public List<IBindable> BindedObjects { get; set; }
    public bool IsBinded {  get { return BindedObjects.Count != 0; } }

    public BarMeter Health { get; private set; }
    public void TakeDamage(float amount)
    {
        Health.DecreaseValue(amount);
    }


    [SerializeField]
    private EnemySettings settings;
    private Gun gun;
    private bool IsStartFiring = false;
    private float timing;

    protected override void Awake()
    {
        InitializeEnemy(settings, GetComponent<Collider2D>().bounds.extents.y);
        base.Awake();
        BindedObjects = new List<IBindable>();
        gun = Instantiate(settings.Gun);
        gun.transform.SetParent(this.transform);
        gun.transform.localPosition = Vector2.up * settings.UpwardGunHoldOffset;
        Health = new BarMeter(0, settings.MaxHealth, settings.MaxHealth);
    }
    protected virtual void Start()
    {
        base.Start();
        GameManager.GetBindManager().RegisterBindable(this, GameConstants.TouchIDs.ENEMY);
        GameManager.GetPointManager().RegisterPointable(this);
    }
    private void OnDestroy()
    {
        GameManager.GetBindManager().UnregisterBindable(this);
        GameManager.GetPointManager().UnregisterPointable(this);
        if (gun.hitMark != null)                //Clear up memory  when the enemy is destroyed
            Destroy(gun.hitMark);
        if (gun.bullet != null)
            Destroy(gun.bullet);
    }

    protected override void OnDefenseUpdate(float sqr_player_distance)
    {
        HandleDefense();
    }
    protected override void OnIdleUpdate()
    {
        if (gun.IsActive)
            gun.IsActive = false;
    }
    protected override void OnHit(Collider2D collider, float relative_velocity_magnitude)
    {
        IDamageable incoming_player = collider.GetComponent<IDamageable>();
        if (incoming_player != null)
        {
            incoming_player.TakeDamage(settings.PlayerHitDamageFactor * relative_velocity_magnitude);
            StaticMemory.CurrentScore += (int)(settings.PlayerHitScoreFactor * relative_velocity_magnitude);
        }
        int damageAmount = (int)(settings.DamageFactor * relative_velocity_magnitude);
        PopUpText(string.Format("-{0}", damageAmount.ToString()));
        TakeDamage(damageAmount);
        StaticMemory.CurrentScore += (int)(settings.FloorHitScoreFactor * relative_velocity_magnitude);

        if (Health.Value == 0)
            Kill();
    }

    private void HandleDefense()
    {
        if (PlayerInAlertRange())
        {
            if (!gun.IsActive)
                gun.IsActive = true;
            gun.AimAt(PlayerCore.instance.transform.position);
        }
        else if (gun.IsActive && !PlayerInAlertRange())
            gun.IsActive = false;

        if (!IsStartFiring && PlayerInThreatRange())
        {
            IsStartFiring = true;
            timing = Time.time;
        }
        else if (IsStartFiring && !PlayerInThreatRange())
            IsStartFiring = false;

        if (IsStartFiring)
        {
            if (Time.time - timing > settings.FireInterval)
            {
                gun.Fire(10.0f);
                timing = Time.time;
            }
        }
    }
}