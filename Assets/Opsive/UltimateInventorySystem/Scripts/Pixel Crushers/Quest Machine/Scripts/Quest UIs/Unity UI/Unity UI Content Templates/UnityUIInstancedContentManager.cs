// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Manages Unity UI content that has been instantiated from templates.
    /// </summary>
    public class UnityUIInstancedContentManager
    {

        protected List<UnityUIContentTemplate> instances = new List<UnityUIContentTemplate>();

        public List<UnityUIContentTemplate> instancedContent { get { return instances; } }

        public void Clear()
        {
            for (int i = 0; i < instances.Count; i++)
            {
                instances[i].Despawn();
            }
            instances.Clear();
        }

        public void Add(UnityUIContentTemplate instance, RectTransform container)
        {
            if (container == null)
            {
                Debug.LogError("Quest Machine: Container isn't assigned to hold instance of UI template.", instance);
                return;
            }
            instance.gameObject.SetActive(true);
            instances.Add(instance);
            instance.transform.SetParent(container, false);
        }

        public void Remove(UnityUIContentTemplate instance)
        {
            instances.Remove(instance);
            instance.Despawn();
        }

        public UnityUIContentTemplate GetLastAdded()
        {
            return (instances.Count > 0) ? instances[instances.Count - 1] : null;
        }
    }
}