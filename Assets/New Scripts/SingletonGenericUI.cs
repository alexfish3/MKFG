///
/// To act as a universal singleton option to derrive for my generic UI. To be used in any project 
/// Created by Alex Fischer | 2023
/// 

using UnityEngine;

public abstract class SingletonGenericUI<T> : GenericUI where T : GenericUI
{
    private static T instance;
    [Space(10)]
    public bool dontDestroyOnLoad;

    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        // Normal singleton reference
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            if (this)
            {
                Destroy(gameObject);
            }
        }

        // If true, allow for scene loading
        if (dontDestroyOnLoad)
        {
            T[] objs = FindObjectsOfType<T>();
            if (objs.Length > 1)
            {
                if (this)
                {
                    Destroy(gameObject);
                }
            }


            DontDestroyOnLoad(this.gameObject);
        }
    }
}
