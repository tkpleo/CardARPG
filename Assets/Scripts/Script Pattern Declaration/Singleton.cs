using System.Dynamic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindAnyObjectByType<T>();
            if (_instance != null) return _instance;
            
            return CreateInstance();
        }
    }

    protected virtual string ObjectName => $"{typeof(T).Name} (Singleton)";

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = (T)this;
        DontDestroyOnLoad(gameObject);
        gameObject.name = ObjectName;
    }

    private static T CreateInstance()
    {
        GameObject singletonObject = new(typeof(T).Name);
        return singletonObject.AddComponent<T>();
    }
}
