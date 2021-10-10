using UnityEngine.Events;

public interface IContiguousPool<t_Object>
{
    int Size { get; }
    int ActiveCount { get; }
    int InactiveCount { get; }

    UnityAction<t_Object> OnInactive { get; set; }
    UnityAction<t_Object> OnActive { get; set; }

    void Add(t_Object obj);
    void Resize(int new_capacity);
    void ForEachActive(UnityAction<t_Object> callback);

    void ReturnToPool(t_Object obj);
    t_Object GetFromPool();
}
