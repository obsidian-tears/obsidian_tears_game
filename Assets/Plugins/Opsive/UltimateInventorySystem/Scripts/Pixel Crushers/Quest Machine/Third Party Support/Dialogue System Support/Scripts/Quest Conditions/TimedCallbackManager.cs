// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// Invokes callback methods on timed frequencies.
    /// </summary>
    public class TimedCallbackManager : MonoBehaviour
    {

        private static TimedCallbackManager m_instance;
        private static TimedCallbackManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameObject("TimedCallbackManager").AddComponent<TimedCallbackManager>();
                    DontDestroyOnLoad(m_instance.gameObject);
                }
                return m_instance;
            }
        }

        public class CallbackInfo
        {
            public System.Action callback;
            public float frequency;
            public float remaining;

            public CallbackInfo(System.Action callback, float frequency)
            {
                this.callback = callback;
                this.frequency = frequency;
                this.remaining = frequency;
            }

            public void Update()
            {
                remaining -= Time.deltaTime;
                if (remaining < 0)
                {
                    remaining = frequency;
                    callback();
                }
            }
        }

        private List<CallbackInfo> callbacks = new List<CallbackInfo>();

        private void Update()
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                callbacks[i].Update();
            }
        }

        public static void StartCallback(System.Action callback, float frequency)
        {
            instance.callbacks.Add(new CallbackInfo(callback, frequency));
        }

        public static void StopCallback(System.Action callback)
        {
            instance.callbacks.RemoveAll(x => x.callback == callback);
        }

        public static void StopAllCallbacks()
        {
            instance.callbacks.Clear();
        }

    }
}