// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest Machine-specific messages used with the MessageSystem.
    /// </summary>
    public static class QuestMachineMessages
    {

        #region Messages

        /// <summary>
        /// Sent when a quest state or quest node state changes. 
        /// - Parameter: Quest ID. 
        /// - Argument 0: [StringField] Quest node ID, or null for main quest state.
        /// - Argument 1: [QuestState] / [QuestNodeState] New state.
        /// </summary>
        public const string QuestStateChangedMessage = "Quest State Changed";

        /// <summary>
        /// Sent when a quest's track toggle changes (i.e., turn progress tracking on or off).
        /// - Parameter: Quest ID.
        /// - Argument 0: [bool] New track state.
        /// </summary>
        public const string QuestTrackToggleChangedMessage = "Quest Track Toggle Changed";

        /// <summary>
        /// Sent when a quest state is abandoned.
        /// - Parameter: Quest ID. 
        /// </summary>
        public const string QuestAbandonedMessage = "Quest Abandoned";

        /// <summary>
        /// Sent to tell a quest to re-check its offer conditions in case
        /// the quest is no longer offerable.
        /// - Parameter: Quest ID.
        /// </summary>
        public const string CheckOfferConditionsMessage = "Check Offer Conditions";

        /// <summary>
        /// Sent when a quest's counter changes value.
        /// - Parameter: Quest ID.
        /// - Argument 0: [StringField] Counter name.
        /// - Argument 1: [int] New value.
        /// </summary>
        public const string QuestCounterChangedMessage = "Quest Counter Changed";

        /// <summary>
        /// Sent to tell a counter to set its value. 
        /// - Parameter: Quest ID.
        /// - Argument 0: Counter name.
        /// - Argument 1: [int] New value.
        /// </summary>
        public const string SetQuestCounterMessage = "Set Quest Counter";

        /// <summary>
        /// Sent every second if a timer condition is active.
        /// </summary>
        public const string TimerTickMessage = "Timer Tick";

        /// <summary>
        /// Sent to tell a counter to increment its value. 
        /// - Parameter: Quest ID.
        /// - Argument 1: [StringField] Counter name.
        /// - Argument 0: [int] Amount to increment (or decrement if negative).
        /// </summary>
        public const string IncrementQuestCounterMessage = "Increment Quest Counter";

        /// <summary>
        /// Sent to set an entity's quest indicator state.
        /// - Target: Entity ID.
        /// - Parameter: Quest ID.
        /// - Argument 0: [QuestIndicatorState] State.
        /// </summary>
        public const string SetIndicatorStateMessage = "Set Indicator State";

        /// <summary>
        /// Sent to tell an entity to refresh its indicator by checking all quests.
        /// - Target: Entity ID, or blank for all entities with QuestIndicatorManagers.
        /// - Parameter: Entity ID, or blank for all.
        /// </summary>
        public const string RefreshIndicatorMessage = "Refresh Indicator";

        /// <summary>
        /// Send to tell all UIs to refresh, typically after loading a saved game.
        /// </summary>
        public const string RefreshUIsMessage = "Refresh UIs";

        /// <summary>
        /// Sent when an alert message is raised. 
        /// - Parameter: Quest ID.
        /// - Argument 0: List of QuestUIContent items.
        /// </summary>
        public const string QuestAlertMessage = "Quest Alert";

        /// <summary>
        /// Sent just prior to starting dialogue with a QuestGiver. 
        /// - Target: QuestGiver being greeted.
        /// - Parameter: [StringField] QuestGiver's ID.
        /// </summary>
        public const string GreetMessage = "Greet";

        /// <summary>
        /// Sent just after starting dialogue with a QuestGiver.
        /// - Target: QuestGiver being greeted.
        /// - Parameter: [StringField] QuestGiver's ID.
        /// </summary>
        public const string GreetedMessage = "Greeted";

        /// <summary>
        /// Sent just prior to starting a dialogue about a specific quest.
        /// - Target: QuestGiver being talked to.
        /// - Parameter: [StringField] Quest ID.
        /// - Argument 0: [string] QuestGiver's ID.
        /// </summary>
        public const string DiscussQuestMessage = "Discuss Quest";

        /// <summary>
        /// Sent just after starting a dialogue about a specific quest.
        /// - Target: QuestGiver being talked to.
        /// - Parameter: [StringField] Quest ID.
        /// - Argument 0: [string] QuestGiver's ID.
        /// </summary>
        public const string DiscussedQuestMessage = "Discussed Quest";

        /// <summary>
        /// Send to start a spawner.
        /// - Parameter: [StringField/string] Spawner name.
        /// </summary>
        public const string StartSpawnerMessage = "Start Spawner";

        /// <summary>
        /// Send to stop a spawner.
        /// - Parameter: [StringField/string] Spawner name.
        /// </summary>
        public const string StopSpawnerMessage = "Stop Spawner";

        /// <summary>
        /// Send to stop a spawner and tell it to despawn all spawned objects.
        /// - Parameter: [StringField/string] Spawner name.
        /// </summary>
        public const string DespawnSpawnerMessage = "Despawn Spawner";

        /// <summary>
        /// Argument 0: [int] Group number.
        /// </summary>
        public const string GroupButtonClickedMessage = "Group Button Clicked";

        #endregion

        #region Utility Methods

        public static void QuestStateChanged(object sender, StringField questID, QuestState state)
        {
            MessageSystem.SendMessage(sender, QuestStateChangedMessage, questID, null, state);
        }

        public static void QuestNodeStateChanged(object sender, StringField questID, StringField questNodeID, QuestNodeState state)
        {
            MessageSystem.SendMessage(sender, QuestStateChangedMessage, questID, questNodeID, state);
        }

        public static void QuestTrackToggleChanged(object sender, StringField questID, bool state)
        {
            MessageSystem.SendMessage(sender, QuestTrackToggleChangedMessage, questID, state);
        }

        public static void QuestAbandoned(object sender, StringField questID)
        {
            MessageSystem.SendMessage(sender, QuestAbandonedMessage, questID);
        }

        public static void QuestCounterChanged(object sender, StringField questID, StringField counterName, int counterValue)
        {
            MessageSystem.SendMessage(sender, QuestCounterChangedMessage, questID, counterName, counterValue);
        }

        public static void SetQuestCounter(object sender, StringField questID, StringField counterName, int counterValue)
        {
            MessageSystem.SendMessage(sender, SetQuestCounterMessage, questID, counterName, counterValue);
        }

        public static void IncrementQuestCounter(object sender, StringField questID, StringField counterName, int amount)
        {
            MessageSystem.SendMessage(sender, IncrementQuestCounterMessage, questID, counterName, amount);
        }

        public static void SetIndicatorState(object sender, StringField entityID, StringField questID, QuestIndicatorState state)
        {
            MessageSystem.SendMessageWithTarget(sender, entityID, SetIndicatorStateMessage, questID, state);
        }

        public static void RefreshIndicator(object sender, StringField entityID)
        {
            MessageSystem.SendMessageWithTarget(sender, entityID, RefreshIndicatorMessage, entityID);
        }

        public static void RefreshIndicators(object sender)
        {
            RefreshIndicator(sender, null);
        }

        public static void RefreshUIs(object sender)
        {
            MessageSystem.SendMessage(sender, RefreshUIsMessage, string.Empty);
        }

        public static void QuestAlert(object sender, StringField questID, List<QuestContent> contents)
        {
            MessageSystem.SendMessage(sender, QuestAlertMessage, questID, contents);
        }

        public static void Greet(object sender, object target, StringField targetID)
        {
            MessageSystem.SendMessageWithTarget(sender, target, GreetMessage, targetID);
        }

        public static void Greeted(object sender, object target, StringField targetID)
        {
            MessageSystem.SendMessageWithTarget(sender, target, GreetedMessage, targetID);
        }

        public static void DiscussQuest(object sender, object target, StringField targetID, StringField questID)
        {
            MessageSystem.SendMessageWithTarget(sender, target, DiscussQuestMessage, questID, targetID);
        }

        public static void DiscussedQuest(object sender, object target, StringField targetID, StringField questID)
        {
            MessageSystem.SendMessageWithTarget(sender, target, DiscussedQuestMessage, questID, targetID);
        }

        public static void StartSpawner(StringField spawnerName)
        {
            MessageSystem.SendMessage(QuestMachineConfiguration.instance, StartSpawnerMessage, spawnerName);
        }

        public static void StopSpawner(StringField spawnerName)
        {
            MessageSystem.SendMessage(QuestMachineConfiguration.instance, StopSpawnerMessage, spawnerName);
        }

        public static void DespawnSpawner(StringField spawnerName)
        {
            MessageSystem.SendMessage(QuestMachineConfiguration.instance, DespawnSpawnerMessage, spawnerName);
        }

        public static void SendCompositeMessage(object sender, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (QuestMachine.debug) Debug.Log("Quest Machine: Sending to Message System: '" + message, sender as UnityEngine.Object);
            MessageSystem.SendCompositeMessage(sender, message);
        }

        #endregion

        #region Parameters & IDs

        /// <summary>
        /// Gets the string value of a message argument. Some arguments such as quest node IDs can be
        /// passed as StringField or string. This utility method simplifies retrieval of the value.
        /// </summary>
        public static string ArgToString(object arg)
        {
            var argType = (arg != null) ? arg.GetType() : null;
            if (argType == typeof(StringField)) return StringField.GetStringValue(arg as StringField);
            if (argType == typeof(string)) return (string)arg;
            return (arg != null) ? arg.ToString() : string.Empty;
        }

        /// <summary>
        /// Gets the int value of a message argument.
        /// </summary>
        public static int ArgToInt(object arg)
        {
            return (arg != null && arg.GetType() == typeof(int)) ? (int)arg : -1;
        }

        public static bool IsRequiredID(object obj, string id)
        {
            return string.IsNullOrEmpty(id) || StringField.Equals(GetID(obj), id);
        }

        /// <summary>
        /// Gets the ID of an object. This is either a QuestGiver ID, QuestEntity ID, 
        /// a string, or the default value.
        /// </summary>
        public static StringField GetID(object obj, StringField defaultID = null)
        {
            if (obj is GameObject)
            {
                var go = obj as GameObject;
                var questList = go.GetComponentInChildren<IdentifiableQuestListContainer>();
                if (questList != null) return questList.id;
                var entity = go.GetComponentInChildren<QuestEntity>();
                if (entity != null) return entity.id;
                var idComponent = go.GetComponentInChildren<IQuestMachineID>();
                if (idComponent != null) return idComponent.id;
                questList = go.GetComponentInParent<IdentifiableQuestListContainer>();
                if (questList != null) return questList.id;
                entity = go.GetComponentInParent<QuestEntity>();
                if (entity != null) return entity.id;
                idComponent = go.GetComponentInParent<IQuestMachineID>();
                if (idComponent != null) return idComponent.id;
            }
            else if (obj is IdentifiableQuestListContainer)
            {
                return (obj as IdentifiableQuestListContainer).id;
            }
            else if (obj is QuestEntity)
            {
                return (obj as QuestEntity).id;
            }
            else if (obj is IQuestMachineID)
            {
                return (obj as IQuestMachineID).id;
            }
            else if (obj is StringField)
            {
                return obj as StringField;
            }
            else if (obj != null && obj.GetType() == typeof(string))
            {
                return (new StringField((string)obj));
            }
            return defaultID;
        }

        /// <summary>
        /// Gets the display name of an object. This is either a QuestEntity ID, a string,
        /// or the default value.
        /// </summary>
        public static StringField GetDisplayName(object obj, StringField defaultDisplayName = null)
        {
            if (obj is GameObject)
            {
                var go = obj as GameObject;
                var questList = go.GetComponentInChildren<IdentifiableQuestListContainer>();
                if (questList != null) return questList.displayName;
                var entity = go.GetComponentInChildren<QuestEntity>();
                if (entity != null) return entity.displayName;
                var idComponent = go.GetComponentInChildren<IQuestMachineID>();
                if (idComponent != null) return idComponent.displayName;
                questList = go.GetComponentInParent<IdentifiableQuestListContainer>();
                if (questList != null) return questList.displayName;
                entity = go.GetComponentInParent<QuestEntity>();
                if (entity != null) return entity.displayName;
                idComponent = go.GetComponentInParent<IQuestMachineID>();
                if (idComponent != null) return idComponent.displayName;
            }
            else if (obj is IdentifiableQuestListContainer)
            {
                return (obj as IdentifiableQuestListContainer).displayName;
            }
            else if (obj is QuestEntity)
            {
                return (obj as QuestEntity).displayName;
            }
            else if (obj is IQuestMachineID)
            {
                return (obj as IQuestMachineID).displayName;
            }
            else if (obj is StringField)
            {
                return obj as StringField;
            }
            else if (obj != null && obj.GetType() == typeof(string))
            {
                return (new StringField((string)obj));
            }
            return defaultDisplayName;
        }

        /// <summary>
        /// Returns the GameObject with the specified ID, giving preference to quest list
        /// containers, then quest entities, and finally by GameObject name.
        /// </summary>
        /// <param name="id">ID to search for.</param>
        public static GameObject FindGameObjectWithID(StringField id)
        {
            return StringField.IsNullOrEmpty(id) ? null : FindGameObjectWithID(id.value);
        }

        /// <summary>
        /// Returns the GameObject with the specified ID, giving preference to quest list
        /// containers, then quest entities, and finally by GameObject name.
        /// </summary>
        /// <param name="id">ID to search for.</param>
        public static GameObject FindGameObjectWithID(string id)
        {
            // Try registered quest list containers first (quest givers, quest journal):
            var qlc = QuestMachine.GetQuestListContainer(id);
            if (qlc != null) return qlc.gameObject;

            //// Otherwise try entities:
            ////--- Replaced by IQuestMachineID below.
            //var entities = GameObject.FindObjectsOfType<QuestEntity>();
            //for (int i = 0; i < entities.Length; i++)
            //{
            //    var entity = entities[i];
            //    if (string.Equals(entity.id, id)) return entity.gameObject;
            //}

            var monobehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
            for (int i = 0; i < monobehaviours.Length; i++)
            {
                var identifiable = (monobehaviours[i] as IQuestMachineID);
                if (identifiable != null && string.Equals(identifiable.id, id)) return monobehaviours[i].gameObject;
            }

            // Otherwise search scene for GameObject name:
            return GameObject.Find(id);
        }

        #endregion

    }
}
