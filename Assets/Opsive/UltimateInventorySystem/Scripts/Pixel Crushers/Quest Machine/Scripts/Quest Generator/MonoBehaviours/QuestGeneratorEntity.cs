// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A quest generator entity is an entity that can generate quests.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    [RequireComponent(typeof(QuestListContainer))]
    public class QuestGeneratorEntity : QuestEntity
    {

        #region Serialized Fields

        [Tooltip("Organize quests in this group. Leave blank for no group.")]
        [SerializeField]
        private StringField m_questGroup = new StringField();

        [Tooltip("The domain type where this quest giver is located.")]
        [SerializeField]
        private DomainType m_domainType;

        [Tooltip("The domains that this quest giver observes.")]
        [SerializeField]
        private QuestDomain[] m_domains = new QuestDomain[0];

        [Tooltip("Require the quester to speak to the quest giver to finish the quest.")]
        [SerializeField]
        private bool m_requireReturnToComplete = true;

        [Tooltip("Allow generated quests to be abandoned.")]
        [SerializeField]
        private bool m_generateAbandonableQuests = false;

        [SerializeField]
        private UrgentFactSelectionMode m_goalSelectionMode = new UrgentFactSelectionMode(UrgentFactionSelectionCriterion.SameAsGlobalSetting, 1);

        [Tooltip("The UI content to show above the list of rewards offered for a quest.")]
        [SerializeField]
        private BasicUIContent m_rewardsUIContents = new BasicUIContent();

        [SerializeField]
        private List<RewardSystem> m_rewardSystems = new List<RewardSystem>();

        [Tooltip("Generate a quest on start. Note: Will only generate one quest on start. To generate additional, call GenerateQuest() method.")]
        [SerializeField]
        private bool m_generateQuestOnStart;

        [Tooltip("Generate a quest only if the quest list is smaller than this.")]
        [SerializeField]
        private int m_maxQuestsToGenerate = 1;

        [NonSerialized]
        private QuestListContainer m_questListContainer;

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Organize quests in this group. Leave blank for no group.
        /// </summary>
        public StringField questGroup
        {
            get { return m_questGroup; }
            set { m_questGroup = value; }
        }

        /// <summary>
        /// The domain type where this quest giver is located.
        /// </summary>
        public DomainType domainType
        {
            get { return m_domainType; }
            set { m_domainType = value; }
        }

        /// <summary>
        /// Require the quester to speak to the quest giver to finish the quest.
        /// </summary>
        public bool requireReturnToComplete
        {
            get { return m_requireReturnToComplete; }
            set { m_requireReturnToComplete = value; }
        }

        /// <summary>
        /// Allow generated quests to be abandoned
        /// </summary>
        public bool generateAbandonableQuests
        {
            get { return m_generateAbandonableQuests; }
            set { m_generateAbandonableQuests = value; }
        }

        /// <summary>
        /// The domains that this quest giver observes.
        /// </summary>
        public QuestDomain[] domains
        {
            get { return m_domains; }
            set { m_domains = value; }
        }

        public UrgentFactSelectionMode goalSelectionMode
        {
            get { return m_goalSelectionMode; }
            set { m_goalSelectionMode = value; }
        }

        /// <summary>
        /// The UI content to show above the list of rewards offered for a quest.
        /// </summary>
        public BasicUIContent rewardsUIContents
        {
            get { return m_rewardsUIContents; }
            set { m_rewardsUIContents = value; }
        }

        /// <summary>
        /// Reward systems to use to generate rewards.
        /// </summary>
        public List<RewardSystem> rewardSystems
        {
            get { return m_rewardSystems; }
            set { m_rewardSystems = value; }
        }

        /// <summary>
        /// Generate a quest as soon as the component starts.
        /// </summary>
        public bool generateQuestOnStart
        {
            get { return m_generateQuestOnStart; }
            set { m_generateQuestOnStart = value; }
        }

        /// <summary>
        /// Generate a quest only if the quest list is smaller than this
        /// </summary>
        public int maxQuestsToGenerate
        {
            get { return m_maxQuestsToGenerate; }
            set { m_maxQuestsToGenerate = value; }
        }

        public QuestListContainer questListContainer
        {
            get { return m_questListContainer; }
            set { m_questListContainer = value; }
        }

        #endregion

        #region Runtime Properties

        public delegate void UpdateWorldModelDelegate(WorldModel worldModel);

        public event UpdateWorldModelDelegate updateWorldModel = delegate { };

        public event GeneratedQuestDelegate generatedQuest = delegate { };

        public QuestGenerator questGenerator { get; private set; }

        private bool m_isGenerating = false;

        /// <summary>
        /// Is this QuestGeneratorEntity currently generating a quest?
        /// </summary>
        public bool isGenerating
        {
            get { return m_isGenerating; }
            protected set { m_isGenerating = value; }
        }

        #endregion

        #region Initialization

        public override void Awake()
        {
            questGenerator = new QuestGenerator();
            questListContainer = GetComponent<QuestListContainer>();
            RecordRewardSystems();
        }

        public virtual void Start()
        {
            StartCoroutine(GenerateQuestOnStart());
        }

        protected IEnumerator GenerateQuestOnStart()
        {
            yield return null; // Give time for entities to spawn and initialize.
            yield return null;
            if (generateQuestOnStart) GenerateQuest();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (questGenerator != null)
            {
                questGenerator.CancelGeneration();
            }
        }

        /// <summary>
        /// Updates the list of reward systems on this generator entity.
        /// </summary>
        public void RecordRewardSystems()
        {
            foreach (var rewardSystem in GetComponentsInChildren<RewardSystem>())
            {
                if (!rewardSystems.Contains(rewardSystem) && rewardSystem.enabled)
                {
                    rewardSystems.Add(rewardSystem);
                }
            }
        }

        #endregion

        #region Generate Quest

        /// <summary>
        /// Returns the current number of generated quests.
        /// </summary>
        public int GetGeneratedQuestCount()
        {
            int count = 0;
            if (questListContainer == null || questListContainer.questList == null) return 0;
            for (int i = 0; i < questListContainer.questList.Count; i++)
            {
                var quest = questListContainer.questList[i];
                if (quest != null && quest.isProcedurallyGenerated) count++;
            }
            return count;
        }

        /// <summary>
        /// Generates a quest if the current number of generated quests is under the max.
        /// </summary>
        public virtual void GenerateQuest()
        {
            if (m_isGenerating || questListContainer == null || questListContainer.questList == null) return;
            if (GetGeneratedQuestCount() >= maxQuestsToGenerate) return;
            var worldModel = BuildWorldModel();
            questGenerator.GenerateQuest(this, questGroup, domainType, worldModel, requireReturnToComplete, rewardsUIContents.contentList, rewardSystems, questListContainer.questList, OnGeneratedQuest, goalSelectionMode, generateAbandonableQuests);
            m_isGenerating = true;
        }

        protected virtual void OnGeneratedQuest(Quest quest)
        {
            m_isGenerating = false;
            if (quest != null)
            {
                if (GetGeneratedQuestCount() < maxQuestsToGenerate)
                {
                    GetComponent<QuestGiver>().AddQuest(quest);
                    generatedQuest(quest);
                }
                else
                {
                    Quest.DestroyInstance(quest);
                }
            }
        }

        /// <summary>
        /// Returns the world model as observed by this entity.
        /// </summary>
        public virtual WorldModel BuildWorldModel()
        {
            var worldModel = BuildWorldModelFromDomains();
            updateWorldModel(worldModel);
            return worldModel;
        }

        public virtual WorldModel BuildWorldModelFromDomains()
        {
            var worldModel = new WorldModel(new Fact(domainType, entityType, 1)); //[TODO] Pool.
            if (domains != null)
            {
                for (int i = 0; i < domains.Length; i++)
                {
                    var domain = domains[i];
                    if (domain != null) domain.AddEntitiesToWorldModel(worldModel);
                }
            }
            return worldModel;
        }

        #endregion

        #region Start Dialogue

        /// <summary>
        /// Starts dialogue with the first GameObject in the scene that's tagged as "Player".
        /// Generates a quest if necessary.
        /// </summary>
        public virtual void StartDialogueWithPlayer()
        {
            StartDialogue(GameObject.FindWithTag("Player"));
        }

        /// <summary>
        /// Starts dialogue with the player. Works like QuestGiver.StartDialogue but generates
        /// a quest if necessary.
        /// </summary>
        /// <param name="player">Player conversing with this QuestGiver. If null, searches the scene for a GameObject tagged Player.</param>
        public virtual void StartDialogue(GameObject player)
        {
            if (player == null) return;
            StartCoroutine(GenerateQuestThenTalk(player));
        }

        protected IEnumerator GenerateQuestThenTalk(GameObject player)
        {
            if (player == null) yield break;
            var questJournal = player.GetComponentInChildren<QuestJournal>();
            var questGiver = GetComponent<QuestGiver>();
            if (questGiver == null || questJournal == null) yield break;
            if (questGiver.questList.Count == 0 && !IsMyQuestActive(questJournal))
            {
                GenerateQuest();
                var maxTime = Time.time + 1; // Give 1 second to generate quest.
                while (questGiver.questList.Count == 0 && Time.time < maxTime)
                {
                    yield return null;
                }
            }
            questGiver.StartDialogue(player);
        }

        /// <summary>
        /// Checks if the quest journal has an active quest with this GameObject's quest giver ID.
        /// </summary>
        public virtual bool IsMyQuestActive(QuestJournal questJournal)
        {
            var questGiver = GetComponent<QuestGiver>();
            if (questGiver == null || questJournal == null) return false;
            return questJournal.questList.Find(quest => quest.GetState() == QuestState.Active && quest.questGiverID == questGiver.id) != null;
        }

        #endregion

    }

}
