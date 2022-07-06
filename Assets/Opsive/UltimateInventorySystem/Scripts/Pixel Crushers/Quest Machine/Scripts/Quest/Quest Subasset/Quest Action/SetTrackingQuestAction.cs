// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Tracks or untracks a quest in the HUD.
    /// </summary>
    public class SetTrackingQuestAction : QuestAction
    {

        [Tooltip("ID of quest for which this indicator state applies, or blank for this quest.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("ID of entity whose indicator to set, or blank to set quest giver.")]
        [SerializeField]
        private StringField m_entityID;

        [Tooltip("Track quest in HUD.")]
        [SerializeField]
        private bool m_showInTrackHUD;

        public StringField questID
        {
            get { return m_questID; }
            set { m_questID = value; }
        }

        public StringField entityID
        {
            get { return m_entityID; }
            set { m_entityID = value; }
        }

        public bool showInTrackHUD
        {
            get { return m_showInTrackHUD; }
            set { m_showInTrackHUD = value; }
        }

        public string runtimeEntityID
        {
            get
            {
                var s = StringField.GetStringValue(entityID);
                return (string.IsNullOrEmpty(s) && quest != null) ? StringField.GetStringValue(quest.questGiverID) : QuestMachineTags.ReplaceTags(s, quest);
            }
        }

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(entityID)) return "Set Tracking";
            return "Set Tracking: " + questID + " " + entityID + " " + showInTrackHUD;
        }

        public override void Execute()
        {
            var affectedQuest = StringField.IsNullOrEmpty(questID) ? this.quest : QuestMachine.GetQuestInstance(questID, entityID);
            if (affectedQuest == null) return;
            affectedQuest.showInTrackHUD = showInTrackHUD;
        }

    }

}
