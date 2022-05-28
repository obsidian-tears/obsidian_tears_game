// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Allows quest generator entities to build content using Dialogue System conversations.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/Dialogue System/Generator/Dialogue System Quest Generator Bridge")]
    public class DialogueSystemQuestGeneratorBridge : MonoBehaviour
    {

        [Tooltip("Use conversations specified in this asset when generating quests.")]
        [SerializeField]
        private DialogueSystemActionConversationMap m_actionConversationMap;

        [Tooltip("Apply this component to all Quest Generator Entities.")]
        [SerializeField]
        private bool m_applyToAllGenerators = false;

        public DialogueSystemActionConversationMap actionConversationMap
        {
            get { return m_actionConversationMap; }
            set { m_actionConversationMap = value; }
        }

        public bool applyToAllGenerators
        {
            get { return m_applyToAllGenerators; }
        }

        private DialogueSystemPlanToQuestBuilder m_planToBuilder = null;

        private void Awake()
        {
            m_planToBuilder = new DialogueSystemPlanToQuestBuilder(m_actionConversationMap);
        }

        private void Start()
        {
            if (m_actionConversationMap == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: No DialogueSystemActionConversationMap is assigned to DialogueSystemPlanToQuestBuilder.", this);
            }
            else if (applyToAllGenerators)
            {
                var generators = FindObjectsOfType<QuestGeneratorEntity>();
                for (int i = 0; i < generators.Length; i++)
                {
                    if (generators[i].questGenerator == null) continue;
                    generators[i].questGenerator.planToQuestBuilder = m_planToBuilder;
                }
            }
            else
            {
                var generator = GetComponent<QuestGeneratorEntity>();
                if (generator == null)
                {
                    if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: DialogueSystemPlanToQuestBuilder didn't find a QuestGeneratorEntity on " + name + " and Apply to All Generators isn't ticked.", this);
                }
                else
                {
                    generator.questGenerator.planToQuestBuilder = m_planToBuilder;
                }
            }
        }

    }

}