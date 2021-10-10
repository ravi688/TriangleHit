using UnityEngine;
using UnityEngine.Events;

//StaticPool is very efficient for Ammunition Systems but very error prone; so use it carefully, very similar to CircularBuffer
//for example, you should not call ReturnToPool multiple times for the same object
//or you should not call GetFromPool without checking the inactiveCount > 0
public class StaticContiguousPool<t_Object> : IContigousPool<t_Object>
{
    public int Capacity { get { return m_capacity; } }
    public int ActiveCount { get { return m_active_count; } }
    public int InactiveCount { get { return m_capacity - m_active_count; } }
    public UnityAction<t_Object> OnInactive { get; set; }
    public UnityAction<t_Object> OnActive { get; set; }

    public StaticContiguousPool(int capacity = 1)
    {
        m_object_buffer = new Reference<t_Object>[capacity];
        m_active_mask = new bool[capacity];
        m_capacity = capacity;
        for (int i = 0; i < capacity; i++)
            m_active_mask[i] = true;
        m_inactive_cursor = 0;
        m_active_cursor = m_capacity - 1;
        m_active_count = m_capacity;
    }
    public void Resize(int new_capacity)
    {
        if (new_capacity == m_capacity) return;
        if (new_capacity > m_capacity)
        {
            Reference<t_Object>[] new_object_buffer = new Reference<t_Object>[new_capacity];
            bool[] new_active_mask = new bool[new_capacity];
            for (int i = 0; i < m_capacity; i++)
            {
                new_object_buffer[i] = m_object_buffer[i];
                new_active_mask[i] = m_active_mask[i];
            }
            for (int i = m_capacity; i < new_capacity; i++)
                new_active_mask[i] = true;
            m_active_mask = new_active_mask;
            m_object_buffer = new_object_buffer;
        }
        else//if new_capacity < m_capacity
        {
            for (int i = new_capacity; i < m_capacity; i++)
                if (m_active_mask[i])
                    m_active_count--;
        }
        m_capacity = new_capacity;
        m_inactive_cursor = Mathf.Clamp(m_inactive_cursor, 0, m_capacity - 1);
        m_active_cursor = Mathf.Clamp(m_active_cursor, 0, m_capacity - 1);
    }
    //Must be called before any of ReturnToPool or GetFromPool methods, in sequence
    public void Add(t_Object obj)
    {
        if (m_inactive_cursor >= m_capacity)   //if object buffer is full
            return;
        m_object_buffer[m_inactive_cursor] = new Reference<t_Object>(obj);
        ReturnToPool(obj);
    }
    public void ReturnToPool(t_Object obj)
    {
        if (!m_active_mask[m_inactive_cursor])  //if already inactive, then return
            return;
        m_object_buffer[m_inactive_cursor].Set(obj);
        m_active_mask[m_inactive_cursor] = false;    //set it inactive
        OnInactive(obj);
        m_inactive_cursor++;
        m_inactive_cursor %= m_capacity;
        --m_active_count;
    }
    public t_Object GetFromPool()
    {
        Reference<t_Object> obj_ref = m_object_buffer[m_active_cursor];
        m_active_mask[m_active_cursor] = true;     //set it active
        OnActive(obj_ref.Get());
        m_active_cursor++;
        m_active_cursor %= m_capacity;
        ++m_active_count;
        return obj_ref.Get();
    }
    public void ForEachActive(UnityAction<t_Object> callback)
    {
        if (m_active_cursor > m_inactive_cursor)  //if active objects are in-between the cursors
            for (int i = m_inactive_cursor; i <= m_active_cursor; i++)
                callback(m_object_buffer[i].Get());
        else //if m_active_cursor <= m_inactive_cursor,  active objects are not in-between the cursors
        {
            int cursor = m_inactive_cursor;
            while (cursor < m_active_cursor)
            {
                callback(m_object_buffer[cursor].Get());
                cursor++;
                cursor %= m_capacity;
            }
        }
    }

    private Reference<t_Object>[] m_object_buffer;
    private bool[] m_active_mask;
    private int m_capacity;
    private int m_active_count;
    private int m_inactive_cursor;  //or rear_cursor 
    private int m_active_cursor;    //or front_cursor
}