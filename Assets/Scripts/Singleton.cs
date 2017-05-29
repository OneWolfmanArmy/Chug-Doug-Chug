using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour 
{
    public static MonoBehaviour Instance;

    //public static Singleton Instance
    //{
    //    get
    //    {
    //        if (mInstance == null)
    //        {
    //            mInstance = (T)FindObjectOfType(typeof(T));

    //            if (FindObjectsOfType(typeof(T)).Length > 1)
    //            {

    //                return mInstance;
    //            }

    //            if(mInstance == null)
    //            {

    //            }
    //        }


    //        return mInstance;
    //    }
    //}
    #region MonoBehaviour

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion


}
