using System;

public interface IStarCollectable
{
    int StarCount { get; }
    void CollectStars(int start_count);
}