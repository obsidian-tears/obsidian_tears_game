// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A motive is text associated with a set of drive values. It's used to provide
    /// descriptive rationale for why a quest generator chose a particular action.
    /// </summary>
    [Serializable]
    public class Motive
    {

        [StringFieldTextArea]
        [SerializeField]
        private StringField m_text;

        [SerializeField]
        private DriveValue[] m_driveValues = new DriveValue[0];

        public StringField text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public DriveValue[] driveValues
        {
            get { return m_driveValues; }
            set { m_driveValues = value; }
        }
    }
}