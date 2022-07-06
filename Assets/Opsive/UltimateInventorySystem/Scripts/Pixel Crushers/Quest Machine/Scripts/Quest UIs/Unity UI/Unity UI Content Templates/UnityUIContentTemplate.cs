// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base class for Unity UI content templates.
    /// </summary>
    public abstract class UnityUIContentTemplate : MonoBehaviour
    {

        public virtual void Despawn()
        {
            Destroy(gameObject);
        }

    }
}
