#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using System;

public abstract class EnemyCore : MonoBehaviour
{
    public bool IsKilled { get; private set; }

    private EnemyCoreSettings settings;
    [SerializeField]
    private StandardAnimation PopUpTextAnimationData;

   
    private AnimatedText DamageText;

    private float sqrAlertDistance;
    private float sqrPlayerEnemyDistance;
    private float sqrThreathDistance;
    private float sqrTakeDamageThresholdVelocity;
    private Vector3 upward_offset_vector;


    protected virtual void Awake()
    {
        StaticMemory.EnemyCount++;
        StaticMemory.MaxEnemyCount = StaticMemory.EnemyCount;
        sqrThreathDistance = settings.ThreatDistance * settings.ThreatDistance;
        sqrAlertDistance = settings.AlertDistance * settings.AlertDistance;
        sqrTakeDamageThresholdVelocity = settings.TakeDamageThresholdVelocity * settings.TakeDamageThresholdVelocity;
        DamageText = AnimatedText.UIText(PopUpTextAnimationData, new Vector2(200, 70), Color.white, 50);
       
    }
    protected virtual void Start()
    {
        //Call this after Awake because Calling any Getter in GameManager may create new default singleton instance if instace == null
        CreateCheats();
    }
    private void CreateCheats()
    {
        Action<CheatType> OnCheat = null;
        OnCheat = delegate (CheatType cheat_type)
        {
            if (cheat_type == CheatType.KillAllEnemies && !IsKilled)
            {
                Kill();
                GameManager.GetCheatManager().OnCheatHandler -= OnCheat;
            }
        };
        GameManager.GetCheatManager().OnCheatHandler += OnCheat;
    }

    protected virtual void Update()
    {
        GameLoop.Update(DamageText);

        if (PlayerCore.instance != null && !IsKilled)
        {
            sqrPlayerEnemyDistance = (transform.position - PlayerCore.instance.transform.position).sqrMagnitude;
            OnDefenseUpdate(sqrPlayerEnemyDistance);
        }
        else
            OnIdleUpdate();
    }

    protected abstract void OnIdleUpdate();
    protected abstract void OnDefenseUpdate(float sqr_player_distance);
    protected abstract void OnHit(Collider2D collider, float relative_velocity_magnitude);

    protected void InitializeEnemy(EnemyCoreSettings settings, float upward_offset)
    {
        this.settings = settings;
        upward_offset_vector = Vector3.up * upward_offset;
    }
    protected bool PlayerInThreatRange()
    {
        return sqrPlayerEnemyDistance < sqrThreathDistance;
    }
    protected bool PlayerInAlertRange()
    {
        return sqrPlayerEnemyDistance < sqrAlertDistance;
    }
    protected void PopUpText(string str)
    {
        DamageText.PopUp(str, GameManager.GetCamera().WorldToScreenPoint(transform.position + upward_offset_vector), Vector2.zero, Quaternion.identity);
    }
    private void OnCollisionEnter2D(Collision2D CollisionInfo)
    {
        if (IsKilled) return;
        if (CollisionInfo.relativeVelocity.sqrMagnitude > sqrTakeDamageThresholdVelocity)
        {
            float relative_velocity_magnitude = CollisionInfo.relativeVelocity.magnitude;
            OnHit(CollisionInfo.collider, relative_velocity_magnitude);
        }
    }

    public virtual void Kill()
    {
        PopUpText("Killed");
        IsKilled = true;
        StaticMemory.CurrentScore += settings.ScoreOnKilled;
    }

}