// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Defines the expected change to the world model when an action is done.
    /// </summary>
    [Serializable]
    public class ActionEffect
    {
        public enum Operation { Add, Remove }

        [SerializeField]
        private Operation m_operation;

        [SerializeField]
        private DomainSpecifier m_domainSpecifier;

        [SerializeField]
        private EntitySpecifier m_entitySpecifier;

        [SerializeField]
        private int m_count = 1;

        public Operation operation
        {
            get { return m_operation; }
            set { m_operation = value; }
        }

        public DomainSpecifier domainSpecifier
        {
            get { return m_domainSpecifier; }
            set { m_domainSpecifier = value; }
        }
        public EntitySpecifier entitySpecifier
        {
            get { return m_entitySpecifier; }
            set { m_entitySpecifier = value; }
        }

        public int count
        {
            get { return m_count; }
            set { m_count = value; }
        }
    }

}