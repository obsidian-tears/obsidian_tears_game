using Firesplash.GameDevAssets.GitCollab.GUI;
using System;
using System.Collections.Concurrent;
using UnityEditor;

namespace Firesplash.GameDevAssets.GitCollab
{
    internal class EngineDispatcher
    {
        static EngineDispatcher _instance;
        ConcurrentQueue<Action> mainThreadQueue;
        ConcurrentQueue<GitDialog> queuedDialogs;

        static EngineDispatcher Instance
        {
            get
            {
                if (_instance == null) _instance = new EngineDispatcher();
                return _instance;
            }
        }

        EngineDispatcher()
        {
            mainThreadQueue = new ConcurrentQueue<Action>();
            queuedDialogs = new ConcurrentQueue<GitDialog>();
            EditorApplication.update += Dispatch;
        }

        public static void Enqueue(Action action)
        {
            Instance._Enqueue(action);
        }

        void _Enqueue(Action action)
        {
            mainThreadQueue.Enqueue(action);
            // == is correct as we add one by one and don't want to flood the console
            if (mainThreadQueue.Count == 50) UnityEngine.Debug.LogWarning("The dispatcher queue is very long. Looks like the dispatcher is having some issues. If this happens often or if you're having issues with the git collaboration tools, please contact our support at assets@firesplash.de");
        }


        public static void Enqueue(GitDialog dialog)
        {
            Instance._Enqueue(dialog);
        }

        void _Enqueue(GitDialog dialog)
        {
            queuedDialogs.Enqueue(dialog);
            // == is correct as we add one by one and don't want to flood the console
            if (mainThreadQueue.Count == 10) UnityEngine.Debug.LogWarning("The dispatcher queue is very long. Looks like the dispatcher is having some issues. If this happens often or if you're having issues with the git collaboration tools, please contact our support at assets@firesplash.de");
        }

        void Dispatch()
        {
            int counter = 0;
            while (mainThreadQueue != null && mainThreadQueue.Count > 0)
            {
                Action action;
                mainThreadQueue.TryDequeue(out action);
                if (action != null) action.Invoke();

                if (++counter > 5) break; //limit actions per "frame" to not cause the editor getting stuck
            }

            while (queuedDialogs != null && queuedDialogs.Count > 0)
            {
                GitDialog dqDialog;
                if (queuedDialogs.TryDequeue(out dqDialog))
                {
                    if (EditorUtility.DisplayDialog((dqDialog.title == null ? "Git Collaborative Tools" : "Git Collaborative Tools: " + dqDialog.title), dqDialog.text, dqDialog.positiveButton, dqDialog.negativeButton) && dqDialog.onPositive != null)
                    {
                        dqDialog.onPositive?.Invoke();
                    }
                    else
                    {
                        dqDialog.onNegative?.Invoke();
                    }
                }
            }
        }
    }
}

