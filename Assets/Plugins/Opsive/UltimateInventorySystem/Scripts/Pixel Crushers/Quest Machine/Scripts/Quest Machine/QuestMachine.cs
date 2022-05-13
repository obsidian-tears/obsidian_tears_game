// Copyright (c) Pixel Crushers. All rights reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest Machine static manager class. This is not a MonoBehaviour. You can add
    /// a QuestMachineConfiguration component to a GameObject to configure this class
    /// through the inspector.
    /// </summary>
    public static class QuestMachine
    {

        #region Properties & Variables

        /// <summary>
        /// Default text table to use for text lookup and localization.
        /// </summary>
        public static TextTable textTable { get { return GlobalTextTable.textTable; } }

        /// <summary>
        /// Default dialogue UI to use if NPC doesn't specify one.
        /// </summary>
        public static IQuestDialogueUI defaultQuestDialogueUI { get; set; }

        /// <summary>
        /// Default journal UI to use if NPC doesn't specify one.
        /// </summary>
        public static IQuestJournalUI defaultQuestJournalUI { get; set; }

        /// <summary>
        /// Default alert UI to use if NPC doesn't specify one.
        /// </summary>
        public static IQuestAlertUI defaultQuestAlertUI { get; set; }

        /// <summary>
        /// Default quest HUD to use if player doesn't specify one.
        /// </summary>
        public static IQuestHUD defaultQuestHUD { get; set; }

        /// <summary>
        /// Specifies what to show in dialogue when quest givers only have completed quests.
        /// </summary>
        public static CompletedQuestGlobalDialogueMode completedQuestDialogueMode { get; set; }

        /// <summary>
        /// When serializing to JSON, format for readability (uses more whitespace).
        /// </summary>
        public static bool prettyPrintJson { get; set; }

        private static bool m_debug;

        /// <summary>
        /// Log verbose debugging info to the Console.
        /// </summary>
        public static bool debug
        {
            get { return m_debug && Debug.isDebugBuild; }
            set { m_debug = value; }
        }

        private static bool m_isLoadingGame = false;

        /// <summary>
        /// True when loading a game, in which case quests shouldn't run their state actions again.
        /// </summary>
        public static bool isLoadingGame
        {
            get { return m_isLoadingGame; }
            set { m_isLoadingGame = value; }
        }

        private static Dictionary<string, IdentifiableQuestListContainer> m_questListContainers = new Dictionary<string, IdentifiableQuestListContainer>(); // key=ID.

        private static Dictionary<string, Quest> m_questAssets = new Dictionary<string, Quest>(); //key=questID

        private static Dictionary<string, List<Quest>> m_questInstances = new Dictionary<string, List<Quest>>(); // key=questID

        private static Dictionary<string, Sprite> m_images = new Dictionary<string, Sprite>(); // key=imagePath

        private static Dictionary<string, AudioClip> m_audioClips = new Dictionary<string, AudioClip>(); // key=audioClipPath

        private static Dictionary<string, Quest> questAssets { get { return m_questAssets; } }

        private static Dictionary<string, List<Quest>> questInstances { get { return m_questInstances; } }

        private static Dictionary<string, Sprite> images { get { return m_images; } }

        private static Dictionary<string, AudioClip> audioClips { get { return m_audioClips; } }

#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            m_questListContainers = new Dictionary<string, IdentifiableQuestListContainer>(); // key=ID.
            m_questAssets = new Dictionary<string, Quest>(); //key=questID
            m_questInstances = new Dictionary<string, List<Quest>>(); // key=questID
            m_images = new Dictionary<string, Sprite>(); // key=imagePath
            m_audioClips = new Dictionary<string, AudioClip>(); // key=audioClipPath
        }
#endif

        #endregion

        #region Quest List Container Registry
        // Quest List Containers include questers (QuestJournal) and quest givers (QuestGiver).

        /// <summary>
        /// Registers a QuestListContainer for easy lookup.
        /// </summary>
        /// <param name="qlc">QuestListContainer to register.</param>
        public static void RegisterQuestListContainer(IdentifiableQuestListContainer qlc)
        {
            if (qlc == null) return;
            var id = StringField.GetStringValue(qlc.id);
            if (m_questListContainers.ContainsKey(id))
            {
                Debug.LogWarning("Quest Machine: A QuestListContainer with id '" + id + "' is already registered. Can't register " + qlc, qlc);
            }
            else
            {
                m_questListContainers.Add(id, qlc);
            }
        }

        /// <summary>
        /// Unregisters a QuestListContainer.
        /// </summary>
        /// <param name="qlc">QuestListContainer to unregister.</param>
        public static void UnregisterQuestListContainer(IdentifiableQuestListContainer qlc)
        {
            if (qlc == null) return;
            var id = StringField.GetStringValue(qlc.id);
            if (m_questListContainers.ContainsKey(id))
            {
                m_questListContainers.Remove(id);
            }
        }

        /// <summary>
        /// Looks up a registered QuestListContainer.
        /// </summary>
        /// <param name="id">ID of the QuestListContainer.</param>
        public static IdentifiableQuestListContainer GetQuestListContainer(string id)
        {
            return (!string.IsNullOrEmpty(id) && m_questListContainers.ContainsKey(id)) ? m_questListContainers[id] : null;
        }

        /// <summary>
        /// Looks up a registered QuestListContainer.
        /// </summary>
        /// <param name="id">ID of the QuestListContainer.</param>
        public static IdentifiableQuestListContainer GetQuestListContainer(StringField id)
        {
            return GetQuestListContainer(StringField.GetStringValue(id));
        }

        /// <summary>
        /// Looks up a QuestJournal.
        /// </summary>
        /// <param name="id">ID of the QuestJournal's owner, or empty for any QuestJournal.</param>
        public static QuestJournal GetQuestJournal(string id)
        {
            foreach (var kvp in m_questListContainers)
            {
                var journal = kvp.Value as QuestJournal;
                if (journal != null && (string.IsNullOrEmpty(id) || string.Equals(id, kvp.Key))) return journal;
            }
            return UnityEngine.Object.FindObjectOfType<QuestJournal>();
        }

        /// <summary>
        /// Looks up a QuestJournal.
        /// </summary>
        /// <param name="id">ID of the QuestJournal's owner, or empty for any QuestJournal.</param>
        public static QuestJournal GetQuestJournal(StringField id)
        {
            return GetQuestJournal(StringField.GetStringValue(id));
        }

        /// <summary>
        /// Looks up the first registered QuestJournal.
        /// </summary>
        public static QuestJournal GetQuestJournal()
        {
            return GetQuestJournal(string.Empty);
        }

        #endregion

        #region Give Quest

        public static Quest GiveQuest(StringField questID)
        {
            return GiveQuestToQuester(questID, StringField.empty);
        }

        public static Quest GiveQuest(string questID)
        {
            return GiveQuestToQuester(questID, string.Empty);
        }

        public static Quest GiveQuest(Quest quest)
        {
            return GiveQuestToQuester(quest, string.Empty);
        }

        public static Quest GiveQuestToQuester(StringField questID, StringField questerID)
        {
            return GiveQuestToQuester(StringField.GetStringValue(questID), StringField.GetStringValue(questerID));
        }

        public static Quest GiveQuestToQuester(Quest quest, StringField questerID)
        {
            return GiveQuestToQuester(quest, GetQuestJournal(questerID));
        }

        public static Quest GiveQuestToQuester(Quest quest, string questerID)
        {
            return GiveQuestToQuester(quest, GetQuestJournal(questerID));
        }

        public static Quest GiveQuestToQuester(string questID, string questerID)
        {
            return GiveQuestToQuester(GetQuestAsset(questID), GetQuestJournal(questerID));
        }

        public static Quest GiveQuestToQuester(Quest quest, QuestJournal questJournal)
        {
            if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GiveQuestToQuester() quest is null.");
                return null;
            }
            else if (questJournal == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GiveQuestToQuester() quest journal is null.");
                return null;
            }
            else
            {
                if (quest.isAsset) quest = quest.Clone();
                // Add the copy to the quester and activate it:
                var questerTextInfo = new QuestParticipantTextInfo(questJournal.id, questJournal.displayName, questJournal.image, null);
                quest.AssignQuester(questerTextInfo);
                quest.timesAccepted = 1;
                questJournal.deletedStaticQuests.Remove(StringField.GetStringValue(quest.id));
                questJournal.AddQuest(quest);
                quest.SetState(QuestState.Active);
                QuestMachineMessages.RefreshIndicators(quest);
                return quest;
            }
        }

        #endregion

        #region Quest Asset Registry

        private static string GetQuestKey(Quest quest)
        {
            if (quest == null) return null;
            return StringField.IsNullOrEmpty(quest.id) ? quest.GetInstanceID().ToString() : StringField.GetStringValue(quest.id);
        }

        /// <summary>
        /// Registers a quest asset so it can be looked up when a quest list needs to instantiate a copy.
        /// </summary>
        /// <param name="quest">A quest asset in the project.</param>
        public static void RegisterQuestAsset(Quest quest)
        {
            if (quest == null) return;
            if (!quest.isAsset)
            {
                Debug.LogWarning("Quest Machine: " + quest.name + " ('" + quest.id + "') is not an asset. Not registering it in Quest Machine's asset list.");
                return;
            }
            var key = GetQuestKey(quest);
            if (debug) Debug.Log("Quest Machine: Registering quest asset '" + quest.id + "'.", quest);
            if (string.IsNullOrEmpty(key) && debug) Debug.LogWarning("Quest Machine: " + quest.name + " ID is blank. This may affect registration with Quest Machine.", quest);
            questAssets[key] = quest;
            RegisterQuestMedia(quest);
        }

        /// <summary>
        /// Unregisters a quest asset.
        /// </summary>
        /// <param name="quest">A quest asset in the project.</param>
        public static void UnregisterQuestAsset(Quest quest)
        {
            if (quest == null) return;
            var key = GetQuestKey(quest);
            if (questAssets.ContainsKey(key))
            {
                if (debug) Debug.Log("Quest Machine: Unregistering quest asset '" + quest.id + "'.", quest);
                questAssets.Remove(key);
            }
        }

        /// <summary>
        /// Unregisters all quest assets from Quest Machine.
        /// </summary>
        public static void UnregisterAllQuestAssets()
        {
            if (debug) Debug.Log("Quest Machine: Unregistering all quests.");
            questAssets.Clear();
        }

        /// <summary>
        /// Looks up a quest asset by ID.
        /// </summary>
        /// <param name="id">Quest ID.</param>
        /// <returns>Quest with the matching ID, or null if none is found.</returns>
        public static Quest GetQuestAsset(string id)
        {
            if (questAssets.ContainsKey(id))
            {
                return questAssets[id];
            }
            else
            {
                if (debug) Debug.LogWarning("Quest Machine: A quest asset with ID '" + id + "' is not registered with Quest Machine.");
                return null;
            }
        }

        /// <summary>
        /// Looks up a quest asset by ID.
        /// </summary>
        /// <param name="id">Quest ID.</param>
        /// <returns>Quest with the matching ID, or null if none is found.</returns>
        public static Quest GetQuestAsset(StringField id)
        {
            return GetQuestAsset(StringField.GetStringValue(id));
        }

        #endregion

        #region Quest Instance Registry

        /// <summary>
        /// Makes Quest Machine aware of a quest instance. Each quest ID may be associated with
        /// multiple instances of a quest if there are multiple questers. This allows Quest Machine
        /// to look it up by ID.
        /// </summary>
        /// <param name="quest">Quest instance to register.</param>
        public static void RegisterQuestInstance(Quest quest)
        {
            if (quest == null) return;
            if (quest.isAsset)
            {
                Debug.LogWarning("Quest Machine: " + quest.name + " ('" + quest.id + "') is an asset. Not registering it in Quest Machine's instance list.");
                return;
            }
            var key = GetQuestKey(quest);
            if (debug) Debug.Log("Quest Machine: Registering quest instance '" + quest.id + "'.", quest);
            if (string.IsNullOrEmpty(key) && debug) Debug.LogWarning("Quest Machine: " + quest.name + " ID is blank. This may affect registration with Quest Machine.", quest);
            if (!questInstances.ContainsKey(key))
            {
                questInstances.Add(key, new List<Quest>());
            }
            questInstances[key].Insert(0, quest);
            RegisterQuestMedia(quest);
        }

        /// <summary>
        /// Unregisters a quest instance.
        /// </summary>
        /// <param name="quest">Quest instance to unregister.</param>
        public static void UnregisterQuestInstance(Quest quest)
        {
            if (quest == null) return;
            var key = GetQuestKey(quest);
            if (questInstances.ContainsKey(key))
            {
                questInstances[key].Remove(quest);
                if (questInstances[key].Count == 0)
                {
                    questInstances.Remove(key);
                }
            }
        }

        /// <summary>
        /// Unregisters all quest IDs.
        /// </summary>
        public static void UnregisterAllQuestInstances()
        {
            questInstances.Clear();
        }

        /// <summary>
        /// Looks up a quest instance by ID.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <param name="questerID">ID of quester assigned to quest, or blank or null for any quester.</param>
        /// <returns>Quest instance with the specified quest and quester ID, or null if none is found.</returns>
        public static Quest GetQuestInstance(string questID, string questerID)
        {
            // Give preference to instance in a quest journal:
            var quester = GetQuestJournal(questerID);
            if (quester != null)
            {
                var quest = quester.FindQuest(questID);
                if (quest != null) return quest;
            }

            // Otherwise search all quest instances that have questID:
            var anyQuester = string.IsNullOrEmpty(questerID);
            if (questInstances.ContainsKey(questID) && questInstances[questID].Count > 0)
            {
                var list = questInstances[questID];
                for (int i = 0; i < list.Count; i++)
                {
                    var questInstance = list[i];
                    if (questInstance == null) continue;
                    if (anyQuester || string.Equals(questerID, questInstance.questerID))
                    {
                        return questInstance;
                    }
                }
            }
            if (debug) Debug.LogWarning("Quest Machine: A quest instance with ID '" + questID + "' is not registered with Quest Machine.");
            return null;
        }

        /// <summary>
        /// Looks up a quest instance by ID.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <returns>A quest instance matching the specified ID, or null if none is found.</returns>
        public static Quest GetQuestInstance(string id)
        {
            return GetQuestInstance(id, string.Empty);
        }

        /// <summary>
        /// Looks up a quest instance by ID.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <param name="questerID">ID of quester assigned to quest, or blank or null for any quester.</param>
        /// <returns>Quest instance with the specified quest and quester ID, or null if none is found.</returns>
        public static Quest GetQuestInstance(StringField id, StringField questerID)
        {
            return GetQuestInstance(StringField.GetStringValue(id), StringField.GetStringValue(questerID));
        }

        /// <summary>
        /// Looks up a quest instance by ID.
        /// </summary>
        /// <param name="questID">Quest ID.</param>
        /// <returns>A quest instance matching the specified ID, or null if none is found.</returns>
        public static Quest GetQuestInstance(StringField id)
        {
            return GetQuestInstance(id, StringField.empty);
        }

        /// <summary>
        /// Returns a dictionary of all registered quest instances, indexed by quest ID.
        /// </summary>
        public static Dictionary<string, List<Quest>> GetAllQuestInstances()
        {
            return questInstances;
        }

        #endregion

        #region Quest Media

        private static void RegisterQuestMedia(Quest quest)
        {
            if (quest == null || quest.nodeList == null) return;
            RegisterImage(quest.icon);
            RegisterMediaInContentList(quest.offerContentList);
            RegisterMediaInContentList(quest.offerConditionsUnmetContentList);
            RegisterMediaInStateInfoList(quest.stateInfoList);
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                if (quest.nodeList[i] == null) continue;
                RegisterMediaInStateInfoList(quest.nodeList[i].stateInfoList);
            }
        }

        private static void RegisterMediaInStateInfoList(List<QuestStateInfo> stateInfoList)
        {
            if (stateInfoList == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                RegisterMediaInStateInfo(stateInfoList[i]);
            }
        }

        private static void RegisterMediaInStateInfo(QuestStateInfo stateInfo)
        {
            if (stateInfo == null || stateInfo.categorizedContentList == null) return;
            for (int i = 0; i < stateInfo.categorizedContentList.Count; i++)
            {
                if (stateInfo.categorizedContentList[i] != null)
                {
                    RegisterMediaInContentList(stateInfo.categorizedContentList[i].contentList);
                }
                if (stateInfo.actionList != null)
                {
                    RegisterMediaInActionList(stateInfo.actionList);
                }
            }
        }

        private static void RegisterMediaInContentList(List<QuestContent> contentList)
        {
            if (contentList == null) return;
            for (int i = 0; i < contentList.Count; i++)
            {
                var content = contentList[i];
                if (content == null) continue;
                RegisterImages(content.GetImages());
                RegisterAudioClips(content.GetAudioClips());
            }
        }

        private static void RegisterMediaInActionList(List<QuestAction> actionList)
        {
            if (actionList == null) return;
            for (int i = 0; i < actionList.Count; i++)
            {
                var action = actionList[i];
                if (action == null) continue;
                RegisterImages(action.GetImages());
                RegisterAudioClips(action.GetAudioClips());
            }
        }

        private static void RegisterImages(Sprite[] images)
        {
            if (images == null) return;
            for (int i = 0; i < images.Length; i++)
            {
                RegisterImage(images[i]);
            }
        }

        private static void RegisterAudioClips(AudioClip[] audioClips)
        {
            if (audioClips == null) return;
            for (int i = 0; i < audioClips.Length; i++)
            {
                RegisterAudioClip(audioClips[i]);
            }
        }

        public static void RegisterImage(Sprite image)
        {
            if (image == null) return;
            var imageName = image.name;
            if (images.ContainsKey(imageName))
            {
                images[imageName] = image;
            }
            else
            {
                images.Add(imageName, image);
            }
        }

        public static void RegisterAudioClip(AudioClip audioClip)
        {
            if (audioClip == null) return;
            var audioClipName = audioClip.name;
            if (audioClips.ContainsKey(audioClipName))
            {
                audioClips[audioClipName] = audioClip;
            }
            else
            {
                audioClips.Add(audioClipName, audioClip);
            }
        }

        public static Sprite GetImage(string imageName)
        {
            var result = (!string.IsNullOrEmpty(imageName) && images.ContainsKey(imageName)) ? images[imageName] : null;
            if (result == null && !Application.isPlaying)
            {
#if EVALUATION
                Debug.LogWarning("Quest Machine: The evaluation version cannot copy image references in the editor, but the full version can.");
#elif UNITY_EDITOR
                var guids = AssetDatabase.FindAssets("t:Sprite " + imageName);
                if (guids.Length > 0)
                {
                    try
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        result = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    }
                    catch (System.Exception) { }
                }
#endif
            }
            return result;
        }

        public static string GetImagePath(Sprite image)
        {
            return (image != null) ? image.name : string.Empty;
        }

        public static AudioClip GetAudioClip(string audioClipName)
        {
            var result = (!string.IsNullOrEmpty(audioClipName) && audioClips.ContainsKey(audioClipName)) ? audioClips[audioClipName] : null;
            if (result == null && !Application.isPlaying)
            {
#if EVALUATION
                Debug.LogWarning("Quest Machine: The evaluation version cannot copy audio clip references in the editor, but the full version can.");
#elif UNITY_EDITOR
                var guids = AssetDatabase.FindAssets("t:AudioClip " + audioClipName);
                if (guids.Length > 0)
                {
                    try
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        result = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    }
                    catch (System.Exception) { }
                }
#endif
            }
            return result;
        }

        public static string GetAudioClipPath(AudioClip audioClip)
        {
            return (audioClip != null) ? audioClip.name : string.Empty;
        }

        #endregion

        #region Quest Counters

        /// <summary>
        /// Looks up a quest's counter.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="counterName">The counter name.</param>
        /// <returns>A quest counter, or null if none matches the questID and counterName.</returns>
        public static QuestCounter GetQuestCounter(string questID, string counterName, string questerID = null)
        {
            var quest = GetQuestInstance(questID, questerID);
            if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GetQuestCounter(" + questID + ", " + counterName + "): Couldn't find a quest with ID '" + questID + "'.");
                return null;
            }
            var counter = quest.GetCounter(counterName);
            if (counter == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GetQuestCounter(" + questID + ", " + counterName + "): Couldn't find a counter named '" + counterName + "'.");
                return null;
            }
            return counter;
        }

        /// <summary>
        /// Looks up a quest's counter.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="counterName">The counter name.</param>
        /// <returns>A quest counter, or null if none matches the questID and counterName.</returns>
        public static QuestCounter GetQuestCounter(StringField questID, StringField counterName, StringField questerID = null)
        {
            return GetQuestCounter(StringField.GetStringValue(questID), StringField.GetStringValue(counterName), (questerID != null) ? StringField.GetStringValue(questerID) : null);
        }

        #endregion

        #region Quest States

        /// <summary>
        /// Looks up a quest's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <returns>The quest's state, or QuestState.Inactive if no quest with the specified ID has been registered.</returns>
        public static QuestState GetQuestState(string questID, string questerID = null)
        {
            var quest = GetQuestInstance(questID, questerID);
            if (quest == null)
            {
                //--- Unnecessary warning. In this case, user probably just wants WaitingToStart.
                //if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GetQuestState(" + questID + "): Couldn't find a quest with ID '" + questID + "'.");
                return QuestState.WaitingToStart;
            }
            return quest.GetState();
        }

        /// <summary>
        /// Looks up a quest's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <returns>The quest's state, or QuestState.Inactive if no quest with the specified ID has been registered.</returns>
        public static QuestState GetQuestState(StringField questID)
        {
            return GetQuestState(StringField.GetStringValue(questID));
        }

        /// <summary>
        /// Sets a quest's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="state">The quest's new state.</param>
        public static void SetQuestState(string questID, QuestState state, string questerID = null)
        {
            var quest = GetQuestInstance(questID, questerID);
            if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: SetQuestState(" + questID + ", " + state + "): Couldn't find a quest with ID '" + questID + "'.");
                return;
            }
            quest.SetState(state);
        }

        /// <summary>
        /// Sets a quest's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="state">The quest's new state.</param>
        public static void SetQuestState(StringField questID, QuestState state, string questerID = null)
        {
            SetQuestState(StringField.GetStringValue(questID), state, questerID);
        }

        /// <summary>
        /// Looks up a quest node's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="questNodeID">The quest node's ID.</param>
        /// <returns>The quest node's state, or QuestNodeState.Inactive if no quest or quest node with the specified IDs has been registered.</returns>
        public static QuestNodeState GetQuestNodeState(string questID, string questNodeID, string questerID = null)
        {
            var quest = GetQuestInstance(questID, questerID);
            if (quest == null)
            {
                //--- Unnecessary warning. In this case, user probably just wants Inactive.
                //if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GetQuestNodeState(" + questID + ", " + questNodeID + "): Couldn't find a quest with ID '" + questID + "'.");
                return QuestNodeState.Inactive;
            }
            var node = quest.GetNode(questNodeID);
            if (node == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: GetQuestNodeState(" + questID + ", " + questNodeID + "): Quest doesn't have a node with ID '" + questNodeID + "'.");
                return QuestNodeState.Inactive;
            }
            return node.GetState();
        }

        /// <summary>
        /// Looks up a quest node's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="questNodeID">The quest node's ID.</param>
        /// <returns>The quest node's state, or QuestNodeState.Inactive if no quest or quest node with the specified IDs has been registered.</returns>
        public static QuestNodeState GetQuestNodeState(StringField questID, StringField questNodeID, string questerID = null)
        {
            return GetQuestNodeState(StringField.GetStringValue(questID), StringField.GetStringValue(questNodeID), questerID);
        }

        /// <summary>
        /// Sets a quest node's state.
        /// </summary>
        /// <param name="questID">The quest's ID.</param>
        /// <param name="questNodeID">The quest node's ID.</param>
        /// <param name="state">The quest node's new state.</param>
        public static void SetQuestNodeState(string questID, string questNodeID, QuestNodeState state, string questerID = null)
        {
            var quest = GetQuestInstance(questID, questerID);
            if (quest == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: SetQuestNodeState(" + questID + ", " + questNodeID + ", " + state + "): Couldn't find a quest with ID '" + questID + "'.");
                return;
            }
            var node = quest.GetNode(questNodeID);
            if (node == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Quest Machine: SetQuestNodeState(" + questID + ", " + questNodeID + ", " + state + "): Quest doesn't have a node with ID '" + questNodeID + "'.");
                return;
            }
            node.SetState(state);
        }

        public static void SetQuestNodeState(StringField questID, StringField questNodeID, QuestNodeState state, string questerID = null)
        {
            SetQuestNodeState(StringField.GetStringValue(questID), StringField.GetStringValue(questNodeID), state, questerID);
        }

        #endregion

    }
}
