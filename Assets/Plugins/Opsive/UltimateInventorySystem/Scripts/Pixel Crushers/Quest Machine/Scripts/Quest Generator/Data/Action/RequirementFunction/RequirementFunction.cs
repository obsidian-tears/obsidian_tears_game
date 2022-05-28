// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Abstract base ScriptableObject class for requirement functions.
    /// </summary>
    public abstract class RequirementFunction : ScriptableObject
    {

        public abstract string typeName { get; }

        public abstract bool IsTrue(WorldModel worldModel);

    }
}