#if !RELEASE_MODE
#define THROW_EXCEPTION
#endif

#if !THROW_EXCEPTION
#define THROW_EXCEPTION
#endif

using UnityEngine;
using UnityEngine.Events;

//StaticPool is very efficient for Ammunition Systems but very error prone; so use it carefully, very similar to CircularBuffer
//for example, you should not call ReturnToPool multiple times for the same object
//or you should not call GetFromPool without checking the inactiveCount > 0
public class StaticContiguousPool<t_Object> : IContiguousPool<t_Object>
{
    public int Size { get { return m_size; } }
    public int ActiveCount { get { return m_active_count; } }
    public int InactiveCount { get { return m_size - m_active_count; } }
    public UnityAction<t_Object> OnInactive { get; set; }
    public UnityAction<t_Object> OnActive { get; set; }

    public StaticContiguousPool(int capacity)
    {
        m_object_buffer = new Reference<t_Object>[capacity];
        m_active_mask = new bool[capacity];
        m_size = capacity;
        for (int i = 0; i < capacity; i++)
            m_active_mask[i] = true;
        m_inactive_cursor = 0;
        m_active_cursor = 0;
        m_active_count = m_size;
    }
    public void Resize(int new_capacity)
    {
        if (new_capacity == m_size) return;
        if (new_capacity > m_size)
        {
            Reference<t_Object>[] new_object_buffer = new Reference<t_Object>[new_capacity];
            bool[] new_active_mask = new bool[new_capacity];
            for (int i = 0; i < m_size; i++)
            {
                new_object_buffer[i] = m_object_buffer[i];
                new_active_mask[i] = m_active_mask[i];
            }
            for (int i = m_size; i < new_capacity; i++)
                new_active_mask[i] = true;
            m_active_mask = new_active_mask;
            m_object_buffer = new_object_buffer;
        }
        else//if new_capacity < m_capacity
        {
            for (int i = new_capacity; i < m_size; i++)
                if (m_active_mask[i])
                    m_active_count--;
        }
        m_size = new_capacity;
        m_inactive_cursor = Mathf.Clamp(m_inactive_cursor, 0, m_size - 1);
        m_active_cursor = Mathf.Clamp(m_active_cursor, 0, m_size - 1);
    }
    //Must be called before any of ReturnToPool or GetFromPool methods, in sequence
    public void Add(t_Object obj)
    {
        if (m_inactive_cursor >= m_size)   //if object buffer is full
            return;
        ReturnToPool(obj);
    }
    public void ReturnToPool(t_Object obj)
    {
#if THROW_EXCEPTION
        if (m_active_count <= 0)
            throw new System.IndexOutOfRangeException("Pool is full of inactive objects; m_active_count <= 0");
#endif
        if (!m_active_mask[m_inactive_cursor])  //if already inactive, then return
        {
#if THROW_EXCEPTION
            throw new System.InvalidOperationException("Object is already present and inactive in the pool, !m_active_mask[m_inactive_cursor]");
#else
            return;
#endif
        }
        m_object_buffer[m_inactive_cursor] = obj;
        m_active_mask[m_inactive_cursor] = false;    //set it inactive
        if(OnInactive != null) OnInactive(obj);
        m_inactive_cursor++;
        m_inactive_cursor %= m_size;
        --m_active_count;
    }
    public t_Object GetFromPool()
    {

#if THROW_EXCEPTION
        if (m_active_count >= m_size)
            throw new System.IndexOutOfRangeException("Poll doesn't have inactive objects; m_active_count >= m_size");
#endif

        t_Object obj = m_object_buffer[m_active_cursor];
        m_active_mask[m_active_cursor] = true;     //set it active
        if(OnActive != null) OnActive(obj);
        m_active_cursor++;
        m_active_cursor %= m_size;
        ++m_active_count;
        return obj;
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
                cursor %= m_size;
            }
        }
    }

    private Reference<t_Object>[] m_object_buffer;
    private bool[] m_active_mask;
    private int m_size;
    private int m_active_count;
    private int m_inactive_cursor;  //or rear_cursor 
    private int m_active_cursor;    //or front_cursor
}