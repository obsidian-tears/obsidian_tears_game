// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest object. May be saved as an asset file in the project or may be a 
    /// runtime instance in the scene.
    /// </summary>
    //---  No [CreateAssetMenu]. Hide from asset menu; use wrapper class instead.
    public class Quest : ScriptableObject
    {

        #region Serialized Fields

        [Tooltip("Quest is a runtime instance, not an asset file.")]
        [SerializeField]
        private bool m_isInstance;

        [Tooltip("If a runtime instance, this is the original asset from which the instance was created.")]
        [SerializeField]
        private Quest m_originalAsset;

        [SerializeField]
        private int m_fileVersion = 0;

        [Tooltip("Unique identifier for this quest.")]
        [SerializeField]
        private StringField m_id;

        [Tooltip("Title shown in UIs.")]
        [SerializeField]
        private StringField m_title;

        [Tooltip("Optional icon shown in UIs.")]
        [SerializeField]
        private Sprite m_icon;

        [Tooltip("Optional group under which to categorize this quest.")]
        [SerializeField]
        private StringField m_group;

        [Tooltip("Optional labels to assign to this quest for sorting and filtering.")]
        [SerializeField]
        private List<StringField> m_labels;

        [Tooltip("ID of the quest giver that offered this quest. Typically set on the quester's runtime instance of the quest when the quester accepts a quest.")]
        [SerializeField]
        private StringField m_questGiverID;

        [Tooltip("Allow the player to toggle tracking on and off.")]
        [SerializeField]
        private bool m_isTrackable;

        [Tooltip("Show in the quest HUD.")]
        [SerializeField]
        private bool m_showInTrackHUD;

        [Tooltip("Allow the player to abandon the quest.")]
        [SerializeField]
        private bool m_isAbandonable;

        [Tooltip("Keep in quest journal if abandoned.")]
        [SerializeField]
        private bool m_rememberIfAbandoned;

        [Tooltip("If specified, conditions that autostart the quest when true.")]
        [SerializeField]
        private QuestConditionSet m_autostartConditionSet;

        [Tooltip("Conditions that must be true before the quest can be offered.")]
        [SerializeField]
        private QuestConditionSet m_offerConditionSet;

        [Tooltip("Show this dialogue content when the offer conditions are unmet.")]
        [SerializeField]
        private List<QuestContent> m_offerConditionsUnmetContentList;

        [Tooltip("Show this dialogue content to offer the quest.")]
        [SerializeField]
        private List<QuestContent> m_offerContentList;

        [Tooltip("Max number of times this quest can be accepted.")]
        [SerializeField]
        private int m_maxTimes;

        [Tooltip("Number of times the quest has been accepted.")]
        [SerializeField]
        private int m_timesAccepted;

        [Tooltip("Minimum duration in seconds that must pass after quest acceptance to offer it again.")]
        [SerializeField]
        private float m_cooldownSeconds;

        [Tooltip("Seconds remaining until cooldown period is over.")]
        [SerializeField]
        private float m_cooldownSecondsRemaining;

        [Tooltip("Do not offer again if quester already has successful completion in its journal.")]
        [SerializeField]
        private bool m_noRepeatIfSuccessful = false;

        [Tooltip("Save counters, node states, & indicator states if waiting to start. Normally omitted to reduce save file size/time.")]
        [SerializeField]
        private bool m_saveAllIfWaitingToStart = false;

        [Tooltip("The current state of the quest.")]
        [SerializeField]
        private QuestState m_state;

        [Tooltip("State info, indexed by the int value of the QuestState enum.")]
        [SerializeField]
        private List<QuestStateInfo> m_stateInfoList;

        [Tooltip("Counters defined for this quest.")]
        [SerializeField]
        private List<QuestCounter> m_counterList;

        [Tooltip("All quest nodes in this quest.")]
        [SerializeField]
        private List<QuestNode> m_nodeList;

        [HideInInspector]
        [SerializeField]
        private string m_goalEntityTypeName = null;

        [HideInInspector]
        [SerializeField]
        private int m_nextContentID = 0;

        #endregion

        #region Property Accessors to Serialized Fields

        /// <summary>
        /// Quest is a runtime instance, not an asset file.
        /// </summary>
        public bool isInstance
        {
            get { return m_isInstance; }
            set { m_isInstance = value; }
        }

        /// <summary>
        /// Quest is an asset, not a runtime instance.
        /// </summary>
        public bool isAsset
        {
            get { return !isInstance; }
            set { isInstance = !value; }
        }

        /// <summary>
        /// If a runtime instance, this is the original asset from which the instance was created.
        /// </summary>
        public Quest originalAsset
        {
            get { return isInstance ? m_originalAsset : this; }
            set { m_originalAsset = value; }
        }

        public int fileVersion
        {
            get { return m_fileVersion; }
            set { m_fileVersion = value; }
        }

        /// <summary>
        /// Quest was procedurally generated.
        /// </summary>
        public bool isProcedurallyGenerated
        {
            get { return isInstance && originalAsset == null; }
        }

        /// <summary>
        /// Unique identifier for this quest.
        /// </summary>
        public StringField id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Title shown in UIs.
        /// </summary>
        public StringField title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        /// <summary>
        /// Optional quest icon shown in UIs.
        /// </summary>
        public Sprite icon
        {
            get { return m_icon; }
            set { m_icon = value; }
        }

        /// <summary>
        /// Optional group under which to categorize this quest.
        /// </summary>
        public StringField group
        {
            get { return m_group; }
            set { m_group = value; }
        }

        /// <summary>
        /// Optional labels to assign to this quest for sorting and filtering.
        /// </summary>
        public List<StringField> labels
        {
            get { return m_labels; }
            set { m_labels = value; }
        }

        /// <summary>
        /// ID of the quest giver that offered this quest. Typically set on the quester's 
        /// runtime instance of the quest when the quester accepts a quest. If this is an
        /// asset or it hasn't been accepted yet, questGiverID will be empty.
        /// </summary>
        public StringField questGiverID
        {
            get { return m_questGiverID; }
            set { m_questGiverID = value; }
        }

        /// <summary>
        /// ID of the quester assigned to this quest. If this is an asset or it hasn't been
        /// accepted yet, questerID will be empty;
        /// </summary>
        public string questerID
        {
            get { return tagDictionary.GetTagValue(QuestMachineTags.QUESTERID, string.Empty); }
        }

        /// <summary>
        /// ID of the quester who is greeting the quest giver. For quests that can be accepted
        /// by quester A and turned in by quester B, this is the ID of quester B.
        /// </summary>
        public string greeterID
        {
            get { return tagDictionary.GetTagValue(QuestMachineTags.GREETERID, string.Empty); }
            set { tagDictionary.SetTag(QuestMachineTags.GREETERID, value); }
        }

        /// <summary>
        /// Display name of the quester who is greeting the quest giver. For quests that can be accepted
        /// by quester A and turned in by quester B, this is the name of quester B.
        /// </summary>
        public string greeter
        {
            get { return tagDictionary.GetTagValue(QuestMachineTags.GREETER, string.Empty); }
            set { tagDictionary.SetTag(QuestMachineTags.GREETER, value); }
        }

        /// <summary>
        /// Specifies whether the player is allowed to toggle tracking on and off.
        /// </summary>
        public bool isTrackable
        {
            get { return m_isTrackable; }
            set { m_isTrackable = value; }
        }

        /// <summary>
        /// Specifies whether to show in the quest tracking HUD.
        /// </summary>
        public bool showInTrackHUD
        {
            get { return m_showInTrackHUD; }
            set
            {
                m_showInTrackHUD = value;
                QuestMachineMessages.QuestTrackToggleChanged(this, id, value);
            }
        }

        /// <summary>
        /// Specifies whether the player is allowed to abandon the quest.
        /// </summary>
        public bool isAbandonable
        {
            get { return m_isAbandonable; }
            set { m_isAbandonable = value; }
        }

        /// <summary>
        /// Specifies whether to keep in quest journal if abandoned.
        /// </summary>
        public bool rememberIfAbandoned
        {
            get { return m_rememberIfAbandoned; }
            set { m_rememberIfAbandoned = value; }
        }

        /// <summary>
        /// If specified, conditions that autostart the quest when true.
        /// </summary>
        public QuestConditionSet autostartConditionSet
        {
            get { return m_autostartConditionSet; }
            set { m_autostartConditionSet = value; }
        }

        /// <summary>
        /// If true, the quest has autostart conditions. The quest will start 
        /// automatically when the conditions are met.
        /// </summary>
        public bool hasAutostartConditions
        {
            get { return QuestConditionSet.ConditionCount(autostartConditionSet) > 0; }
        }

        /// <summary>
        /// Conditions that must be true before the quest can be offered.
        /// </summary>
        public QuestConditionSet offerConditionSet
        {
            get { return m_offerConditionSet; }
            set { m_offerConditionSet = value; }
        }

        /// <summary>
        /// If true, the quest has offer conditions. The giver should not offer the 
        /// quest until the conditions are met.
        /// </summary>
        public bool hasOfferConditions
        {
            get { return QuestConditionSet.ConditionCount(offerConditionSet) > 0; }
        }

        /// <summary>
        /// If true, the giver can offer the quest.
        /// </summary>
        public bool canOffer
        {
            get { return (!hasOfferConditions || offerConditionSet.areConditionsMet) && (timesAccepted < maxTimes) && (cooldownSecondsRemaining <= 0); }
        }

        /// <summary>
        /// Dialogue text to show when the offer conditions are unmet.
        /// </summary>
        public List<QuestContent> offerConditionsUnmetContentList
        {
            get { return m_offerConditionsUnmetContentList; }
            set { m_offerConditionsUnmetContentList = value; }
        }

        /// <summary>
        /// Dialogue text to show when offering the quest.
        /// </summary>
        public List<QuestContent> offerContentList
        {
            get { return m_offerContentList; }
            set { m_offerContentList = value; }
        }

        /// <summary>
        /// Max number of times this quest can be accepted.
        /// </summary>
        public int maxTimes
        {
            get { return m_maxTimes; }
            set { m_maxTimes = value; }
        }

        /// <summary>
        /// The number of times the quest has been accepted.
        /// </summary>
        public int timesAccepted
        {
            get { return m_timesAccepted; }
            set { m_timesAccepted = value; }
        }

        /// <summary>
        /// Minimum duration in seconds that must pass after quest acceptance to 
        /// offer it again.
        /// </summary>
        public float cooldownSeconds
        {
            get { return m_cooldownSeconds; }
            set { m_cooldownSeconds = value; }
        }

        /// <summary>
        /// Seconds remaining until cooldown period is over.
        /// </summary>
        public float cooldownSecondsRemaining
        {
            get { return m_cooldownSecondsRemaining; }
            set { m_cooldownSecondsRemaining = value; }
        }

        /// <summary>
        /// Do not offer again if quester already has successful completion in its journal.
        /// </summary>
        public bool noRepeatIfSuccessful
        {
            get { return m_noRepeatIfSuccessful; }
            set { m_noRepeatIfSuccessful = value; }
        }

        /// <summary>
        /// Save counters, node states, & indicator states if waiting to start. Normally omitted to reduce save file size/time.
        /// </summary>
        public bool saveAllIfWaitingToStart
        {
            get { return m_saveAllIfWaitingToStart; }
            set { m_saveAllIfWaitingToStart = value; }
        }

        /// <summary>
        /// Info for each state, indexed by the int value of the QuestState enum.
        /// </summary>
        public List<QuestStateInfo> stateInfoList
        {
            get { return m_stateInfoList; }
            set { m_stateInfoList = value; }
        }

        /// <summary>
        /// Counters defined for this quest.
        /// </summary>
        public List<QuestCounter> counterList
        {
            get { return m_counterList; }
            set { m_counterList = value; }
        }

        /// <summary>
        /// All nodes in this quest.
        /// </summary>
        public List<QuestNode> nodeList
        {
            get { return m_nodeList; }
            set { m_nodeList = value; }
        }

        /// <summary>
        /// The quest's start node.
        /// </summary>
        public QuestNode startNode
        {
            get { return (m_nodeList != null && m_nodeList.Count > 0) ? m_nodeList[0] : null; }
        }

        /// <summary>
        /// If this quest was procedurally generated, the goal EntityType's name.
        /// </summary>
        public string goalEntityTypeName
        {
            get { return m_goalEntityTypeName; }
            set { m_goalEntityTypeName = value; }
        }

        /// <summary>
        /// Used by QuestContent to assign a unique ID number usable by LinkQuestContent.
        /// </summary>
        public int nextContentID
        {
            get { return m_nextContentID; }
            set { m_nextContentID = value; }
        }

        #endregion

        #region Runtime References

        [NonSerialized]
        private float m_timeCooldownLastChecked;

        [NonSerialized]
        private TagDictionary m_tagDictionary = new TagDictionary();

        [NonSerialized]
        private Dictionary<string, QuestIndicatorState> m_questIndicatorStates = new Dictionary<string, QuestIndicatorState>();

        [NonSerialized]
        private HashSet<string> m_speakers = new HashSet<string>();

        [NonSerialized]
        private Dictionary<int, QuestContent> m_questContentByID = new Dictionary<int, QuestContent>();

        /// <summary>
        /// Dictionary of tags defined in this quest and their values.
        /// </summary>
        public TagDictionary tagDictionary
        {
            get { return m_tagDictionary; }
            set { m_tagDictionary = value; }
        }

        /// <summary>
        /// Current quest state indicator states by entity ID.
        /// </summary>
        public Dictionary<string, QuestIndicatorState> indicatorStates
        {
            get { return m_questIndicatorStates; }
            set { m_questIndicatorStates = value; }
        }

        /// <summary>
        /// List of all quest node speakers.
        /// </summary>
        public HashSet<string> speakers
        {
            get { return m_speakers; }
            set { m_speakers = value; }
        }

        private QuestParticipantTextInfo m_currentSpeaker = null;

        /// <summary>
        /// The current speaker's info, if the speaker is different
        /// from the quest giver. If the quest giver is speaking, this
        /// property will be null.
        /// </summary>
        public QuestParticipantTextInfo currentSpeaker
        {
            get { return m_currentSpeaker; }
            private set { m_currentSpeaker = value; }
        }

        /// <summary>
        /// Raised when the quest has become offerable.
        /// </summary>
        public event QuestParameterDelegate questOfferable = delegate { };

        /// <summary>
        /// Raised when the quest's state has changed.
        /// </summary>
        public event QuestParameterDelegate stateChanged = delegate { };

        #endregion

        #region Editor

        public string GetEditorName()
        {
            if (!StringField.IsNullOrEmpty(title)) return title.value;
            if (!StringField.IsNullOrEmpty(id)) return id.value;
            return "Unnamed Quest";
        }

        #endregion

        #region Initialization & Destruction

        /// <summary>
        /// Initializes a quest to empty starting values. Invoked when object is 
        /// created by ScriptableObjectUtility.CreateInstance.
        /// </summary>
        public void Initialize()
        {
            // (isInstance & originalAsset are not set here.)
            var instanceID = GetInstanceID();
            id = new StringField("Quest" + instanceID);
            title = new StringField("Quest " + instanceID);
            icon = null;
            group = new StringField();
            labels = new List<StringField>();
            questGiverID = new StringField();
            isTrackable = true;
            showInTrackHUD = true;
            isAbandonable = false;
            rememberIfAbandoned = false;
            autostartConditionSet = new QuestConditionSet();
            offerConditionSet = new QuestConditionSet();
            offerConditionsUnmetContentList = new List<QuestContent>();
            offerContentList = new List<QuestContent>();
            maxTimes = 1;
            timesAccepted = 0;
            cooldownSeconds = 3600;
            cooldownSecondsRemaining = 0;
            m_state = QuestState.WaitingToStart;
            var numStates = Enum.GetNames(typeof(QuestState)).Length;
            stateInfoList = new List<QuestStateInfo>();
            for (int i = 0; i < numStates; i++)
            {
                stateInfoList.Add(new QuestStateInfo());
            }
            counterList = new List<QuestCounter>();
            var startNode = new QuestNode();
            startNode.InitializeAsStartNode(id.value);
            nodeList = new List<QuestNode>();
            nodeList.Add(startNode);
        }

        /// <summary>
        /// Returns a new instance of the quest, including new instances of all subassets
        /// such as QuestAction, QuestCondition, and QuestContent subassets.
        /// </summary>
        public Quest Clone()
        {
            var clone = Instantiate(this);
            SetRuntimeReferences(); // Fix original's references since Instantiate calls OnEnable > SetRuntimeReferences while clone's fields still point to original.
            clone.isInstance = true;
            clone.originalAsset = originalAsset;
            clone.cooldownSecondsRemaining = 0;
            autostartConditionSet.CloneSubassetsInto(clone.autostartConditionSet);
            offerConditionSet.CloneSubassetsInto(clone.offerConditionSet);
            clone.offerConditionsUnmetContentList = QuestSubasset.CloneList(offerConditionsUnmetContentList);
            clone.offerContentList = QuestSubasset.CloneList(offerContentList);
            QuestStateInfo.CloneSubassets(stateInfoList, clone.stateInfoList);
            QuestNode.CloneSubassets(nodeList, clone.nodeList);
            tagDictionary.CopyInto(clone.tagDictionary);
            clone.SetRuntimeReferences();
            return clone;
        }

        private void OnDestroy()
        {
            if (isInstance && Application.isPlaying)
            {
                QuestMachine.UnregisterQuestInstance(this);
                SetState(QuestState.Disabled);
                if (autostartConditionSet != null) autostartConditionSet.DestroySubassets();
                if (offerConditionSet != null) offerConditionSet.DestroySubassets();
                QuestSubasset.DestroyList(offerConditionsUnmetContentList);
                QuestSubasset.DestroyList(offerContentList);
                QuestStateInfo.DestroyListSubassets(stateInfoList);
                QuestNode.DestroyListSubassets(nodeList);
            }
        }

        public static void DestroyInstance(Quest quest)
        {
            if (quest != null && quest.isInstance)
            {
                if (quest.GetState() != QuestState.Disabled) quest.SetState(QuestState.Disabled);
                Destroy(quest);
            }
        }

        private void OnEnable()
        {
            SetRuntimeReferences();
        }

        /// <summary>
        /// Sets sub-objects' runtime references to this quest.
        /// </summary>
        public void SetRuntimeReferences()
        {
            // Set references in start info:
            if (Application.isPlaying) m_timeCooldownLastChecked = GameTime.time;
            if (autostartConditionSet != null) autostartConditionSet.SetRuntimeReferences(this, null);
            if (offerConditionSet != null) offerConditionSet.SetRuntimeReferences(this, null);
            QuestContent.SetRuntimeReferences(offerConditionsUnmetContentList, this, null);
            QuestContent.SetRuntimeReferences(offerContentList, this, null);

            // Set references in counters:
            if (counterList != null)
            {
                for (int i = 0; i < counterList.Count; i++)
                {
                    counterList[i].SetRuntimeReferences(this);
                }
            }

            // Set references in state info:
            if (stateInfoList != null)
            {
                for (int i = 0; i < stateInfoList.Count; i++)
                {
                    var stateInfo = QuestStateInfo.GetStateInfo(stateInfoList, (QuestState)i);
                    if (stateInfo != null) stateInfo.SetRuntimeReferences(this, null);
                }
            }

            // Set references in nodes:
            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i] != null) nodeList[i].InitializeRuntimeReferences(this);
                }
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i] != null) nodeList[i].ConnectRuntimeNodeReferences();
                }
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i] != null) nodeList[i].SetRuntimeNodeReferences();
                }
            }

            // Record list of any nodes' speakers who aren't the quest giver:
            RecordSpeakersUsedInQuestAndAnyNodes();

            // Add tags to dictionary:
            QuestMachineTags.AddTagsToDictionary(tagDictionary, title);
            QuestMachineTags.AddTagsToDictionary(tagDictionary, group);
            if (!StringField.IsNullOrEmpty(questGiverID)) tagDictionary.SetTag(QuestMachineTags.QUESTGIVERID, questGiverID);
        }

        /// <summary>
        /// Populates the speakers list with the speakers used in the quest and
        /// any of its nodes.
        /// </summary>
        private void RecordSpeakersUsedInQuestAndAnyNodes()
        {
            if (speakers == null) speakers = new HashSet<string>();
            speakers.Clear();
            if (!StringField.IsNullOrEmpty(questGiverID)) speakers.Add(StringField.GetStringValue(questGiverID));
            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i] == null) continue;
                var speaker = StringField.GetStringValue(nodeList[i].speaker);
                if (string.IsNullOrEmpty(speaker) || speakers.Contains(speaker)) continue;
                speakers.Add(speaker);
            }
        }

        /// <summary>
        /// Assigns a quest giver to the quest.
        /// </summary>
        /// <param name="questGiverTextInfo">Identifying information about the quest giver.</param>
        public void AssignQuestGiver(QuestParticipantTextInfo questGiverTextInfo)
        {
            if (questGiverTextInfo == null) return;
            questGiverID = questGiverTextInfo.id;
            if (!StringField.IsNullOrEmpty(questGiverTextInfo.id)) speakers.Add(StringField.GetStringValue(questGiverTextInfo.id));
            QuestMachineTags.AddTagValuesToDictionary(tagDictionary, questGiverTextInfo.textTable);
            tagDictionary.SetTag(QuestMachineTags.QUESTGIVERID, questGiverTextInfo.id);
            tagDictionary.SetTag(QuestMachineTags.QUESTGIVER, questGiverTextInfo.displayName);
        }

        /// <summary>
        /// Assigns a quester (e.g., player) to the quest.
        /// </summary>
        /// <param name="questerTextInfo">Idenntifying information about the quester.</param>
        public void AssignQuester(QuestParticipantTextInfo questerTextInfo)
        {
            if (questerTextInfo == null || StringField.IsNullOrEmpty(questerTextInfo.id)) return;
            tagDictionary.SetTag(QuestMachineTags.QUESTERID, questerTextInfo.id);
            tagDictionary.SetTag(QuestMachineTags.QUESTER, questerTextInfo.displayName);
        }

        #endregion

        #region Startup

        /// <summary>
        /// Invoke to tell the quest to perform its runtime startup actions.
        /// </summary>
        public void RuntimeStartup()
        {
            if (Application.isPlaying) SetState(m_state, !QuestMachine.isLoadingGame);
        }

        /// <summary>
        /// Begins checking autostart and offer conditions.
        /// </summary>
        public void SetStartChecking(bool enable)
        {
            if (!Application.isPlaying) return;
            if (enable)
            {
                SetRandomCounterValues();
                if (hasAutostartConditions) autostartConditionSet.StartChecking(Autostart);
                if (GetState() == QuestState.WaitingToStart) // Autostart check above might set quest active.
                {
                    if (hasOfferConditions)
                    {
                        offerConditionSet.StartChecking(BecomeOfferable);
                    }
                    else
                    {
                        BecomeOfferable();
                    }
                }
            }
            else
            {
                if (hasAutostartConditions) autostartConditionSet.StopChecking();
                if (hasOfferConditions) offerConditionSet.StopChecking();
            }
        }

        private void Autostart()
        {
            SetState(QuestState.Active);
        }

        public void BecomeOfferable()
        {
            try
            {
                if (GetState() != QuestState.WaitingToStart) SetState(QuestState.WaitingToStart);
                questOfferable(this);
                SetQuestIndicatorState(questGiverID, QuestIndicatorState.Offer);
            }
            catch (Exception e) // Don't let exceptions in user-added events break our code.
            {
                if (Debug.isDebugBuild) Debug.LogException(e);
            }
        }

        public void BecomeUnofferable()
        {
            try
            {
                if (GetState() != QuestState.Disabled) SetState(QuestState.Disabled);
                SetQuestIndicatorState(questGiverID, QuestIndicatorState.None);
            }
            catch (Exception e) // Don't let exceptions in user-added events break our code.
            {
                if (Debug.isDebugBuild) Debug.LogException(e);
            }
        }

        /// <summary>
        /// Starts the cooldown period for this quest.
        /// </summary>
        public void StartCooldown()
        {
            if (cooldownSeconds <= 0) return;
            cooldownSecondsRemaining = cooldownSeconds;
            m_timeCooldownLastChecked = GameTime.time;
        }

        /// <summary>
        /// Checks the current game time and updates the cooldown period.
        /// </summary>
        public void UpdateCooldown()
        {
            if (cooldownSecondsRemaining <= 0) return;
            var elapsed = GameTime.time - m_timeCooldownLastChecked;
            m_timeCooldownLastChecked = GameTime.time;
            cooldownSecondsRemaining = Mathf.Max(0, cooldownSecondsRemaining - elapsed);
            if (cooldownSecondsRemaining <= 0) BecomeOfferable();
        }

        #endregion

        #region Quest State

        /// <summary>
        /// Gets the quest state.
        /// </summary>
        /// <returns>The current quest state. Each quest node also has its own state.</returns>
        public QuestState GetState()
        {
            return m_state;
        }

        /// <summary>
        /// Sets the quest state. This may also affect the states of the quest's nodes.
        /// </summary>
        /// <param name="newState">The new quest state.</param>
        public void SetState(QuestState newState, bool informListeners = true)
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + GetEditorName() + ".SetState(" + newState + ", informListeners=" + informListeners + ")", this);

            m_state = newState;

            SetStartChecking(m_state == QuestState.WaitingToStart);
            SetCounterListeners(m_state == QuestState.Active || (m_state == QuestState.WaitingToStart && (hasAutostartConditions || hasOfferConditions)));
            if (m_state != QuestState.Active) StopNodeListeners();

            if (!informListeners) return;

            // Execute state actions:
            ExecuteStateActions(m_state);

            // Notify that state changed:
            QuestMachineMessages.QuestStateChanged(this, id, m_state);
            try
            {
                stateChanged(this);
            }
            catch (Exception e) // Don't let exceptions in user-added events break our code.
            {
                if (Debug.isDebugBuild) Debug.LogException(e);
            }

            // If going active, activate the start node:
            if (m_state == QuestState.Active && startNode != null) startNode.SetState(QuestNodeState.Active);

            // If inactive, clear the indicators:
            if (m_state != QuestState.Active) ClearQuestIndicatorStates();

            // If done, set all active nodes to inactive:
            if (m_state == QuestState.Successful || m_state == QuestState.Failed || m_state == QuestState.Abandoned)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].GetState() == QuestNodeState.Active) nodeList[i].SetState(QuestNodeState.Inactive);
                }

                if (QuestMachineConfiguration.instance != null && QuestMachineConfiguration.instance.untrackCompletedQuests)
                {
                    showInTrackHUD = false;
                }
            }
        }

        /// <summary>
        /// Sets the internal state value without performing any state change processing.
        /// </summary>
        public void SetStateRaw(QuestState state)
        {
            m_state = state;
        }

        /// <summary>
        /// Returns the state info associated with a quest state.
        /// </summary>
        public QuestStateInfo GetStateInfo(QuestState state)
        {
            return (stateInfoList != null) ? QuestStateInfo.GetStateInfo(stateInfoList, state) : null;
        }

        private void StopNodeListeners()
        {
            if (nodeList == null) return;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i] != null) nodeList[i].SetConditionChecking(false);
            }
        }

        public void ExecuteStateActions(QuestState state)
        {
            var stateInfo = GetStateInfo(state);
            if (stateInfo != null && stateInfo.actionList != null)
            {
                for (int i = 0; i < stateInfo.actionList.Count; i++)
                {
                    if (stateInfo.actionList[i] != null) stateInfo.actionList[i].Execute();
                }
            }
        }

        #endregion

        #region Counters

        private void SetRandomCounterValues()
        {
            if (counterList == null) return;
            for (int i = 0; i < counterList.Count; i++)
            {
                var counter = counterList[i];
                if (counter != null && counter.randomizeInitialValue) counter.InitializeToRandomValue();
            }
        }

        private void SetCounterListeners(bool enable)
        {
            if (counterList == null) return;
            for (int i = 0; i < counterList.Count; i++)
            {
                if (counterList[i] != null) counterList[i].SetListeners(enable);
            }
        }

        /// <summary>
        /// Gets a counter defined in this quest.
        /// </summary>
        /// <param name="index">The index of the counter defined in the quest.</param>
        /// <returns>The counter, or null if there is no counter with the specified name.</returns>
        public QuestCounter GetCounter(int index)
        {
            return (counterList != null && 0 <= index && index < counterList.Count) ? counterList[index] : null;
        }

        /// <summary>
        /// Gets a counter defined in this quest.
        /// </summary>
        /// <param name="counterName">The name of the counter defined in the quest.</param>
        /// <returns>The counter, or null if there is no counter with the specified name.</returns>
        public QuestCounter GetCounter(string counterName)
        {
            if (counterList == null) return null;
            for (int i = 0; i < counterList.Count; i++)
            {
                var counter = counterList[i];
                if (counter != null && StringField.Equals(counter.name, counterName)) return counter;
            }
            return null;
        }

        /// <summary>
        /// Gets a counter defined in this quest.
        /// </summary>
        /// <param name="counterName">The name of the counter defined in the quest.</param>
        /// <returns>The counter, or null if there is no counter with the specified name.</returns>
        public QuestCounter GetCounter(StringField counterName)
        {
            return GetCounter(StringField.GetStringValue(counterName));
        }

        public int GetCounterIndex(string counterName)
        {
            if (counterList == null) return -1;
            for (int i = 0; i < counterList.Count; i++)
            {
                var counter = counterList[i];
                if (counter != null && StringField.Equals(counter.name, counterName)) return i;
            }
            return -1;
        }

        public int GetCounterIndex(StringField counterName)
        {
            return GetCounterIndex(StringField.GetStringValue(counterName));
        }

        #endregion

        #region Nodes

        /// <summary>
        /// Looks up a node by its ID.
        /// </summary>
        public QuestNode GetNode(string questNodeID)
        {
            if (string.IsNullOrEmpty(questNodeID) || nodeList == null) return null;
            return nodeList.Find(x => StringField.Equals(StringField.GetStringValue(x.id), questNodeID));
        }

        /// <summary>
        /// Looks up a node by its ID.
        /// </summary>
        public QuestNode GetNode(StringField questNodeID)
        {
            return GetNode(StringField.GetStringValue(questNodeID));
        }

        #endregion

        #region UI Content

        public bool IsSpeakerQuestGiver(QuestParticipantTextInfo speaker)
        {
            return (speaker == null) || StringField.Equals(speaker.id, questGiverID);
        }

        /// <summary>
        /// Checks if there is any UI content for a specific category.
        /// </summary>
        /// <param name="category">The content category (Dialogue, Journal, etc.).</param>
        /// <param name="speaker">The speaker whose content to check, or blank for the quest giver.</param>
        /// <returns>True if GetContentList would return anything.</returns>
        public bool HasContent(QuestContentCategory category, QuestParticipantTextInfo speaker = null)
        {
            currentSpeaker = IsSpeakerQuestGiver(speaker) ? null : speaker;
            var stateInfo = GetStateInfo(GetState());
            if (stateInfo.HasContent(category)) return true;
            if (nodeList == null) return false;
            for (int i = 0; i < nodeList.Count; i++)
            {
                var node = nodeList[i];
                if (node != null && node.HasContent(category)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the UI content for a specific category.
        /// </summary>
        /// <param name="category">The content category (Dialogue, Journal, etc.).</param>
        /// <param name="speaker">The speaker whose content to get, or blank for the quest giver.</param>
        /// <returns>A list of content items based on the current state of the quest and all of its nodes.</returns>
        public List<QuestContent> GetContentList(QuestContentCategory category, QuestParticipantTextInfo speaker = null)
        {
            var contentList = new List<QuestContent>();
            currentSpeaker = IsSpeakerQuestGiver(speaker) ? null : speaker;
            var stateInfo = GetStateInfo(GetState());
            if (stateInfo != null) AddToContentList(contentList, stateInfo.GetContentList(category));
            if (nodeList != null)
            {
                for (int i = 0; i < nodeList.Count; i++)
                {
                    var node = nodeList[i];
                    var nodeContentList = (node != null) ? node.GetContentList(category) : null;
                    if (nodeContentList != null) AddToContentList(contentList, nodeContentList);
                }
            }
            return contentList;
        }

        // Adds to contentList, replacing links with the linked content.
        private void AddToContentList(List<QuestContent> contentList, List<QuestContent> add)
        {
            if (add == null) return;
            for (int i = 0; i < add.Count; i++)
            {
                var content = add[i];
                if (content == null) continue;
                if (content is LinkQuestContent)
                {
                    var linkedContent = GetContentByID((content as LinkQuestContent).linkedContentID);
                    if (linkedContent != null) contentList.Add(linkedContent);
                }
                else
                {
                    contentList.Add(content);
                }
            }
        }

        public void AssignContentID(QuestContent content)
        {
            if (content == null) return;
            content.contentID = m_nextContentID++;
        }

        public QuestContent GetContentByID(int contentID)
        {
            QuestContent content;
            if (!m_questContentByID.TryGetValue(contentID, out content))
            {
                content = FindContentByID(contentID);
                if (content != null) m_questContentByID[contentID] = content;
            }
            return content;
        }

        protected QuestContent FindContentByID(int contentID)
        {
            return FindContentByID(contentID, offerContentList) ??
                FindContentByID(contentID, offerConditionsUnmetContentList) ??
                FindContentByIDInMainQuest(contentID) ??
                FindContentByIDInNodes(contentID);
        }

        protected QuestContent FindContentByID(int contentID, List<QuestContent> contentList)
        {
            if (contentList == null) return null;
            for (int i = 0; i < contentList.Count; i++)
            {
                var content = contentList[i];
                if (content != null && content.contentID == contentID) return content;
            }
            return null;
        }

        protected QuestContent FindContentByIDInStateInfo(int contentID, QuestStateInfo stateInfo)
        {
            if (stateInfo == null) return null;
            return FindContentByID(contentID, stateInfo.GetContentList(QuestContentCategory.Dialogue)) ??
                FindContentByID(contentID, stateInfo.GetContentList(QuestContentCategory.Journal)) ??
                FindContentByID(contentID, stateInfo.GetContentList(QuestContentCategory.HUD));
        }

        protected QuestContent FindContentByIDInMainQuest(int contentID)
        {
            for (int i = 0; i <= (int)QuestState.Abandoned; i++)
            {
                var content = FindContentByIDInStateInfo(contentID, GetStateInfo((QuestState)i));
                if (content != null) return content;
            }
            return null;
        }

        protected QuestContent FindContentByIDInNodes(int contentID)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                var node = nodeList[i];
                if (node == null) continue;
                var content = FindContentByIDInStateInfo(contentID, node.GetStateInfo(QuestNodeState.Inactive)) ??
                    FindContentByIDInStateInfo(contentID, node.GetStateInfo(QuestNodeState.Active)) ??
                    FindContentByIDInStateInfo(contentID, node.GetStateInfo(QuestNodeState.True));
                if (content != null) return content;
            }
            return null;
        }

        #endregion

        #region Quest Indicator States

        public void SetQuestIndicatorState(string entityID, QuestIndicatorState questIndicatorState)
        {
            if (string.IsNullOrEmpty(entityID)) return;
            if (!indicatorStates.ContainsKey(entityID)) indicatorStates.Add(entityID, QuestIndicatorState.None);
            indicatorStates[entityID] = questIndicatorState;
            MessageSystem.SendMessageWithTarget(this, entityID, QuestMachineMessages.SetIndicatorStateMessage, id, questIndicatorState);
        }

        public void SetQuestIndicatorState(StringField entityID, QuestIndicatorState questIndicatorState)
        {
            SetQuestIndicatorState(StringField.GetStringValue(entityID), questIndicatorState);
        }

        public QuestIndicatorState GetQuestIndicatorState(string entityID)
        {
            return (string.IsNullOrEmpty(entityID) || !indicatorStates.ContainsKey(entityID)) ? QuestIndicatorState.None : indicatorStates[entityID];
        }

        public QuestIndicatorState GetQuestIndicatorState(StringField entityID)
        {
            return GetQuestIndicatorState(StringField.GetStringValue(entityID));
        }

        public void ClearQuestIndicatorStates()
        {
            if (indicatorStates == null) return;
            foreach (var kvp in indicatorStates)
            {
                MessageSystem.SendMessageWithTarget(this, kvp.Key, QuestMachineMessages.SetIndicatorStateMessage, id, QuestIndicatorState.None);
            }
            indicatorStates.Clear();
            QuestMachineMessages.RefreshIndicators(questGiverID);
        }

        #endregion

    }
}
