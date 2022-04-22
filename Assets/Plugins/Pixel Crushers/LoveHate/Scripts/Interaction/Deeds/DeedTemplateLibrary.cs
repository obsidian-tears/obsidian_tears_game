// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// A deed template library contains a list of deed templates.
    /// </summary>
    public class DeedTemplateLibrary : ScriptableObject
    {

        public FactionDatabase factionDatabase = null;

        /// <summary>
        /// The predefined deed templates.
        /// </summary>
        public List<DeedTemplate> deedTemplates = new List<DeedTemplate>();

    }

}
