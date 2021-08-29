using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T: Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            ForceAwake();
            return _instance;
        }
    }

    public static void ForceAwake(HideFlags hideFlags = HideFlags.NotEditable)
    {
        if (_instance == null)
        {
            _instance = new GameObject(typeof(T).Name).AddComponent<T>();
            _instance.gameObject.hideFlags = hideFlags;
        }
    }
}