// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// A quest node is a task or stage in a quest.
    /// </summary>
    [Serializable]
    public class QuestNode
    {

        #region Serialized Fields

        [Tooltip("Internal ID of the quest node, used to reference quest nodes in scripts.")]
        [SerializeField]
        private StringField m_id = new StringField();

        [Tooltip("Internal name of the quest node for the designer's reference. Not seen by the player.")]
        [SerializeField]
        private StringField m_internalName = new StringField();

        [Tooltip("Type of node, which determines some of its behavior.")]
        [SerializeField]
        private QuestNodeType m_nodeType;

        [Tooltip("Completion of this quest node is optional.")]
        [SerializeField]
        private bool m_isOptional;

        [Tooltip("The current state of the quest node.")]
        [SerializeField]
        private QuestNodeState m_state = QuestNodeState.Inactive;

        [Tooltip("Speaker of this node's dialogue content. If unassigned, the quest giver is the speaker.")]
        [SerializeField]
        private StringField m_speaker = new StringField();

        [Tooltip("Info (actions & UI content) for a specific quest state, indexed by the int value of the QuestState enum.")]
        [SerializeField]
        private List<QuestStateInfo> m_stateInfoList = new List<QuestStateInfo>();

        [Tooltip("Conditions required for the node's state to become true.")]
        [SerializeField]
        private QuestConditionSet m_conditionSet = new QuestConditionSet();

        [HideInInspector]
        [SerializeField]
        private List<int> m_childIndexList = new List<int>(); // Indices into Quest.nodes.

        [HideInInspector]
        [SerializeField]
        private Rect m_canvasRect; // Position in editor canvas.

        #endregion

        #region Accessor Properties for Serialized Fields

        /// <summary>
        /// Internal ID of the quest node, used to reference quest nodes in scripts.
        /// </summary>
        public StringField id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        /// <summary>
        /// Name for your internal reference; shown only in the editor.
        /// </summary>
        public StringField internalName
        {
            get { return m_internalName; }
            set { m_internalName = value; }
        }

        /// <summary>
        /// Type of node, which determines some of its behavior.
        /// </summary>
        public QuestNodeType nodeType
        {
            get { return m_nodeType; }
            set { m_nodeType = value; }
        }

        /// <summary>
        /// Specifies that the node is a type that terminates the quest tree.
        /// </summary>
        public bool isEndNodeType
        {
            get { return nodeType == QuestNodeType.Success || nodeType == QuestNodeType.Failure; }
        }

        /// <summary>
        /// Specifies that the node is a type that can connect to other nodes.
        /// </summary>
        public bool isConnectionNodeType
        {
            get { return !isEndNodeType; }
        }

        /// <summary>
        /// Completion of this quest node is optional.
        /// </summary>
        public bool isOptional
        {
            get { return m_isOptional; }
            set { m_isOptional = value; }
        }

        /// <summary>
        /// Speaker of this node's dialogue content. If unassigned, the quest giver is the speaker.
        /// </summary>
        public StringField speaker
        {
            get { return m_speaker; }
            set { m_speaker = value; }
        }

        /// <summary>
        /// Info (actions & UI content) for a specific quest state. This list is indexed by the
        /// int value of the QuestState enum, such as <c>stateInfoList[(int)QuestState.Active]</c>.
        /// </summary>
        public List<QuestStateInfo> stateInfoList
        {
            get { return m_stateInfoList; }
            set { m_stateInfoList = value; }
        }

        /// <summary>
        /// Conditions required for the node's state to become true.
        /// </summary>
        public QuestConditionSet conditionSet
        {
            get { return m_conditionSet; }
            set { m_conditionSet = value; }
        }

        /// <summary>
        /// Indices into the quest's node list. Since Unity can't serialize nested types, such as
        /// a QuestNode reference inside the QuestNode class, we use a list of indices to record
        /// connections between nodes for serialization. At runtime, we use this list of indices
        /// to construct a list of QuestNode references.
        /// </summary>
        public List<int> childIndexList
        {
            get { return m_childIndexList; }
            set { m_childIndexList = value; }
        }

        #endregion

        #region Runtime References

        [NonSerialized]
        private TagDictionary m_tagDictionary = new TagDictionary();

        [NonSerialized]
        private Quest m_quest;

        /// <summary>
        /// The Quest that this node belongs to.
        /// </summary>
        public Quest quest
        {
            get { return m_quest; }
            set { m_quest = value; }
        }

        [NonSerialized]
        public List<QuestNode> m_childList;

        /// <summary>
        /// References to the child nodes linked from this node.
        /// </summary>
        public List<QuestNode> childList
        {
            get { return m_childList; }
            set { m_childList = value; }
        }

        [NonSerialized]
        private List<QuestNode> m_parentList;

        /// <summary>
        /// References to the parent nodes that link to this node.
        /// </summary>
        public List<QuestNode> parentList
        {
            get { return m_parentList; }
            set { m_parentList = value; }
        }

        [NonSerialized]
        private List<QuestNode> m_optionalParentList;

        /// <summary>
        /// The subset of parents that are marked optional.
        /// </summary>
        public List<QuestNode> optionalParentList
        {
            get { return m_optionalParentList; }
            set { m_optionalParentList = value; }
        }

        [NonSerialized]
        private List<QuestNode> m_nonoptionalParentList;

        /// <summary>
        /// The subset of parents that are not marked optional.
        /// </summary>
        public List<QuestNode> nonoptionalParentList
        {
            get { return m_nonoptionalParentList; }
            set { m_nonoptionalParentList = value; }
        }

        /// <summary>
        /// Dictionary of tags defined in this quest node and their values.
        /// </summary>
        public TagDictionary tagDictionary
        {
            get { return m_tagDictionary; }
            set { m_tagDictionary = value; }
        }

        /// <summary>
        /// Invoked when the node changes state.
        /// </summary>
        public event QuestNodeParameterDelegate stateChanged = delegate { };

        private bool m_isCheckingConditions = false;

        #endregion

        #region Editor

        // Node sizes for editor:
        public const float DefaultNodeWidth = 120;
        public const float DefaultNodeHeight = 48;
        public const float ShortNodeHeight = 35;
        public const float DefaultStartNodeX = 200;
        public const float DefaultStartNodeY = 20;

        /// <summary>
        /// Position of the quest node in the Quest Editor window.
        /// </summary>
        public Rect canvasRect
        {
            get { return m_canvasRect; }
            set { m_canvasRect = value; }
        }

        public string GetEditorName()
        {
            if (!StringField.IsNullOrEmpty(internalName)) return internalName.value;
            if (!StringField.IsNullOrEmpty(id)) return id.value;
            return "Node";
        }

        #endregion

        #region Initialization

        public QuestNode() { }

        public QuestNode(StringField id, StringField internalName, QuestNodeType nodeType, bool isOptional = false)
        {
            m_id = id;
            m_internalName = internalName;
            m_nodeType = nodeType;
            m_isOptional = isOptional;
        }

        public void InitializeAsStartNode(string questID)
        {
            id = new StringField(questID + ".start");
            internalName = new StringField("Start");
            nodeType = QuestNodeType.Start;
            m_state = QuestNodeState.Inactive;
            stateInfoList = new List<QuestStateInfo>();
            canvasRect = new Rect(DefaultStartNodeX, DefaultStartNodeY, DefaultNodeWidth, DefaultNodeHeight);
        }

        public void CloneSubassetsInto(QuestNode copy)
        {
            // Assumes lists are identical except subassets haven't been copied.
            if (copy == null) return;
            conditionSet.CloneSubassetsInto(copy.conditionSet);
            QuestStateInfo.CloneSubassets(stateInfoList, copy.stateInfoList);
            tagDictionary.CopyInto(copy.tagDictionary);
        }

        public static void CloneSubassets(List<QuestNode> original, List<QuestNode> copy)
        {
            // Assumes lists are identical except subassets haven't been copied.
            if (original == null || copy == null || copy.Count != original.Count)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: QuestNode.CloneSubassets() failed because copy or original is invalid.");
                return;
            }
            for (int i = 0; i < original.Count; i++)
            {
                if (original[i] != null) original[i].CloneSubassetsInto(copy[i]);
            }
        }

        public void DestroySubassets()
        {
            if (conditionSet != null) conditionSet.DestroySubassets();
            QuestStateInfo.DestroyListSubassets(stateInfoList);
        }

        public static void DestroyListSubassets(List<QuestNode> nodes)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null) nodes[i].DestroySubassets();
            }
        }

        public void InitializeRuntimeReferences(Quest quest)
        {
            this.quest = quest;

            // Set references in condition set:
            if (conditionSet != null) conditionSet.SetRuntimeReferences(quest, this);

            // Build children list:
            if (quest.nodeList != null)
            {
                childList = new List<QuestNode>();
                for (var i = 0; i < childIndexList.Count; i++)
                {
                    var index = childIndexList[i];
                    if (0 <= index && index < quest.nodeList.Count) childList.Add(quest.nodeList[index]);
                }
            }

            parentList = new List<QuestNode>();
            optionalParentList = new List<QuestNode>();
            nonoptionalParentList = new List<QuestNode>();
        }

        public void ConnectRuntimeNodeReferences()
        {
            if (childList == null) return;
            for (int i = 0; i < childList.Count; i++)
            {
                if (childList[i] != null) childList[i].SetParent(this);
            }
        }

        private void SetParent(QuestNode parent)
        {
            if (parent == null) return;
            if (parentList == null) parentList = new List<QuestNode>();
            parentList.Add(parent);
            if (parent.isOptional)
            {
                if (optionalParentList == null) optionalParentList = new List<QuestNode>();
                optionalParentList.Add(parent);
            }
            else
            {
                if (nonoptionalParentList == null) nonoptionalParentList = new List<QuestNode>();
                nonoptionalParentList.Add(parent);
            }
            parent.stateChanged -= OnParentStateChange;
            parent.stateChanged += OnParentStateChange;
        }

        public void SetRuntimeNodeReferences()
        {
            var stateCount = Enum.GetNames(typeof(QuestNodeState)).Length;
            for (int i = 0; i < stateCount; i++)
            {
                var stateInfo = QuestStateInfo.GetStateInfo(stateInfoList, (QuestNodeState)i);
                if (stateInfo != null) stateInfo.SetRuntimeReferences(quest, this);
            }
        }

        #endregion

        #region Quest Node State

        /// <summary>
        /// Returns the current state of the quest node.
        /// </summary>
        public QuestNodeState GetState()
        {
            return m_state;
        }

        /// <summary>
        /// Sets the quest node to a quest state and performs all related activities 
        /// such as enabling connections and executing actions. This may cause other
        /// nodes to advance their states, too.
        /// </summary>
        /// <param name="newState">New state.</param>
        public void SetState(QuestNodeState newState, bool informListeners = true)
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + ((quest != null) ? quest.GetEditorName() : "Quest") + "." + GetEditorName() + ".SetState(" + newState + ")", quest);

            m_state = newState;

            SetConditionChecking(newState == QuestNodeState.Active);

            if (!informListeners) return;

            // Execute state actions:
            var stateInfo = GetStateInfo(m_state);
            if (stateInfo != null && stateInfo.actionList != null)
            {
                for (int i = 0; i < stateInfo.actionList.Count; i++)
                {
                    if (stateInfo.actionList[i] == null) continue;
                    stateInfo.actionList[i].Execute();
                }
            }

            // Notify that state changed:
            QuestMachineMessages.QuestNodeStateChanged(this, quest.id, id, m_state);
            try
            {
                stateChanged(this);
            }
            catch (Exception e) // Don't let exceptions in user-added events break our code.
            {
                if (Debug.isDebugBuild) Debug.LogException(e);
            }

            // Handle special node types:
            switch (m_state)
            {
                case QuestNodeState.Active:
                    if (nodeType != QuestNodeType.Condition)
                    {
                        // Automatically switch non-Condition nodes to True state:
                        SetState(QuestNodeState.True);
                    }
                    break;
                case QuestNodeState.True:
                    // If it's an endpoint, set the overall quest state:
                    switch (nodeType)
                    {
                        case QuestNodeType.Success:
                            if (quest != null) quest.SetState(QuestState.Successful);
                            break;
                        case QuestNodeType.Failure:
                            if (quest != null) quest.SetState(QuestState.Failed);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Sets the internal state value without performing any state change processing.
        /// </summary>
        public void SetStateRaw(QuestNodeState state)
        {
            m_state = state;
        }

        /// <summary>
        /// Returns the state info associated with a quest node state.
        /// </summary>
        public QuestStateInfo GetStateInfo(QuestNodeState state)
        {
            return (stateInfoList != null) ? QuestStateInfo.GetStateInfo(stateInfoList, state) : null;
        }

        /// <summary>
        /// Starts or stops condition checking on condition nodes.
        /// </summary>
        /// <param name="enable">Specifies whether to start (enable) or stop.</param>
        public void SetConditionChecking(bool enable)
        {
            if (!Application.isPlaying) return;
            if ((enable && m_isCheckingConditions) || (!enable && !m_isCheckingConditions)) return;
            if (!isConnectionNodeType || conditionSet == null) return;
            if (enable == true)
            {
                conditionSet.StartChecking(OnConditionsTrue);
            }
            else
            {
                conditionSet.StopChecking();
            }
            m_isCheckingConditions = enable;
        }

        private void OnConditionsTrue()
        {
            SetState(QuestNodeState.True);
        }

        /// <summary>
        /// Invoked by parent when parent's state changes.
        /// </summary>
        /// <param name="parent">Parent node whose state changed.</param>
        private void OnParentStateChange(QuestNode parent)
        {
            if (parent != null && parent.GetState() == QuestNodeState.True && 
                quest != null && quest.GetState() == QuestState.Active && 
                GetState() == QuestNodeState.Inactive)
            {
                SetState(QuestNodeState.Active);
            }
        }

        #endregion

        #region UI Content

        /// <summary>
        /// Checks if there is any UI content for a specific category.
        /// </summary>
        /// <param name="category">The content category (Dialogue, Journal, etc.).</param>
        /// <returns>True if GetContentList would return anything.</returns>
        public bool HasContent(QuestContentCategory category)
        {
            if (!IsContentValidForCurrentSpeaker(category)) return false;
            var stateInfo = QuestStateInfo.GetStateInfo(stateInfoList, m_state);
            if (stateInfo == null) return false;
            var contentList = stateInfo.GetContentList(category);
            return contentList != null && contentList.Count > 0;
        }

        /// <summary>
        /// Gets the UI content for a specific category.
        /// </summary>
        /// <param name="category">The content category (Dialogue, Journal, etc.).</param>
        /// <returns>A list of UI content items based on the current state of the quest and all of its nodes.</returns>
        public List<QuestContent> GetContentList(QuestContentCategory category)
        {
            if (!IsContentValidForCurrentSpeaker(category)) return null;
            var stateInfo = QuestStateInfo.GetStateInfo(stateInfoList, m_state);
            return (stateInfo != null) ? stateInfo.GetContentList(category) : null;
        }

        private bool IsContentValidForCurrentSpeaker(QuestContentCategory category)
        {
            // Non-dialogue content is always valid:
            if (category != QuestContentCategory.Dialogue) return true;
            if (quest == null) return true;

            // Are quest's current speaker and this node's speaker both the quest giver?
            if (quest.currentSpeaker == null)
            {
                return StringField.IsNullOrEmpty(speaker) || StringField.Equals(speaker, quest.questGiverID);
            }

            // Otherwise is quest's current speaker same as this node's speaker?
            return StringField.Equals(speaker, quest.currentSpeaker.id);
        }

        #endregion

    }

}
