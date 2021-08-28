
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Custom/EnemySettings/EnemySettings")]
public class EnemySettings : EnemyCoreSettings
{
    public EnemyDamageKey[] Keys;
    public float FireInterval = 1.0f;
    public float UpwardGunHoldOffset= 3.0f;
    public float MaxHealth = 100;
    public Gun Gun;
}