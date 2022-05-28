// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    public enum QuestMessageParticipant
    {
        /// <summary>
        /// Accepts any object.
        /// </summary>
        Any,

        /// <summary>
        /// Must be the quester assigned to this quest.
        /// </summary>
        Quester,

        /// <summary>
        /// Must be this quest's quest giver.
        /// </summary>
        QuestGiver,

        /// <summary>
        /// Specified by ID.
        /// </summary>
        Other
    }

}
