
#pragma warning disable
using System;
using UnityEngine;

public class StaticRandomAccessPool<t_ObjectType, t_Key> : IRandomAccessPool<t_ObjectType, t_Key>
{
    public int ActiveCount {  get { return 0; } }
    public int InactiveCount {  get {  return 0; } }
    public int Capacity {  get { return 0; } }

    public StaticRandomAccessPool()
    {

    }
    public StaticRandomAccessPool(int capacity = 1)
    {
        m_capacity = capacity;
        m_object_buffer = new Reference<t_ObjectType>[m_capacity];
        m_active_mask = new bool[m_capacity];
    }

    public int capacity { get { return m_capacity; } }
    public int activeCount { get { return m_active_count; } }
    public int inactiveCount { get { return m_capacity - m_active_count; } }

    public void Add(t_ObjectType obj, t_Key key)
    {
        Debug.Log("Added in StaticRandomAccessPool");
    }
    public void Resize(int new_capacity)
    {
        Debug.LogFormat("Resized StaticRandomAccessPool to {0}", new_capacity);
    }
    public void ReturnToPool(t_ObjectType obj)
    {
        Debug.Log("Return to StaticRandomAccessPool");
    }
    public t_ObjectType GetFromPool(t_Key key)
    {
        throw new System.NotImplementedException();
    }

    public t_ObjectType PeekInPool(t_Key key)
    {
        throw new System.NotImplementedException();
    }

    private int m_capacity;
    private int m_active_count;
    private bool[] m_active_mask;
    private Reference<t_ObjectType>[] m_object_buffer;
}