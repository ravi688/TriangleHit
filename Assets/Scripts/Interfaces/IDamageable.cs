
public interface IDamageable
{
    BarMeter Health { get; }
    void TakeDamage(float amount);
}