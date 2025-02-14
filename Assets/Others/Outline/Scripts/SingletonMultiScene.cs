using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMultiScene<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<T>();
            if (_instance) return _instance;
            GameObject obj = new GameObject(typeof(T).ToString());
            _instance = obj.AddComponent<T>();
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (!_instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
