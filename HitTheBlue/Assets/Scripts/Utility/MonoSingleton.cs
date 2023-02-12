using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static object lockObject = new object();
    private static bool applicationIsQuitting = false;
    protected static T instance = null;
    public static T Instance {
        get {

            if (applicationIsQuitting)
            {
                return null;
            }
            lock (lockObject)
            {
                if (instance == null)
                    instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null)
                {
                    instance = new GameObject("@" + typeof(T).ToString(),
                                               typeof(T)).AddComponent<T>();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }
    }

    private void OnDisable()
    {
        applicationIsQuitting = true;
        //혹시 해제 안됬으면 가비지 컬렉터한테 끌려가라
        if (instance.gameObject != null)
            Destroy(instance.gameObject);
        instance = null;
    }


    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.quitting += () => applicationIsQuitting = true;
    }

}