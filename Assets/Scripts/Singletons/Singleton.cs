using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T>: MonoBehaviour 
{
    public bool DoNotDestroy;

    public static T Instance;

    #region MonoBehaviour

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
        else if (!ReferenceEquals(Instance, GetComponent<T>()))
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion
}
