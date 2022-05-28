// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Gives a quest to a quester and activates it.
    /// </summary>
    public class GiveQuestToQuesterQuestAction : QuestAction
    {
        [HelpBox("Gives a quest to a quester and activates it. Leave Quester ID blank to give to the default player quest journal. Assigns this quest's quest giver as the quest giver.", HelpBoxMessageType.None)]

        [Tooltip("Quest to give to quester. If assigned, you can leave Quest ID To Give unassigned. Quest To Give takes precedence over Quest ID To Give.")]
        [SerializeField]
        private Quest m_questToGive;

        [Tooltip("ID of quest to give to quester. If assigned, you can leave Quest To Give unassigned. Quest To Give takes precedence over Quest ID To Give.")]
        [SerializeField]
        private StringField m_questIDToGive;

        [Tooltip("Leave blank to give to default player quest journal.")]
        [SerializeField]
        [FormerlySerializedAs("questerID")]
        private StringField m_questerID = new StringField();

        public Quest questToGive
        {
            get { return m_questToGive; }
            set { m_questToGive = value; }
        }

        public StringField questIDToGive
        {
            get { return m_questIDToGive; }
            set { m_questIDToGive = value; }
        }

        public StringField questerID
        {
            get { return m_questerID; }
            set { m_questerID = value; }
        }

        public override string GetEditorName()
        {
            var questText = (questToGive != null) ? ("'" + questToGive.id + "'")
            : !StringField.IsNullOrEmpty(questIDToGive) ? ("'" + questIDToGive + "'")
            : "<none>";
            var questerText = StringField.IsNullOrEmpty(questerID) ? "Player" : questerID.value;
            return "Give Quest " + questText + " to " + questerText;
        }

        public override void Execute()
        {
            base.Execute();
            if (QuestMachine.debug)
            {
                Debug.Log("Quest Machine: " + GetEditorName());
            }
            var questInstance = (questToGive != null) ? QuestMachine.GiveQuestToQuester(questToGive, questerID)
                : !StringField.IsNullOrEmpty(questIDToGive) ? QuestMachine.GiveQuestToQuester(questIDToGive, questerID)
                : null;
            if (questInstance != null)
            {
                questInstance.questGiverID = quest.questGiverID;
                if (!StringField.IsNullOrEmpty(quest.questGiverID))
                {
                    questInstance.tagDictionary.SetTag(QuestMachineTags.QUESTGIVERID, quest.questGiverID);
                    questInstance.tagDictionary.SetTag(QuestMachineTags.QUESTGIVER, quest.tagDictionary.GetTagValue(QuestMachineTags.QUESTGIVER, string.Empty));
                }
            }
        }
    }

}
