
using UnityEngine;

public class SingletonBehaviour<T>: MonoBehaviour where T: MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance is null)
                {
                    GameObject obj = new GameObject(nameof(T));
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
