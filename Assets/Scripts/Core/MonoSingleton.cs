using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ApplicationQuitting
    {
        public bool quitting;
    }

    /// <summary>
    /// Mono singleton Class. Extend this class to make singleton component.
    /// Example: 
    /// Override Init method instead of using Awake() from this class.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T s_instance = null;
        private static bool s_isInitialized;
        private static ApplicationQuitting s_isQuitting = new ApplicationQuitting();

        public static bool Exist
        {
            get
            {
                return s_instance != null;
            }
        }

        public static T Instance
        {
            get
            {
                // Instance requiered for the first time, we look for it
                if (s_instance == null && !s_isQuitting.quitting)
                {
                    s_instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    // Object not found
                    if (s_instance == null)
                    {
                        s_instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        // Problem during the creation, this should not happen
                        if (s_instance == null)
                        {
                            Debug.LogError("ERROR! Problem during the singleton creation of " + typeof(T).ToString());
                        }
                    }

                    if (!s_isInitialized)
                    {
                        DontDestroyOnLoad(s_instance.gameObject);
                        s_isInitialized = true;
                        s_instance.Init();
                    }
                }
                else if (s_isQuitting.quitting && s_instance == null)
                {
                    Debug.LogError("ERROR! Application is quitting, cannot create singleton instance of " + typeof(T).ToString());
                }

                return s_instance;
            }
        }

        // If no other monobehaviour request the instance in an awake function
        // executing before this one, no need to search the object.
        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
            }
            else if (s_instance != this)
            {
                Debug.LogWarning("WARNING: Another singleton instance of " + GetType() + " is already exist! Destroying self...");
                DestroyImmediate(this);
                return;
            }

            if (!s_isInitialized)
            {
                DontDestroyOnLoad(gameObject);
                s_isInitialized = true;
                s_instance.Init();
            }
        }

        void OnApplicationQuit()
        {
            s_isQuitting.quitting = true;
        }

        /// <summary>
        /// This function is called when the instance is used the first time
        /// Put all the initializations you need here, as you would do in Awake
        /// </summary>
        protected abstract void Init();
    }
}