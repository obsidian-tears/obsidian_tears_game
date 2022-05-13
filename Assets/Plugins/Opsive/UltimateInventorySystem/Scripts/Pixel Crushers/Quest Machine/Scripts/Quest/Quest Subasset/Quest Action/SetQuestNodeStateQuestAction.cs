// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sets a quest node state.
    /// </summary>
    public class SetQuestNodeStateQuestAction : QuestAction
    {

        [Tooltip("ID of quest. Leave blank to set this quest's state.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("ID of quest node. Leave blank to set this quest node's state.")]
        [SerializeField]
        private StringField m_questNodeID;

        [Tooltip("New quest node state.")]
        [SerializeField]
        private QuestNodeState m_state;

        /// <summary>
        /// ID of quest. Leave blank to set this quest's state.
        /// </summary>
        public StringField questID
        {
            get { return (StringField.IsNullOrEmpty(m_questID) && quest != null) ? quest.id : m_questID; }
            set { m_questID = value; }
        }

        /// <summary>
        /// ID of quest node. Leave blank to set this quest node's state.
        /// </summary>
        public StringField questNodeID
        {
            get { return (StringField.IsNullOrEmpty(m_questNodeID) && questNode != null) ? questNode.id : m_questNodeID; }
            set { m_questNodeID = value; }
        }

        public QuestNodeState state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public override string GetEditorName()
        {
            if (StringField.IsNullOrEmpty(questID) && StringField.IsNullOrEmpty(questNodeID))
            {
                return "Set Quest Node State: " + state;
            }
            else if (StringField.IsNullOrEmpty(questID))
            {
                return "Set Quest Node State: '" + questNodeID + "' to " + state;
            }
            else if (StringField.IsNullOrEmpty(questNodeID))
            {
                return "Set Quest Node State: Quest '" + questID + "' Node (unspecified) to " + state;
            }
            else
            {
                return "Set Quest Node State: Quest '" + questID + "' Node '" + questNodeID + "' to " + state;
            }
        }

        public override void Execute()
        {
            if (QuestMachine.GetQuestNodeState(questID, questNodeID) != state)
            {
                QuestMachine.SetQuestNodeState(questID, questNodeID, state);
            }
        }

    }

}
