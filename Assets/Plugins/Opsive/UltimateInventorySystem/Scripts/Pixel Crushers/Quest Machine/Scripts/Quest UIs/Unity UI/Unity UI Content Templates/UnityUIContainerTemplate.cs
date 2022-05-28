// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Unity UI holder for general UI content.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class UnityUIContainerTemplate : UnityUIContentTemplate
    {

        [NonSerialized]
        private List<UnityUIContentTemplate> m_instances = new List<UnityUIContentTemplate>();

        public List<UnityUIContentTemplate> instances { get { return m_instances; } }

        public void AddInstanceToContainer(UnityUIContentTemplate instance)
        {
            instance.gameObject.SetActive(true);
            instances.Add(instance);
            instance.transform.SetParent(this.transform, false);
        }

        public override void Despawn()
        {
            instances.ForEach(instance => instance.Despawn());
            instances.Clear();
            base.Despawn();
        }

    }
}
