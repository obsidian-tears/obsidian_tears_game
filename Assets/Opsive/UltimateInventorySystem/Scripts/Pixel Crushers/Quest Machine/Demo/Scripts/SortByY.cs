// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Sets a sprite's sortingOrder according to its Y position.
    /// </summary>
    public class SortByY : MonoBehaviour
    {

        public bool isStationary;

        public SpriteRenderer shadow;

        public float multiplier = 10f;

        private SpriteRenderer m_spriteRenderer;

        private void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            SetSortingOrder();
            if (isStationary) enabled = false;
        }

        public void Update()
        {
            SetSortingOrder();
        }

        public void SetSortingOrder()
        { 
            if (m_spriteRenderer != null)
            {
                m_spriteRenderer.sortingOrder = -Mathf.FloorToInt(transform.position.y * multiplier);
                if (shadow != null) shadow.sortingOrder = m_spriteRenderer.sortingOrder - 1;
            }
        }
    }

}