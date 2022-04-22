// Copyright (c) Pixel Crushers. All rights reserved.

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// These categories of content may be shown in UIs.
    /// 
    /// While Offer and OfferConditionsUnmet are technically dialogue, they
    /// use separate categories so the quest editor can keep track of their
    /// foldout states separately from the regular dialogue foldout state.
    /// </summary>
    public enum QuestContentCategory
    {
        /// <summary>
        /// Content shown in the dialogue UI.
        /// </summary>
        Dialogue,

        /// <summary>
        /// Content shown in the quest journal UI.
        /// </summary>
        Journal,

        /// <summary>
        /// Content shown in the quest tracker HUD.
        /// </summary>
        HUD,

        /// <summary>
        /// Content shown in the alert UI.
        /// </summary>
        Alert,

        /// <summary>
        /// Content shown in the dialogue UI when the quest giver offers the quest.
        /// </summary>
        Offer,

        /// <summary>
        /// Content shown in the dialogue UI when the offer conditions haven't been met yet.
        /// </summary>
        OfferConditionsUnmet
    }

}
