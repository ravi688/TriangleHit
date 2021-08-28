
public interface IShieldable
{
    Timer ShieldTimer { get; }
    bool IsShielded { get; }
    void Shield(float duration);
}
