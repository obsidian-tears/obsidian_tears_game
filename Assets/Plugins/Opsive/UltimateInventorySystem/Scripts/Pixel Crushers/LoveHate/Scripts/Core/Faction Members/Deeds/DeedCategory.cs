// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Optional category to assign to a deed.
    /// </summary>
    public class DeedCategory : ScriptableObject
    {
        [SerializeField]
        protected string m_categoryName;

        public virtual string categoryName { 
            get { return m_categoryName; } 
            set { m_categoryName = value; }
        }

    }
}
