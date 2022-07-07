// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility methods used by the custom editors.
    /// </summary>
    public static class QuestEditorUtility
    {

        #region Class Types

        public static System.Type GetWrapperType(System.Type type)
        {
            try
            {
                if (string.Equals(type.Namespace, "PixelCrushers.QuestMachine"))
                {
                    var wrapperName = "PixelCrushers.QuestMachine.Wrappers." + type.Name;
                    var assemblies = RuntimeTypeUtility.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        try
                        {
                            var wrapperList = (from assemblyType in assembly.GetExportedTypes()
                                               where string.Equals(assemblyType.FullName, wrapperName)
                                               select assemblyType).ToArray();
                            if (wrapperList.Length > 0) return wrapperList[0];
                        }
                        catch (System.Exception)
                        {
                            // Ignore exceptions and try next assembly.
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Ignore exceptions.
            }
            return null;
        }

        public static bool HasWrapperType(System.Type type)
        {
            return GetWrapperType(type) != null;
        }

        public static List<Type> GetSubtypes<T>() where T : class
        {
            var subtypes = TypeUtility.GetSubtypes<T>();
            subtypes.RemoveAll(x => HasWrapperType(x));
            return subtypes;
        }

        #endregion

        #region Editor GUI

        public static bool EditorGUILayoutFoldout(string label, string tooltip, bool foldout, bool topLevel = true)
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = foldout ? QuestEditorStyles.collapsibleHeaderOpenColor : QuestEditorStyles.collapsibleHeaderClosedColor;
#if UNITY_2019_1_OR_NEWER
                var text = label;
#else
                var text = topLevel ? ("<b>" + label + "</b>") : label;
#endif
                var guiContent = new GUIContent((foldout ? QuestEditorStyles.FoldoutOpenArrow : QuestEditorStyles.FoldoutClosedArrow) + text, tooltip);
                var guiStyle = topLevel ? QuestEditorStyles.CollapsibleHeaderButtonStyleName : QuestEditorStyles.CollapsibleSubheaderButtonStyleName;
                if (!GUILayout.Toggle(true, guiContent, guiStyle))
                {
                    foldout = !foldout;
                }
                GUI.backgroundColor = Color.white;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
            return foldout;
        }

        public static void EditorGUILayoutVerticalSpace(float pixels)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(pixels);
            EditorGUILayout.EndVertical();
        }

        public static void EditorGUILayoutBeginGroup()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(QuestEditorStyles.GroupBoxStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight));
            GUILayout.BeginVertical();
            GUILayout.Space(2);
        }

        public static void EditorGUILayoutEndGroup()
        {
            try
            {
                GUILayout.Space(3);
                GUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
            }
            catch (ArgumentException)
            {
                // If Unity opens a popup bwindow such as a color picker, it raises an exception.
            }
        }

        public static void EditorGUILayoutBeginIndent()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            EditorGUILayout.BeginVertical();
        }

        public static void EditorGUILayoutEndIndent()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        public static void RemoveReorderableListElementWithoutLeavingNull(UnityEditorInternal.ReorderableList list)
        {
            // If an objectReferenceValue is assigned to the list element, the default DoRemoveButton method
            // will only unassign it but it won't actually delete the element. To cleanly delete the element,
            // this method first unassigns the objectReferenceValue, then calls DoRemoveButton.
            if (!(list != null && 0 <= list.index && list.index < list.serializedProperty.arraySize)) return;
            var element = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (element.propertyType == SerializedPropertyType.ObjectReference)
            {
                element.objectReferenceValue = null;
            }
            UnityEditorInternal.ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        public static string[] GetCounterNames()
        {
            if (QuestEditorWindow.selectedQuest == null || QuestEditorWindow.selectedQuest.counterList == null) return null;
            var nameList = new string[QuestEditorWindow.selectedQuest.counterList.Count];
            for (int i = 0; i < QuestEditorWindow.selectedQuest.counterList.Count; i++)
            {
                var counter = QuestEditorWindow.selectedQuest.counterList[i];
                nameList[i] = (counter != null) ? StringField.GetStringValue(counter.name) : "<unassigned>";
            }
            return nameList;
        }

        public static void EditorGUILayoutCounterNamePopup(SerializedProperty counterIndexProperty, string[] nameList)
        {
            if (counterIndexProperty == null) return;
            if (nameList != null)
            {
                counterIndexProperty.intValue = EditorGUILayout.Popup("Counter", counterIndexProperty.intValue, nameList);
            }
            else
            {
                EditorGUILayout.PropertyField(counterIndexProperty);
            }
        }

        public static void EditorGUICounterNamePopup(Rect rect, SerializedProperty counterIndexProperty, string[] nameList)
        {
            if (counterIndexProperty == null) return;
            if (nameList != null)
            {
                counterIndexProperty.intValue = EditorGUI.Popup(rect, counterIndexProperty.intValue, nameList);
            }
            else
            {
                EditorGUI.PropertyField(rect, counterIndexProperty, GUIContent.none, true);
            }
        }

        public static void SetMessageParticipantID(SerializedProperty senderSpecifierProperty, SerializedProperty senderIDProperty)
        {
            if (senderSpecifierProperty == null || senderIDProperty == null) return;
            switch ((QuestMessageParticipant)senderSpecifierProperty.enumValueIndex)
            {
                case QuestMessageParticipant.Quester:
                    SetTextFieldProperty(senderIDProperty, QuestMachineTags.QUESTERID);
                    break;
                case QuestMessageParticipant.QuestGiver:
                    SetTextFieldProperty(senderIDProperty, QuestMachineTags.QUESTGIVERID);
                    break;
                case QuestMessageParticipant.Any:
                    SetTextFieldProperty(senderIDProperty, string.Empty);
                    break;
            }
        }

        public static void SetTextFieldProperty(SerializedProperty textFieldProperty, string value)
        {
            if (textFieldProperty == null) return;
            var textProperty = textFieldProperty.FindPropertyRelative("m_text");
            var stringAssetProperty = textFieldProperty.FindPropertyRelative("m_stringAsset");
            var textTableProperty = textFieldProperty.FindPropertyRelative("m_textTable");
            if (textProperty != null) textProperty.stringValue = value;
            if (stringAssetProperty != null) stringAssetProperty.objectReferenceValue = null;
            if (textTableProperty != null) textTableProperty.objectReferenceValue = null;
        }

        #endregion

        #region Destroy Quest Immediate

        /// <summary>
        /// Call this to destroy quest instances in the editor (i.e., using DestroyImmediate).
        /// </summary>
        public static void DestroyQuestImmediate(Quest quest)
        {
            if (quest.isInstance)
            {
                quest.isInstance = false;
                DestroyImmediateConditionSetSubassets(quest.autostartConditionSet);
                DestroyImmediateConditionSetSubassets(quest.offerConditionSet);
                DestroyImmediateUIContentListSubassets(quest.offerConditionsUnmetContentList);
                DestroyImmediateUIContentListSubassets(quest.offerContentList);
                DestroyImmediateStateInfoListSubassets(quest.stateInfoList);
                DestroyImmediateNodeListSubassets(quest.nodeList);
                UnityEngine.Object.DestroyImmediate(quest);
            }
        }

        private static void DestroyImmediateQuestSubassetsList<T>(List<T> subassets) where T : QuestSubasset
        {
            if (subassets == null) return;
            for (int i = 0; i < subassets.Count; i++)
            {
                UnityEngine.Object.DestroyImmediate(subassets[i]);
            }
        }

        private static void DestroyImmediateConditionSetSubassets(QuestConditionSet conditionSet)
        {
            if (conditionSet == null || conditionSet.conditionList == null) return;
            DestroyImmediateQuestSubassetsList(conditionSet.conditionList);
        }

        private static void DestroyImmediateUIContentListSubassets(List<QuestContent> uiContentList)
        {
            DestroyImmediateQuestSubassetsList(uiContentList);
        }

        private static void DestroyImmediateActionListSubassets(List<QuestAction> actions)
        {
            DestroyImmediateQuestSubassetsList(actions);
        }

        private static void DestroyImmediateStateInfoListSubassets(List<QuestStateInfo> stateInfoList)
        {
            if (stateInfoList == null) return;
            for (int i = 0; i < stateInfoList.Count; i++)
            {
                DestroyImmediateActionListSubassets(stateInfoList[i].actionList);
                if (stateInfoList[i].categorizedContentList == null) continue;
                for (int j = 0; j < stateInfoList[i].categorizedContentList.Count; j++)
                {
                    DestroyImmediateUIContentListSubassets(stateInfoList[i].categorizedContentList[j].contentList);
                }
            }
        }

        private static void DestroyImmediateNodeListSubassets(List<QuestNode> nodes)
        {
            if (nodes == null) return;
            for (int i = 0; i < nodes.Count; i++)
            {
                DestroyImmediateConditionSetSubassets(nodes[i].conditionSet);
                DestroyImmediateStateInfoListSubassets(nodes[i].stateInfoList);
            }
        }

        #endregion

        #region Arrange Nodes

        public static void ArrangeNodes(Quest quest, List<int> nodeIndicesToArrange)
        {
            if (quest == null) return;
            Undo.RecordObject(quest, "Arrange Nodes");

            var allNodes = nodeIndicesToArrange.Count <= 1;
            var nodeIndices = new List<int>();
            var offset = Vector2.zero;

            // Prepare the list of nodes that we need to arrange:
            if (allNodes)
            {
                for (int i = 1; i < quest.nodeList.Count; i++)
                {
                    nodeIndices.Add(i);
                }
            }
            else
            {
                nodeIndices.AddRange(nodeIndicesToArrange);
                offset = new Vector2(QuestCanvasGUI.CanvasWidth, QuestCanvasGUI.CanvasHeight);
                foreach (var index in nodeIndices)
                {
                    if (!(0 <= index && index < quest.nodeList.Count)) continue;
                    var node = quest.nodeList[index];
                    offset.x = Mathf.Min(offset.x, node.canvasRect.x);
                    offset.y = Mathf.Min(offset.y, node.canvasRect.y);
                }
            }

            // Prepare to build tree:
            var tree = new List<List<int>>();
            int rootIndex = 0;
            if (!allNodes)
            {
                // If arranging a subset, find a root node that isn't linked from others in the subset:
                var childNodeIndices = new HashSet<int>();
                foreach (var index in nodeIndices)
                {
                    if (!(0 <= index && index < quest.nodeList.Count)) continue;
                    var node = quest.nodeList[index];
                    foreach (var childIndex in node.childIndexList)
                    {
                        childNodeIndices.Add(childIndex);
                    }
                }
                foreach (var index in nodeIndices)
                {
                    if (!childNodeIndices.Contains(index))
                    {
                        rootIndex = index;
                        break;
                    }
                }
            }

            // Build tree:
            tree.Add(new List<int>(new int[] { rootIndex }));
            var unassigned = new List<int>();
            unassigned.AddRange(nodeIndices);
            unassigned.Remove(rootIndex);
            var maxLevelSize = ArrangeNodes_BuildTree(quest, nodeIndices, tree, unassigned, 0);

            // Place nodes:
            var padding = QuestNode.DefaultNodeWidth / 4;
            var treeWidth = maxLevelSize * (QuestNode.DefaultNodeWidth + padding);
            for (int i = 0; i < tree.Count; i++)
            {
                var level = tree[i];
                var levelWidth = level.Count * (QuestNode.DefaultNodeWidth + padding);
                var left = padding + ((treeWidth - levelWidth) / 2);
                var top = QuestNode.DefaultNodeHeight + i * (1.5f * QuestNode.DefaultNodeHeight);
                for (int j = 0; j < level.Count; j++)
                {
                    var node = quest.nodeList[level[j]];
                    var nodeLeft = left + j * (QuestNode.DefaultNodeWidth + padding);
                    node.canvasRect = new Rect(nodeLeft + offset.x, top + offset.y, node.canvasRect.width, node.canvasRect.height);
                }
            }

            // Place unassigned nodes:
            if (unassigned.Count > 0)
            {
                float farthestRight = 0;
                for (int i = 0; i < quest.nodeList.Count; i++)
                {
                    if (!unassigned.Contains(i))
                    {
                        var placedNode = quest.nodeList[i];
                        farthestRight = Mathf.Max(farthestRight, placedNode.canvasRect.x);
                    }
                }
                float unassignedNodeLeft = farthestRight + padding + QuestNode.DefaultNodeWidth + padding;
                for (int i = 0; i < unassigned.Count; i++)
                {
                    var top = QuestNode.DefaultNodeHeight + i * (1.5f * QuestNode.DefaultNodeHeight);
                    var unassignedNode = quest.nodeList[unassigned[i]];
                    unassignedNode.canvasRect = new Rect(unassignedNodeLeft, top, unassignedNode.canvasRect.width, unassignedNode.canvasRect.height);
                }
            }
        }

        private static int ArrangeNodes_BuildTree(Quest quest, List<int> nodeIndices, List<List<int>> tree, List<int> unassigned, int safeguard)
        {
            if (safeguard > 9999) return 0; // Prevent infinite recursion in case of bug.
            var level = tree[tree.Count - 1];
            var children = new List<int>();
            for (int i = 0; i < level.Count; i++)
            {
                var parentIndex = level[i];
                var parentNode = quest.nodeList[parentIndex];
                foreach (var childIndex in parentNode.childIndexList)
                {
                    if (!nodeIndices.Contains(childIndex)) continue;
                    if (unassigned.Contains(childIndex))
                    {
                        unassigned.Remove(childIndex);
                        children.Add(childIndex);
                    }
                }
            }
            if (children.Count > 0)
            {
                tree.Add(children);
                return Mathf.Max(level.Count, ArrangeNodes_BuildTree(quest, nodeIndices, tree, unassigned, safeguard++));
            }
            else
            {
                return level.Count;
            }
        }

        #endregion

    }

}