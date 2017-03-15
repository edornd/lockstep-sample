using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour {
    protected static T instance;

    /**
       Returns the instance of this singleton.
    */
    public static T Instance {
        get {
            if (instance == null) {
                Init();

                if (instance == null) {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }

    public static void Init() {
        if (!instance) {
            instance = (T)FindObjectOfType(typeof(T));
        }
    }
}
