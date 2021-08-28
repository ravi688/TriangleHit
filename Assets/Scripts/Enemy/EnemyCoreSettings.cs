using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCoreSettings", menuName = "Custom/EnemySettings/EnemyCoreSettings")]
public class EnemyCoreSettings : ScriptableObject
{
    public float DamageFactor;
    public float TakeDamageThresholdVelocity;
    public float AlertDistance;
    public float ThreatDistance;
    public float PlayerHitDamageFactor = 0.1f;
    public float PlayerHitScoreFactor = 0.5f; 
    public int ScoreOnKilled = 20;
    public float FloorHitScoreFactor = 0.1f; 
    
}
