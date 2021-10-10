
using System;

public interface IRandomAccessPool<t_Object, t_Key>
{
    int Capacity { get; }
    int ActiveCount { get; }
    int InactiveCount { get; }

    void Add(t_Object obj, t_Key key);
    void Resize(int new_capacity);
    void ReturnToPool(t_Object obj);
    t_Object GetFromPool(t_Key key);
    t_Object PeekInPool(t_Key key);
}