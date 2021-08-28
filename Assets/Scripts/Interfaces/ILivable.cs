public interface ILivable
{
    bool IsKilled { get; }
    int LiveCount { get; }
    int MaxLives { get; }
    void CreditLives(int live_count);
}
