public interface IKillable : IDamageable
{
    bool IsKilled { get; }
    void Kill();
}