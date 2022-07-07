// Copyright © Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Main bridge component between Quest Machine and Dialogue System.
    /// 
    /// - Adds Lua functions that interface with Quest Machine, also replacing the
    /// QuestLog functions so the Lua dropdown wizards can work with Quest Machine
    /// quests.
    /// 
    /// - Processes Quest Machine tags in Dialogue System text.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/Dialogue System/Dialogue System Quest Machine Bridge")]
    [RequireComponent(typeof(CommonLibraryLua))]
    public class DialogueSystemQuestMachineBridge : MonoBehaviour
    {

        [Tooltip("Process Quest Machine tags in Dialogue System content.")]
        [SerializeField]
        private bool m_processQuestMachineTags = true;


        [Tooltip("Redirect the CurrentQuestState & SetQuestState Lua functions to Quest Machine.")]
        [SerializeField]
        private bool m_overrideQuestLog = true;

        public bool processQuestMachineTags
        {
            get { return m_processQuestMachineTags; }
            set { m_processQuestMachineTags = value; }
        }

        public bool overrideQuestLog
        {
            get { return m_overrideQuestLog; }
            set { m_overrideQuestLog = value; }
        }

        #region Initialization

        void Start()
        {
            if (overrideQuestLog)
            {
                var dialogueSystemQuests = DialogueManager.masterDatabase.items;
                var allQuestMachineQuests = QuestMachine.GetAllQuestInstances();
                foreach (var kvp in allQuestMachineQuests)
                {
                    var questID = kvp.Key;
                    var inDialogueSystem = dialogueSystemQuests.Find(x => string.Equals(x.Name, questID)) != null;
                    if (inDialogueSystem)
                    {
                        QuestLog.SetQuestTrackingAvailable(questID, false);
                    }
                    else
                    {
                        QuestLog.AddQuest(questID, string.Empty);
                    }
                }
            }
        }

        protected static bool areFunctionsRegistered = false;
        private bool didIRegisterFunctions = false;

        void Awake()
        {
            if (areFunctionsRegistered)
            {
                didIRegisterFunctions = false;
            }
            else
            {
                // Make the functions available to Lua:
                didIRegisterFunctions = true;
                areFunctionsRegistered = true;
                Lua.RegisterFunction("CanOfferQuest", this, SymbolExtensions.GetMethodInfo(() => CanOfferQuest(string.Empty, string.Empty)));
                Lua.RegisterFunction("CanOfferQuestToQuester", this, SymbolExtensions.GetMethodInfo(() => CanOfferQuestToQuester(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("GiveQuest", this, SymbolExtensions.GetMethodInfo(() => GiveQuestToDefaultPlayer(string.Empty, string.Empty)));
                Lua.RegisterFunction("GiveAllQuests", this, SymbolExtensions.GetMethodInfo(() => GiveAllQuestsToDefaultPlayer(string.Empty)));
                Lua.RegisterFunction("HasQuest", this, SymbolExtensions.GetMethodInfo(() => DefaultPlayerHasQuest(string.Empty)));
                Lua.RegisterFunction("GiveQuestToQuester", this, SymbolExtensions.GetMethodInfo(() => GiveQuestToQuester(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("GiveAllQuestsToQuester", this, SymbolExtensions.GetMethodInfo(() => GiveAllQuestsToQuester(string.Empty, string.Empty)));
                Lua.RegisterFunction("QuesterHasQuest", this, SymbolExtensions.GetMethodInfo(() => QuesterHasQuest(string.Empty, string.Empty)));
                Lua.RegisterFunction("GetQuesterQuestState", this, SymbolExtensions.GetMethodInfo(() => GetQuesterQuestState(string.Empty, string.Empty)));
                Lua.RegisterFunction("SetQuesterQuestState", this, SymbolExtensions.GetMethodInfo(() => SetQuesterQuestState(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("GetQuesterQuestNodeState", this, SymbolExtensions.GetMethodInfo(() => GetQuesterQuestNodeState(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("SetQuesterQuestNodeState", this, SymbolExtensions.GetMethodInfo(() => SetQuesterQuestNodeState(string.Empty, string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("GetQuestNodeState", this, SymbolExtensions.GetMethodInfo(() => GetQuestNodeStateOnDefaultPlayer(string.Empty, string.Empty)));
                Lua.RegisterFunction("SetQuestNodeState", this, SymbolExtensions.GetMethodInfo(() => SetQuestNodeStateOnDefaultPlayer(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("GetQuesterQuestCounterValue", this, SymbolExtensions.GetMethodInfo(() => GetQuesterQuestCounterValue(string.Empty, string.Empty, string.Empty)));
                Lua.RegisterFunction("SetQuesterQuestCounterValue", this, SymbolExtensions.GetMethodInfo(() => SetQuesterQuestCounterValue(string.Empty, string.Empty, (double)0, string.Empty)));
                Lua.RegisterFunction("GetQuestCounterValue", this, SymbolExtensions.GetMethodInfo(() => GetQuestCounterValueOnDefaultPlayer(string.Empty, string.Empty)));
                Lua.RegisterFunction("SetQuestCounterValue", this, SymbolExtensions.GetMethodInfo(() => SetQuestCounterValueOnDefaultPlayer(string.Empty, string.Empty, (double)0)));
                Lua.RegisterFunction("SetQuestIndicator", this, SymbolExtensions.GetMethodInfo(() => SetQuestIndicator(string.Empty, string.Empty, string.Empty)));

                if (overrideQuestLog)
                {
                    // Override Dialogue System's QuestLog Current/Set functions to use Quest Machine quests:
                    QuestLog.CurrentQuestStateOverride = GetQuestStateOnDefaultPlayer;
                    QuestLog.SetQuestStateOverride = SetQuestStateOnDefaultPlayer;
                    QuestLog.CurrentQuestEntryStateOverride = GetQuestEntryStateOnDefaultPlayer;
                    QuestLog.SetQuestEntryStateOverride = SetQuestEntryStateOnDefaultPlayer;
                }
                else
                {
                    Lua.RegisterFunction("GetQuestState", this, SymbolExtensions.GetMethodInfo(() => GetQuestStateOnDefaultPlayer(string.Empty)));
                    Lua.RegisterFunction("SetQMQuestState", this, SymbolExtensions.GetMethodInfo(() => SetQuestStateOnDefaultPlayer(string.Empty, string.Empty)));
                }
            }
        }

        void OnDestroy()
        {
            if (didIRegisterFunctions)
            {
                // Remove the functions from Lua:
                didIRegisterFunctions = false;
                areFunctionsRegistered = false;
                Lua.UnregisterFunction("CanOfferQuest");
                Lua.UnregisterFunction("CanOfferQuestToQuester");
                Lua.UnregisterFunction("GiveQuest");
                Lua.UnregisterFunction("GiveAllQuests");
                Lua.UnregisterFunction("HasQuest");
                Lua.UnregisterFunction("GiveQuestToQuester");
                Lua.UnregisterFunction("GiveAllQuestsToQuester");
                Lua.UnregisterFunction("QuesterHasQuest");
                Lua.UnregisterFunction("GetQuesterQuestState");
                Lua.UnregisterFunction("SetQuesterQuestState");
                Lua.UnregisterFunction("GetQuesterQuestNodeState");
                Lua.UnregisterFunction("SetQuesterQuestNodeState");
                Lua.UnregisterFunction("GetQuestNodeState");
                Lua.UnregisterFunction("SetQuestNodeState");
                Lua.UnregisterFunction("SetQuestIndicator");

                if (overrideQuestLog)
                {
                    // Undo override of Dialogue System's QuestLog Current/Set functions.
                    QuestLog.CurrentQuestStateOverride = null;
                    QuestLog.SetQuestStateOverride = null;
                    QuestLog.CurrentQuestEntryStateOverride = null;
                    QuestLog.SetQuestEntryStateOverride = null;
                }
                else
                {
                    Lua.UnregisterFunction("GetQuestState");
                    Lua.UnregisterFunction("SetQMQuestState");
                }
            }
        }

        void UpdateTracker()
        {
            QuestMachineMessages.RefreshUIs(this);
        }

        #endregion

        #region Lua Functions

        private QuestGiver GetQuestGiver(string questGiverID)
        {
            var qlc = (string.IsNullOrEmpty(questGiverID) && DialogueManager.currentConversant != null) 
                ? DialogueManager.currentConversant.GetComponent<QuestGiver>()
                : QuestMachine.GetQuestListContainer(questGiverID);
            return (qlc != null) ? qlc.GetComponent<QuestGiver>() : null;
        }

        private QuestJournal GetQuester(string questerID)
        {
            return QuestMachine.GetQuestJournal(questerID);
        }

        private void SetConversationQuesterID(QuestJournal quester)
        {
            if (quester == null) return;
            if (DialogueManager.IsConversationActive)
            {
                Lua.Run("Conversation[" + DialogueManager.CurrentConversationState.subtitle.dialogueEntry.conversationID + "].QuesterID = \"" + quester.id + "\"");
            }
        }

        private bool CanOfferQuest(string questGiverID, string questID)
        {
            return CanOfferQuestToQuester(questGiverID, questID, string.Empty);
        }

        private bool CanOfferQuestToQuester(string questGiverID, string questID, string questerID)
        {
            var questGiver = GetQuestGiver(questGiverID);
            var quester = GetQuester(questerID);
            if (questGiver == null || quester == null) return false;
            var quest = questGiver.FindQuest(questID);
            if (quest == null) return false;

            // Check if the player is already doing a copy of this quest:
            var playerCopy = quester.FindQuest(questID);
            var isPlayerCopyActive = (playerCopy != null && playerCopy.GetState() == QuestState.Active);
            var playerHasSuccessfullyCompleted = playerCopy != null && playerCopy.GetState() == QuestState.Successful;
            quest.UpdateCooldown();
            return quest.canOffer && !isPlayerCopyActive && (playerCopy == null || playerCopy.timesAccepted < quest.maxTimes) &&
                (!(quest.noRepeatIfSuccessful && playerHasSuccessfullyCompleted));
        }

        private void GiveQuestToDefaultPlayer(string questGiverID, string questID)
        {
            GiveQuestToQuester(questGiverID, questID, string.Empty);
        }

        private void GiveAllQuestsToDefaultPlayer(string questGiverID)
        {
            GiveAllQuestsToQuester(questGiverID, string.Empty);
        }

        private bool DefaultPlayerHasQuest(string questID)
        {
            return QuesterHasQuest(questID, string.Empty);
        }

        private void GiveQuestToQuester(string questGiverID, string questID, string questerID)
        {
            var giver = GetQuestGiver(questGiverID);
            if (giver == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find quest giver '" + questGiverID + "' to give quest '" + questID + "'.", this);
                return;
            }
            var quest = giver.FindQuest(questID);
            if (quest == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest giver '" + questGiverID + "' doesn't have quest '" + questID + "' to give.", this);
                return;
            }
            var quester = GetQuester(questerID);
            giver.GiveQuestToQuester(quest, quester);
            SetConversationQuesterID(quester);
        }

        private void GiveAllQuestsToQuester(string questGiverID, string questerID)
        {
            var giver = GetQuestGiver(questGiverID);
            if (giver == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find quest giver '" + questGiverID + "' to give all quests.", this);
            }
            else
            {
                var quester = GetQuester(questerID);
                giver.GiveAllQuestsToQuester(quester);
                SetConversationQuesterID(quester);
            }
        }

        private bool QuesterHasQuest(string questID, string questerID)
        {
            var qlc = QuestMachine.GetQuestJournal(questerID);
            if (qlc == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find quester '" + questerID + "' to check if it has quest '" + questID + "'.", this);
                return false;
            }
            return qlc.ContainsQuest(questID);
        }

        private Quest GetQuestInstance(string questID, string questerID)
        {
            var originalDebug = QuestMachine.debug;
            QuestMachine.debug = false;
            var quester = GetQuester(questerID);
            var result = (quester != null)
                ? (quester.FindQuest(questID) ?? QuestMachine.GetQuestInstance(questID, StringField.GetStringValue(quester.id)) ?? QuestMachine.GetQuestInstance(questID, questerID))
                : QuestMachine.GetQuestInstance(questID, questerID);
            QuestMachine.debug = originalDebug;
            return result;
        }

        private string GetQuesterQuestState(string questID, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
            }
            switch (questInstance.GetState())
            {
                default:
                case QuestState.Disabled:
                case QuestState.WaitingToStart:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
                case QuestState.Abandoned:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Abandoned);
                case QuestState.Active:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Active);
                case QuestState.Successful:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Success);
                case QuestState.Failed:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Failure);
            }
        }

        private void SetQuesterQuestState(string questID, string state, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find Quest Machine quest with ID '" + questID + "' to set its state.", this);
                return;
            }
            var newState = string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Active)) ? QuestState.Active
                : string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Success)) ? QuestState.Successful
                : string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Failure)) ? QuestState.Failed
                : string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Abandoned)) ? QuestState.Abandoned
                : QuestState.WaitingToStart;
            questInstance.SetState(newState);
        }

        private string GetQuesterQuestNodeNumberState(string questID, double nodeNumber, string questerID)
        {
            var entryNumber = (int)nodeNumber;
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
            }
            else if (!(0 <= entryNumber && entryNumber < questInstance.nodeList.Count) || questInstance.nodeList[entryNumber] == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest Machine quest with ID '" + questID + "' doesn't have a node " + entryNumber + ". Can't get its state.", this);
                return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
            }
            switch (questInstance.nodeList[entryNumber].GetState())
            {
                default:
                case QuestNodeState.Inactive:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
                case QuestNodeState.Active:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Active);
                case QuestNodeState.True:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Success);
            }
        }

        private string GetQuesterQuestNodeState(string questID, string questNodeID, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
            }
            var node = questInstance.GetNode(questNodeID);
            if (node == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest Machine quest with ID '" + questID + "' doesn't have a node '" + questNodeID + "'. Can't get its state.", this);
                return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
            }
            switch (node.GetState())
            {
                default:
                case QuestNodeState.Inactive:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Unassigned);
                case QuestNodeState.Active:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Active);
                case QuestNodeState.True:
                    return QuestLog.StateToString(DialogueSystem.QuestState.Success);
            }
        }

        private void SetQuesterQuestNodeNumberState(string questID, double nodeNumber, string state, string questerID)
        {
            var entryNumber = (int)nodeNumber;
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find Quest Machine quest with ID '" + questID + "' to set its node " + entryNumber + " state.", this);
                return;
            }
            else if (!(0 <= entryNumber && entryNumber < questInstance.nodeList.Count) || questInstance.nodeList[entryNumber] == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest Machine quest with ID '" + questID + "' doesn't have a node " + entryNumber + ". Can't set its state.", this);
                return;
            }
            var newState = string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Active)) ? QuestNodeState.Active
                : string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Success)) ? QuestNodeState.True
                : QuestNodeState.Inactive;
            questInstance.nodeList[entryNumber].SetState(newState);
        }

        private void SetQuesterQuestNodeState(string questID, string questNodeID, string state, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find Quest Machine quest with ID '" + questID + "' to set its node '" + questNodeID + "' state.", this);
                return;
            }
            var node = questInstance.GetNode(questNodeID);
            if (node == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest Machine quest with ID '" + questID + "' doesn't have a node '" + questNodeID + "'. Can't set its state.", this);
                return;
            }
            var newState = string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Active)) ? QuestNodeState.Active
                : string.Equals(state, QuestLog.StateToString(DialogueSystem.QuestState.Success)) ? QuestNodeState.True
                : QuestNodeState.Inactive;
            node.SetState(newState);
        }

        private string GetQuestStateOnDefaultPlayer(string questID)
        {
            return GetQuesterQuestState(questID, string.Empty);
        }

        private void SetQuestStateOnDefaultPlayer(string questID, string state)
        {
            SetQuesterQuestState(questID, state, string.Empty);
        }

        private string GetQuestNodeStateOnDefaultPlayer(string questID, string questNodeID)
        {
            return GetQuesterQuestNodeState(questID, questNodeID, string.Empty);
        }

        private void SetQuestNodeStateOnDefaultPlayer(string questID, string questNodeID, string state)
        {
            SetQuesterQuestNodeState(questID, questNodeID, state, string.Empty);
        }

        private string GetQuestEntryStateOnDefaultPlayer(string questID, int entryNumber)
        {
            return GetQuesterQuestNodeNumberState(questID, entryNumber, string.Empty);
        }

        private void SetQuestEntryStateOnDefaultPlayer(string questID, int entryNumber, string state)
        {
            SetQuesterQuestNodeNumberState(questID, entryNumber, state, string.Empty);
        }

        private double GetQuesterQuestCounterValue(string questID, string counterName, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find Quest Machine quest with ID '" + questID + "' to get the value of counter '" + counterName + "'.", this);
                return 0;
            }
            var counter = questInstance.GetCounter(counterName);
            if (counter == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest '" + questID + "' doesn't have a counter '" + counterName + "'. Can't get its value.", this);
                return 0;
            }
            return counter.currentValue;
        }

        private void SetQuesterQuestCounterValue(string questID, string counterName, double value, string questerID)
        {
            var questInstance = GetQuestInstance(questID, questerID);
            if (questInstance == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Can't find Quest Machine quest with ID '" + questID + "' to set the value of counter '" + counterName + "'.", this);
                return;
            }
            var counter = questInstance.GetCounter(counterName);
            if (counter == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Quest '" + questID + "' doesn't have a counter '" + counterName + "'. Can't set its value.", this);
                return;
            }
            counter.currentValue = (int)value;
        }

        private double GetQuestCounterValueOnDefaultPlayer(string questID, string counterName)
        {
            return GetQuesterQuestCounterValue(questID, counterName, string.Empty);
        }

        private void SetQuestCounterValueOnDefaultPlayer(string questID, string counterName, double value)
        {
            SetQuesterQuestCounterValue(questID, counterName, value, string.Empty);
        }

        private void SetQuestIndicator(string questID, string entityID, string state)
        {
            if (string.IsNullOrEmpty(questID)) return;
            var affectedQuest = QuestMachine.GetQuestInstance(questID);
            if (affectedQuest == null) return;
            affectedQuest.SetQuestIndicatorState(entityID, StringToQuestIndicatorState(state));
        }

        private QuestIndicatorState StringToQuestIndicatorState(string state)
        {
            switch (state.ToLower())
            {
                default:
                case "none": return QuestIndicatorState.None;
                case "offerdisabled": return QuestIndicatorState.OfferDisabled;
                case "talkdisabled": return QuestIndicatorState.TalkDisabled;
                case "interactdisabled": return QuestIndicatorState.InteractDisabled;
                case "offer": return QuestIndicatorState.Offer;
                case "talk": return QuestIndicatorState.Talk;
                case "interact": return QuestIndicatorState.Interact;
                case "custom0": return QuestIndicatorState.Custom0;
                case "custom1": return QuestIndicatorState.Custom1;
                case "custom2": return QuestIndicatorState.Custom2;
                case "custom3": return QuestIndicatorState.Custom3;
                case "custom4": return QuestIndicatorState.Custom4;
                case "custom5": return QuestIndicatorState.Custom5;
                case "custom6": return QuestIndicatorState.Custom6;
                case "custom7": return QuestIndicatorState.Custom7;
                case "custom8": return QuestIndicatorState.Custom8;
                case "custom9": return QuestIndicatorState.Custom9;
            }
        }

        #endregion

        #region Quest Machine Tags

        private Quest m_currentQuest = null;
        private int m_currentQuestConversationID = -1;

        private void OnConversationStart(Transform actor)
        {
            m_currentQuest = null;
        }

        private void OnConversationEnd(Transform actor)
        {
            m_currentQuest = null;
        }

        private void OnConversationLine(Subtitle subtitle)
        {
            if (subtitle == null) return;
            SetCurrentQuestID(subtitle.dialogueEntry);
            if (processQuestMachineTags)
            {
                subtitle.formattedText.text = ReplaceQuestMachineTags(subtitle.formattedText.text);
            }
        }

        private void OnConversationResponseMenu(Response[] responses)
        {
            if (responses == null) return;
            for (int i = 0; i < responses.Length; i++)
            {
                SetCurrentQuestID(responses[i].destinationEntry);
                responses[i].formattedText.text = ReplaceQuestMachineTags(responses[i].formattedText.text);
            }
        }

        private void SetCurrentQuestID(DialogueEntry entry)
        {
            if (entry == null) return;
            if (entry.conversationID == m_currentQuestConversationID && m_currentQuest != null) return;
            m_currentQuestConversationID = entry.conversationID;
            var conversation = DialogueManager.MasterDatabase.GetConversation(entry.conversationID);
            if (conversation != null)
            {
                var questID = GetConversationField(conversation, "QuestID");
                var questerID = GetConversationField(conversation, "QuesterID");
                if (!string.IsNullOrEmpty(questID))
                {
                    var originalDebugValue = QuestMachine.debug;
                    QuestMachine.debug = false;
                    try
                    {
                        m_currentQuest = GetQuestInstance(questID, questerID) ?? QuestMachine.GetQuestInstance(questID);
                    }
                    finally
                    {
                        QuestMachine.debug = originalDebugValue;
                    }

                }
            }
        }

        public static string GetConversationField(Conversation conversation, string fieldName)
        {
            if (conversation == null || string.IsNullOrEmpty(fieldName)) return string.Empty;
            var value = DialogueLua.GetConversationField(conversation.id, fieldName).AsString;
            if (string.IsNullOrEmpty(value) || string.Equals(value, "nil")) value = conversation.LookupValue(fieldName);
            return value;
        }

        private string ReplaceQuestMachineTags(string text)
        {
            return QuestMachineTags.ReplaceTags(text, m_currentQuest);
        }

        #endregion

    }
}