// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Interface for classes that have an id, displayName, and image
    /// such as QuestEntity, IdentifiableQuestListContainer, and QuestMachineID.
    /// </summary>
    public interface IQuestMachineID
    {

        /// <summary>
        /// Internal ID that Quest Machine can use to reference this GameObject.
        /// </summary>
        StringField id { get; }

        /// <summary>
        /// Name to show in UIs.
        /// </summary>
        StringField displayName { get; }

        /// <summary>
        /// Image to show in UIs.
        /// </summary>
        Sprite image { get; }

    }
}