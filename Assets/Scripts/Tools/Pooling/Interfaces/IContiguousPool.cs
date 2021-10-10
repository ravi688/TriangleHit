using UnityEngine.Events;

public interface IContigousPool<t_Object>
{
    int Capacity { get; }
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
