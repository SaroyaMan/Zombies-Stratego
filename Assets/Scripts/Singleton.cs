using UnityEngine;

//T must inherits from MonoBehaviour
public class Singleton<T>: MonoBehaviour where T : MonoBehaviour {

    private static T instance;

    public static T Instance {
        get {
            T memorizedObject = FindObjectOfType<T>();
            if(instance == null) {
                instance = memorizedObject;
                //DontDestroyOnLoad(instance);
            }
            else if(instance != memorizedObject)
                Destroy(memorizedObject);
            return instance;
        }
    }
}
