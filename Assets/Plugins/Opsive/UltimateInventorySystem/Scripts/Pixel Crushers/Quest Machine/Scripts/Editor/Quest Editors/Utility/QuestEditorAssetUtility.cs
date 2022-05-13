// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility methods used by the custom editors to manage assets, namely to
    /// save a quest ScriptableObject and all of its sub-ScriptableObjects to
    /// an asset file.
    /// </summary>
    public static class QuestEditorAssetUtility
    {

        #region Create/Save

        private static string s_lastDirectory = "Assets";

        public static Quest SaveQuestAsAsset(Quest quest, string filePath, bool select = false)
        {
            if (filePath.StartsWith(Application.dataPath))
            {
                filePath = "Assets" + filePath.Substring(Application.dataPath.Length);
            }
            var questAsset = quest.Clone();
            questAsset.isInstance = false;
            var existingAsset = AssetDatabase.LoadAssetAtPath<Quest>(filePath);
            if (existingAsset != null)
            {
                // If overwriting an existing asset (to keep its GUID intact), use the existing one but copy the new values:
                CopyQuestValues(questAsset, existingAsset);
                questAsset = existingAsset;
                var assets = AssetDatabase.LoadAllAssetsAtPath(filePath);
                foreach (var asset in assets)
                {
                    if (!(asset is Quest)) AssetUtility.DeleteFromAsset(asset as ScriptableObject, questAsset);
                }
            }
            else
            {
                // Otherwise we can create it fresh:
                AssetDatabase.CreateAsset(questAsset, filePath);
            }
            SaveQuestSubassets(questAsset);
            //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(questAsset));
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
            if (select)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = questAsset;
            }
            return questAsset;
        }

        private static void CopyQuestValues(Quest source, Quest dest)
        {
            dest.originalAsset = null;
            dest.id = source.id;
            dest.title = source.title;
            dest.icon = source.icon;
            dest.group = source.group;
            dest.labels = source.labels;
            dest.questGiverID = source.questGiverID;
            dest.isTrackable = source.isTrackable;
            dest.showInTrackHUD = source.showInTrackHUD;
            dest.isAbandonable = source.isAbandonable;
            dest.rememberIfAbandoned = source.rememberIfAbandoned;
            dest.autostartConditionSet = source.autostartConditionSet;
            dest.offerConditionSet = source.offerConditionSet;
            dest.offerContentList = source.offerContentList;
            dest.maxTimes = source.maxTimes;
            dest.cooldownSeconds = source.cooldownSeconds;
            dest.SetStateRaw(source.GetState());
            dest.stateInfoList = source.stateInfoList;
            dest.counterList = source.counterList;
            dest.nodeList = source.nodeList;
            dest.goalEntityTypeName = source.goalEntityTypeName;
        }

        private static void SaveQuestSubassets(Quest questAsset)
        {
            SaveConditionSetSubassets(questAsset, questAsset.autostartConditionSet);
            SaveConditionSetSubassets(questAsset, questAsset.offerConditionSet);
            SaveUIContentListSubassets(questAsset, questAsset.offerConditionsUnmetContentList);
            SaveUIContentListSubassets(questAsset, questAsset.offerContentList);
            SaveStateInfoListSubassets(questAsset, questAsset.stateInfoList);
            SaveNodeListSubassets(questAsset, questAsset.nodeList);
        }

        private static void SaveQuestSubassetsList<T>(Quest questAsset, List<T> subassets) where T : QuestSubasset
        {
            if (subassets == null) return;
            for (int i = 0; i < subassets.Count; i++)
            {
                //--- Was: AssetUtility.AddToAsset(subassets[i], questAsset);
                //--- Don't save until end. Keeps questAsset reference valid, and runs faster.
                subassets[i].hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(subassets[i], questAsset);
            }
        }

        private static void SaveConditionSetSubassets(Quest questAsset, QuestConditionSet conditionSet)
        {
            if (conditionSet == null || conditionSet.conditionList == null) return;
            SaveQuestSubassetsList(questAsset, conditionSet.conditionList);
        }

        private static void SaveUIContentListSubassets(Quest questAsset, List<QuestContent> uiContentList)
        {
            SaveQuestSubassetsList(questAsset, uiContentList);
        }

        private static void SaveActionListSubassets(Quest questAsset, List<QuestAction> actions)
        {
            SaveQuestSubassetsList(questAsset, actions);
        }

        private static void SaveStateInfoListSubassets(Quest questAsset, List<QuestStateInfo> stateInfoList)
        {
            if (stateInfoList == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                SaveActionListSubassets(questAsset, stateInfoList[i].actionList);
                if (stateInfoList[i].categorizedContentList == null) continue;
                for (int j = 0; j < stateInfoList[i].categorizedContentList.Count; j++)
                {
                    SaveUIContentListSubassets(questAsset, stateInfoList[i].categorizedContentList[j].contentList);
                }
            }
        }

        private static void SaveNodeListSubassets(Quest questAsset, List<QuestNode> nodes)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Count; i++)
            {
                SaveConditionSetSubassets(questAsset, nodes[i].conditionSet);
                SaveStateInfoListSubassets(questAsset, nodes[i].stateInfoList);
            }
        }

        public static Quest CreateNewQuestAssetFromDialog()
        {
            var filePath = EditorUtility.SaveFilePanel("Save Quest As", s_lastDirectory, string.Empty, "asset");
            if (string.IsNullOrEmpty(filePath)) return null;
            s_lastDirectory = System.IO.Path.GetDirectoryName(filePath);
            var questWrapperType = QuestEditorUtility.GetWrapperType(typeof(Quest));
            if (questWrapperType == null)
            {
                Debug.LogError("Quest Machine: Internal error. Can't access Quest type!");
                return null;
            }
            var quest = ScriptableObjectUtility.CreateScriptableObject(questWrapperType) as Quest;
            if (quest == null)
            {
                Debug.LogError("Quest Machine: Internal error. Can't create Quest object!");
                return null;
            }
            var filename = System.IO.Path.GetFileNameWithoutExtension(filePath);
            quest.id.value = filename.Replace(" ", string.Empty);
            quest.title.value = filename;
            return SaveQuestAsAsset(quest, filePath, false);
        }

        /// <summary>
        /// Assumes quest is an asset. Adds subassets in node (typically a newly copy-pasted node)
        /// to the asset.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="quest"></param>
        public static void AddNodeSubassetsToAsset(QuestNode node, Quest quest)
        {
            node.conditionSet.conditionList.ForEach(condition => AddSubassetToAsset(condition, quest));
            foreach (var stateInfo in node.stateInfoList)
            {
                foreach (var category in stateInfo.categorizedContentList)
                {
                    category.contentList.ForEach(content => AddSubassetToAsset(content, quest));
                }
                foreach (var action in stateInfo.actionList)
                {
                    AddSubassetToAsset(action, quest);
                    if (action is AlertQuestAction)
                    {
                        (action as AlertQuestAction).contentList.ForEach(content => AddSubassetToAsset(content, quest));
                    }
                }
            }
        }

        public static void AddSubassetToAsset(QuestSubasset subasset, Quest quest)
        {
            AssetUtility.AddToAsset(subasset, quest);
            subasset.SetRuntimeReferences(quest, null);
        }

        #endregion

        #region Delete Unused Subassets

        public static void DeleteUnusedSubassets(Quest quest)
        {
            // Get all assets in quest:
            var assetPath = AssetDatabase.GetAssetPath(quest);
            if (string.IsNullOrEmpty(assetPath)) return;
            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            // Get list of used subassets:
            var usedSubassets = new List<QuestSubasset>();
            AddSubassetsToList(quest, usedSubassets);

            // Delete unused subassets:
            for (int i = 0; i < assets.Length; i++)
            {
                var subasset = assets[i] as QuestSubasset;
                if (subasset != null && !usedSubassets.Contains(subasset))
                {
                    var details = (subasset is QuestContent) ? (" ('" + StringField.GetStringValue((subasset as QuestContent).originalText) + "')") : string.Empty;
                    if (subasset is HeadingTextQuestContent && (subasset as HeadingTextQuestContent).useQuestTitle) details = "(quest title)";
                    Debug.Log("Quest Machine: Cleanup: Deleting unused " + subasset.GetType().Name + " from " + quest.name + details, quest);
                     AssetUtility.DeleteFromAsset(subasset, quest);
                }
            }
        }

        public static void AddSubassetsToList(Quest quest, List<QuestSubasset> subassets)
        {
            if (quest == null || subassets == null) return;
            AddMainQuestSubassetsToList(quest, subassets);
            for (int i = 0; i < quest.nodeList.Count; i++)
            {
                AddNodeSubassetsToList(quest.nodeList[i], subassets);
            }
        }

        private static void AddMainQuestSubassetsToList(Quest quest, List<QuestSubasset> subassets)
        {
            if (quest == null || subassets == null) return;
            AddConditionSetSubassetsToList(quest.autostartConditionSet, subassets);
            AddConditionSetSubassetsToList(quest.offerConditionSet, subassets);
            AddSubassetListSubassetsToList(quest.offerConditionsUnmetContentList, subassets);
            AddSubassetListSubassetsToList(quest.offerContentList, subassets);
            AddStateInfoListSubassetsToList(quest.stateInfoList, subassets);
        }

        private static void AddConditionSetSubassetsToList(QuestConditionSet conditionSet, List<QuestSubasset> subassets)
        {
            if (conditionSet == null || conditionSet.conditionList == null || subassets == null) return;
            AddSubassetListSubassetsToList(conditionSet.conditionList, subassets);
        }

        private static void AddStateInfoListSubassetsToList(List<QuestStateInfo> stateInfoList, List<QuestSubasset> subassets)
        {
            if (stateInfoList == null || subassets == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                var stateInfo = stateInfoList[i];
                if (stateInfo == null) continue;
                AddSubassetListSubassetsToList(stateInfo.actionList, subassets);
                AddCategorizedContentListSubassetsToList(stateInfo.categorizedContentList, subassets);
            }
        }

        private static void AddCategorizedContentListSubassetsToList(List<QuestContentSet> categorizedContentList, List<QuestSubasset> subassets)
        {
            if (categorizedContentList == null || subassets == null) return;
            for (int i = 0; i < categorizedContentList.Count; i++)
            {
                var contentSet = categorizedContentList[i];
                if (contentSet == null) continue;
                AddSubassetListSubassetsToList(contentSet.contentList, subassets);
            }
        }

        private static void AddNodeSubassetsToList(QuestNode questNode, List<QuestSubasset> subassets)
        {
            if (questNode == null || subassets == null) return;
            AddConditionSetSubassetsToList(questNode.conditionSet, subassets);
            AddStateInfoListSubassetsToList(questNode.stateInfoList, subassets);
        }

        private static void AddSubassetListSubassetsToList<T>(List<T> subassetList, List<QuestSubasset> subassets) where T : QuestSubasset
        {
            if (subassetList == null || subassets == null) return;
            for (int i = 0; i < subassetList.Count; i++)
            {
                subassets.Add(subassetList[i]);
                if (subassetList[i] is AlertQuestAction)
                {
                    AddSubassetListSubassetsToList((subassetList[i] as AlertQuestAction).contentList, subassets);
                }
                if (subassetList[i] is ButtonQuestContent)
                {
                    AddSubassetListSubassetsToList((subassetList[i] as ButtonQuestContent).actionList, subassets);
                }
            }
        }

        #endregion

    }

}
