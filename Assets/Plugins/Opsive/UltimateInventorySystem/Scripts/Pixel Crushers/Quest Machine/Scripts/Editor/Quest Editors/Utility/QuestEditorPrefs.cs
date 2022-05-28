// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Quest editor preferences.
    /// Stores editor preferences in a single EditorPrefs key as serialized XML.
    /// </summary>
    public static class QuestEditorPrefs
    {

        public const string QuestEditorPrefsKey = "PixelCrushers.QuestMachine.EditorPrefs";

        [Serializable]
        public class Data
        {
            public bool zoomLock = false;
            public bool mainInfoFoldout = true;
            public bool autostartFoldout = false;
            public bool offerFoldout = false;
            public bool countersFoldout = false;
            public bool actionsFoldout = false;
            public bool nodesFoldout = false;
            public bool nodeMainInfoFoldout = true;
            public bool successConnectionFoldout = false;
            public bool failureConnectionFoldout = false;
            public bool trueConnectionFoldout = false;
            public List<bool> stateInfoFoldouts = new List<bool>();
            public List<bool> inactiveFoldouts = new List<bool>();
            public List<bool> activeFoldouts = new List<bool>();
            public List<bool> successfulFoldouts = new List<bool>();
            public List<bool> failedFoldouts = new List<bool>();
            public List<bool> abandonedFoldouts = new List<bool>();
            public List<bool> trueFoldouts = new List<bool>();
            public List<bool> dialogueContentFoldouts = new List<bool>();
            public List<bool> journalContentFoldouts = new List<bool>();
            public List<bool> hudContentFoldouts = new List<bool>();
            public List<bool> alertContentFoldouts = new List<bool>();
            public List<bool> offerContentFoldouts = new List<bool>();
            public List<bool> offerConditionsUnmetContentFoldouts = new List<bool>();
            public List<bool> actionFoldouts = new List<bool>();
            public bool actionMessagesFoldout = true;
            public bool actionCounterValuesFoldout = true;
            public bool actionOnExecuteFoldout = true;
            public float runtimeRepaintFrequency = 1;

            public bool questListContainerSaveSettingsFoldout = true;
            public bool questListContainerOtherSettingsFoldout = true;
            public bool questGiverIDFoldout = true;
            public bool questGiverDialogueContentFoldout = true;
            public bool questListUIFoldout = true;

            public bool actionTextFoldout = true;
            public bool actionMotivesFoldout = true;
            public bool actionRequirementsFoldout = true;
            public bool actionEffectsFoldout = true;
            public bool actionCompletionConditionsFoldout = true;

            public bool entityTypeParentsFoldout = true;
            public bool entityTypeUrgencyFoldout = true;
            public bool entityTypeActionsFoldout = true;
            public bool entityTypeDriveFoldout = true;
            public bool entityTypeRewardMultipliersFoldout = false;

            public bool databaseImagesFoldout = false;
        }

        private static Data m_data = null;

        private static Data data
        {
            get
            {
                if (m_data == null) m_data = LoadData();
                return m_data;
            }
        }

        private static Data LoadData()
        {
            if (!EditorPrefs.HasKey(QuestEditorPrefsKey)) return new Data();
            var xml = EditorPrefs.GetString(QuestEditorPrefsKey);
            var xmlSerializer = new XmlSerializer(typeof(Data));
            return (xmlSerializer.Deserialize(new StringReader(xml)) as Data) ?? new Data();
        }

        public static void SavePrefs()
        {
            if (m_data == null) return;
            var xmlSerializer = new XmlSerializer(typeof(Data));
            var writer = new StringWriter();
            xmlSerializer.Serialize(writer, m_data);
            EditorPrefs.SetString(QuestEditorPrefsKey, writer.ToString());
        }

        public static bool zoomLock
        {
            get { return data.zoomLock; }
            set { data.zoomLock = value; }
        }

        public static bool mainInfoFoldout
        {
            get { return data.mainInfoFoldout; }
            set { data.mainInfoFoldout = value; }
        }

        public static bool autostartFoldout
        {
            get { return data.autostartFoldout; }
            set { data.autostartFoldout = value; }
        }

        public static bool offerFoldout
        {
            get { return data.offerFoldout; }
            set { data.offerFoldout = value; }
        }

        public static bool countersFoldout
        {
            get { return data.countersFoldout; }
            set { data.countersFoldout = value; }
        }

        public static bool actionsFoldout
        {
            get { return data.actionsFoldout; }
            set { data.actionsFoldout = value; }
        }

        public static bool nodesFoldout
        {
            get { return data.nodesFoldout; }
            set { data.nodesFoldout = value; }
        }

        public static bool nodeMainInfoFoldout
        {
            get { return data.nodeMainInfoFoldout; }
            set { data.nodeMainInfoFoldout = value; }
        }

        public static bool successConnectionFoldout
        {
            get { return data.successConnectionFoldout; }
            set { data.successConnectionFoldout = value; }
        }

        public static bool failureConnectionFoldout
        {
            get { return data.failureConnectionFoldout; }
            set { data.failureConnectionFoldout = value; }
        }

        public static bool trueConnectionFoldout
        {
            get { return data.trueConnectionFoldout; }
            set { data.trueConnectionFoldout = value; }
        }

        public static bool actionMessagesFoldout
        {
            get { return data.actionMessagesFoldout; }
            set { data.actionMessagesFoldout = value; }
        }

        public static bool actionCounterValuesFoldout
        {
            get { return data.actionCounterValuesFoldout; }
            set { data.actionCounterValuesFoldout = value; }
        }

        public static bool actionOnExecuteFoldout
        {
            get { return data.actionOnExecuteFoldout; }
            set { data.actionOnExecuteFoldout = value; }
        }

        public static float runtimeRepaintFrequency
        {
            get { return data.runtimeRepaintFrequency; }
            set { data.runtimeRepaintFrequency = value; }
        }

        public static bool GetStateInfoFoldout(int nodeIndex)
        {
            VerifyListSize(data.stateInfoFoldouts, nodeIndex, true);
            return data.stateInfoFoldouts[nodeIndex];
        }

        public static void ToggleStateInfoFoldout(int nodeIndex)
        {
            data.stateInfoFoldouts[nodeIndex] = !GetStateInfoFoldout(nodeIndex);
        }

        public static bool GetQuestStateFoldout(QuestState questState, int nodeIndex)
        {
            var list = GetQuestStateFoldoutList(questState);
            VerifyListSize(list, nodeIndex, true);
            return list[nodeIndex];
        }

        public static void ToggleQuestNodeStateFoldout(QuestNodeState questNodeState, int nodeIndex)
        {
            var list = GetQuestNodeStateFoldoutList(questNodeState);
            list[nodeIndex] = !GetQuestNodeStateFoldout(questNodeState, nodeIndex);
        }

        public static bool GetQuestNodeStateFoldout(QuestNodeState questNodeState, int nodeIndex)
        {
            var list = GetQuestNodeStateFoldoutList(questNodeState);
            VerifyListSize(list, nodeIndex, true);
            return list[nodeIndex];
        }

        public static void ToggleQuestStateFoldout(QuestState questState, int nodeIndex)
        {
            var list = GetQuestStateFoldoutList(questState);
            list[nodeIndex] = !GetQuestStateFoldout(questState, nodeIndex);
        }

        public static bool GetQuestActionFoldout(QuestState questState)
        {
            var list = data.actionFoldouts;
            VerifyListSize(list, (int)questState, true);
            return list[(int)questState];
        }

        public static void ToggleQuestActionFoldout(QuestState questState)
        {
            var list = data.actionFoldouts;
            list[(int)questState] = !GetQuestActionFoldout(questState);
        }

        public static bool GetQuestContentFoldout(QuestContentCategory category, QuestState questState)
        {
            var list = GetQuestContentFoldoutList(category);
            VerifyListSize(list, (int)questState, false);
            return list[(int)questState];
        }

        public static void ToggleQuestContentFoldout(QuestContentCategory category, QuestState questState)
        {
            var list = GetQuestContentFoldoutList(category);
            list[(int)questState] = !GetQuestContentFoldout(category, questState);
        }

        private static List<bool> GetQuestStateFoldoutList(QuestState questState)
        {
            switch (questState)
            {
                case QuestState.WaitingToStart:
                    return data.inactiveFoldouts;
                case QuestState.Active:
                    return data.activeFoldouts;
                case QuestState.Successful:
                    return data.successfulFoldouts;
                case QuestState.Failed:
                    return data.failedFoldouts;
                case QuestState.Abandoned:
                    return data.abandonedFoldouts;
                default:
                    return null;
            }
        }

        private static List<bool> GetQuestNodeStateFoldoutList(QuestNodeState questNodeState)
        {
            switch (questNodeState)
            {
                case QuestNodeState.Inactive:
                    return data.inactiveFoldouts;
                case QuestNodeState.Active:
                    return data.activeFoldouts;
                case QuestNodeState.True:
                    return data.trueFoldouts;
                default:
                    return null;
            }
        }

        private static List<bool> GetQuestContentFoldoutList(QuestContentCategory category)
        {
            switch (category)
            {
                case QuestContentCategory.Dialogue:
                    return data.dialogueContentFoldouts;
                case QuestContentCategory.Journal:
                    return data.journalContentFoldouts;
                case QuestContentCategory.HUD:
                    return data.hudContentFoldouts;
                case QuestContentCategory.Alert:
                    return data.alertContentFoldouts;
                case QuestContentCategory.Offer:
                    return data.offerContentFoldouts;
                case QuestContentCategory.OfferConditionsUnmet:
                    return data.offerConditionsUnmetContentFoldouts;
                default:
                    return null;
            }
        }

        private static void VerifyListSize(List<bool> list, int index, bool defaultValue)
        {
            if (list == null) return;
            if (index >= list.Count)
            {
                for (int i = list.Count; i <= index; i++)
                {
                    list.Add(defaultValue);
                }
            }
        }

        public static bool questListContainerSaveSettingsFoldout
        {
            get { return data.questListContainerSaveSettingsFoldout; }
            set { data.questListContainerSaveSettingsFoldout = value; }
        }

        public static bool questListContainerOtherSettingsFoldout
        {
            get { return data.questListContainerOtherSettingsFoldout; }
            set { data.questListContainerOtherSettingsFoldout = value; }
        }

        public static bool questGiverIDFoldout
        {
            get { return data.questGiverIDFoldout; }
            set { data.questGiverIDFoldout = value; }
        }

        public static bool questGiverDialogueContentFoldout
        {
            get { return data.questGiverDialogueContentFoldout; }
            set { data.questGiverDialogueContentFoldout = value; }
        }

        public static bool questListUIFoldout
        {
            get { return data.questListUIFoldout; }
            set { data.questListUIFoldout = value; }
        }

        public static bool actionTextFoldout
        {
            get { return data.actionTextFoldout; }
            set { data.actionTextFoldout = value; }
        }

        public static bool actionMotivesFoldout
        {
            get { return data.actionMotivesFoldout; }
            set { data.actionMotivesFoldout = value; }
        }

        public static bool actionRequirementsFoldout
        {
            get { return data.actionRequirementsFoldout; }
            set { data.actionRequirementsFoldout = value; }
        }

        public static bool actionEffectsFoldout
        {
            get { return data.actionEffectsFoldout; }
            set { data.actionEffectsFoldout = value; }
        }

        public static bool actionCompletionConditionsFoldout
        {
            get { return data.actionCompletionConditionsFoldout; }
            set { data.actionCompletionConditionsFoldout = value; }
        }

        public static bool entityTypeParentsFoldout
        {
            get { return data.entityTypeParentsFoldout; }
            set { data.entityTypeParentsFoldout = value; }
        }

        public static bool entityTypeDriveFoldout
        {
            get { return data.entityTypeDriveFoldout; }
            set { data.entityTypeDriveFoldout = value; }
        }

        public static bool entityTypeUrgencyFoldout
        {
            get { return data.entityTypeUrgencyFoldout; }
            set { data.entityTypeUrgencyFoldout = value; }
        }

        public static bool entityTypeActionsFoldout
        {
            get { return data.entityTypeActionsFoldout; }
            set { data.entityTypeActionsFoldout = value; }
        }

        public static bool entityTypeRewardMultipliersFoldout
        {
            get { return data.entityTypeRewardMultipliersFoldout; }
            set { data.entityTypeRewardMultipliersFoldout = value; }
        }

        public static bool databaseImagesFoldout
        {
            get { return data.databaseImagesFoldout; }
            set { data.databaseImagesFoldout = value; }
        }
    }
}
