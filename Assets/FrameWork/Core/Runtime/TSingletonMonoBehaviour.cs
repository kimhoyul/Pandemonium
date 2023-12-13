using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TOONIPLAY
{
    public class TSingleton<T> where T : new()
    {
        private static bool _isCreated;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_isCreated)
                    return _instance;
                
                _instance ??= new T();
                _isCreated = true;

                return _instance;
            }
        }
    }

    public class TSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _isCreated;
        private static T _instance;

        private void Awake()
        {
#if DEV_LOG_SINGLETON
            Debug.Log($"[Singleton] Awake of {typeof(T).Name}.");
#endif

            if (!_isCreated)
            {
                _instance = this as T;
                _isCreated = true;

                if (transform.parent == null)
                    DontDestroyOnLoad(this);
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private static void CreateInstance()
        {
            var existInstance = FindObjectOfType<T>();
            if (existInstance != null)
            {
                _instance = existInstance;
                _isCreated = true;
            }
            else
            {
                var go = new GameObject(typeof(T).Name);
                go.AddComponent<T>();
            }
        }

        public static T Instance
        {
            get
            {
                if (_isCreated)
                    return _instance;
                
                if (_instance == null)
                    CreateInstance();

#if DEV_LOG_SINGLETON
                Debug.Log($"[Singleton] Return instance of {_instance.GetType().Name}.");
#endif

                return _instance;
            }
        }

        public static bool IsExist => _instance != null;
    }

#if ODIN_INSPECTOR
	public class TSingletonSerializedMonoBehaviour<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
    {
        private static bool isCreated = false;
        private static T _instance;

        private void Awake()
        {
            if (!isCreated)
            {
                _instance = this as T;
                isCreated = true;

                if (transform.parent == null)
                    DontDestroyOnLoad(this);
            }

            OnAwake();
        }

        protected virtual void OnAwake() { }

        private static void CreateInstance()
        {
            var existInstance = FindObjectOfType<T>();
            if (existInstance != null)
            {
                _instance = existInstance;
                isCreated = true;
            }
            else
            {
                GameObject go = new GameObject(typeof(T).Name);
                go.AddComponent<T>();
            }
        }

        public static T Instance
        {
            get
            {
                if (!isCreated)
                {
                    if (_instance == null)
                        CreateInstance();
                }

                return _instance;
            }
        }

        public static bool IsExist { get { return _instance != null; } }
    }
#else
    public class TSingletonSerializedMonoBehaviour<T> : TSingletonMonoBehaviour<T> where T : MonoBehaviour
    {
    }
#endif
}
