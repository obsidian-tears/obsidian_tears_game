// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sets a quest state.
    /// </summary>
    public class SetQuestStateQuestAction : QuestAction
    {

        [Tooltip("ID of quest. Leave blank to set this quest's state.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("New quest state.")]
        [SerializeField]
        private QuestState m_state;

        [Tooltip("Set all quest nodes to equivalent state.")]
        [SerializeField]
        private bool m_setQuestNodesToSame = false;

        public StringField questID
        {
            get { return (StringField.IsNullOrEmpty(m_questID) && quest != null) ? quest.id : m_questID; }
            set { m_questID = value; }
        }

        public QuestState state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public bool setQuestNodesToSame
        {
            get { return m_setQuestNodesToSame; }
            set { m_setQuestNodesToSame = value; }
        }

        public override string GetEditorName()
        {
            return StringField.IsNullOrEmpty(questID) ? ("Set Quest State: " + state) : ("Set Quest State: Quest '" + questID + "' to " + state);
        }

        public override void Execute()
        {
            var useThisQuest = StringField.IsNullOrEmpty(questID) && quest != null;
            if (useThisQuest)
            {
                quest.SetState(state);
            }
            else if (QuestMachine.GetQuestState(questID) != state)
            {
                QuestMachine.SetQuestState(questID, state);
            }
            if (setQuestNodesToSame)
            {
                var questToSet = useThisQuest ? quest : QuestMachine.GetQuestInstance(questID);
                if (questToSet != null)
                {
                    var stateToSet = GetEquivalentQuestNodeState(state);
                    for (int i = 0; i < questToSet.nodeList.Count; i++)
                    {
                        questToSet.nodeList[i].SetStateRaw(stateToSet);
                    }
                }
            }
        }

        private QuestNodeState GetEquivalentQuestNodeState(QuestState state)
        {
            switch (state)
            {
                default:
                case QuestState.WaitingToStart:
                case QuestState.Disabled:
                case QuestState.Abandoned:
                    return QuestNodeState.Inactive;
                case QuestState.Active:
                    return QuestNodeState.Active;
                case QuestState.Successful:
                case QuestState.Failed:
                    return QuestNodeState.True;
            }
        }
    }

}
