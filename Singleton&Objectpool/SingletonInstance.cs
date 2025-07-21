using UnityEngine;

namespace Assets.Scripts.Framework
{
    public class SingletonInstance<T> : MonoBehaviour where T : Component
    {
        private static T s_instance;

        public static T Singleton
        {
            get
            {
                if (s_instance) return s_instance;

                s_instance = FindFirstObjectByType<T>();

                if (s_instance) return s_instance;

                s_instance = (new GameObject
                {
                    name = nameof(T),
                    hideFlags = HideFlags.HideAndDontSave,
                }).AddComponent<T>();

                Debug.Log("Singleton.cs Created new Singleton of type: " + typeof(T) +
                                          ", Script calling to Singleton instance that does not exists.");
                return s_instance;
            }
        }

        private void OnDestroy()
        {
            if (s_instance == this)
            {
                s_instance = null;
            }
        }
    }

    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public bool unParentOnAwake = true;

        public static bool HasInstance => instance != null;
        public static T Current => instance;

        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        var obj = new GameObject();
                        obj.name = typeof(T).Name + "AutoCreated";
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake() => InitSingleton();

        protected virtual void InitSingleton()
        {
            if (!Application.isPlaying) return;
            if (unParentOnAwake) transform.SetParent(null);

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                enabled = true;
            }
            else
            {
                if (this != instance) Destroy(gameObject);
            }
        }
    }
}