

public interface IHealable 
{
    BarMeter Health { get; }
    void Heal(float amount);
}