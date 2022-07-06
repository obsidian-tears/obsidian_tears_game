// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Maps Quest Machine actions to Dialogue System conversations. Used by the 
    /// Dialogue System integration's replacement PlanToQuestBuilder.
    /// </summary>
    [CreateAssetMenu(menuName = "Pixel Crushers/Quest Machine/Generator/Dialogue System Action Conversation Map")]
    public class DialogueSystemActionConversationMap : ScriptableObject
    {

        [Tooltip("The display name of this action.")]
        [SerializeField]
        private List<DialogueSystemActionConversationMapRecord> m_records = new List<DialogueSystemActionConversationMapRecord>();

        public List<DialogueSystemActionConversationMapRecord> records
        {
            get { return m_records; }
            set { m_records = value; }
        }

        public DialogueSystemActionConversationMapRecord GetRecordForAction(PixelCrushers.QuestMachine.Action action)
        {
            return records.Find(x => x.action == action);
        }

    }
}