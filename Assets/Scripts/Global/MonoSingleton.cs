using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour {
    private static T instance;
    public static T Get() => instance;

    protected static void SetInstance(T newInstance) {
        instance = newInstance;
    }
}