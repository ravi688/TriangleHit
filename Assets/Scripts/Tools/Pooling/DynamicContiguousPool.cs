
using System;
using UnityEngine.Events;

public class DynamicContigousPool<t_ObjectType> : IContigousPool<t_ObjectType>
{
    public int Capacity { get { return 0; } }
    public int ActiveCount { get { return 0; } }
    public int InactiveCount { get { return 0; } }

    public UnityAction<t_ObjectType> OnActive { get; set; }
    public UnityAction<t_ObjectType> OnInactive { get; set; }

    public DynamicContigousPool()
    {

    }

    public void Add(t_ObjectType obj) { }
    public void Resize(int new_capacity) { }
    public void ReturnToPool(t_ObjectType obj) { }
    public t_ObjectType GetFromPool()
    {
        throw new NotImplementedException();
    }
    public void ForEachActive(UnityAction<t_ObjectType> callback)
    {
        throw new NotImplementedException();
    }
}