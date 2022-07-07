// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sets a quest indicator.
    /// </summary>
    public class SetIndicatorQuestAction : QuestAction
    {

        [Tooltip("ID of quest for which this indicator state applies, or blank for this quest.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("ID of entity whose indicator to set, or blank to set quest giver.")]
        [SerializeField]
        private StringField m_entityID;

        [Tooltip("Quest indicator state to set on the entity.")]
        [SerializeField]
        private QuestIndicatorState m_questIndicatorState;

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

        public QuestIndicatorState questIndicatorState
        {
            get { return m_questIndicatorState; }
            set { m_questIndicatorState = value; }
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
            if (StringField.IsNullOrEmpty(entityID)) return "Set Indicator";
            return "Set Indicator: " + questID + " " + entityID + " " + questIndicatorState;
        }

        public override void Execute()
        {
            var affectedQuest = StringField.IsNullOrEmpty(questID) ? this.quest : QuestMachine.GetQuestInstance(questID, entityID);
            if (affectedQuest == null) return;
            affectedQuest.SetQuestIndicatorState(runtimeEntityID, questIndicatorState);
        }

    }

}
