// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Associates a value with a drive. Quest givers' personalities (quest generation
    /// priorities) are defined by the values they associated with drives.
    /// </summary>
    [Serializable]
    public class DriveValue
    {

        [SerializeField]
        private Drive m_drive;

        [Range(-100,+100)]
        [SerializeField]
        private float m_value;

        public Drive drive
        {
            get { return m_drive; }
            set { m_drive = value; }
        }

        public float value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public DriveValue() { }

        public DriveValue(Drive drive, float value)
        {
            m_drive = drive;
            m_value = value;
        }

        public DriveValue(DriveValue other)
        {
            if (other != null)
            {
                m_drive = other.drive;
                m_value = other.value;
            }
        }

    }

}
