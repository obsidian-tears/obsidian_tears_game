// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Draws the node reordering editor GUI for a Quest.
    /// </summary>
    public class QuestNodeOrderInspectorGUI
    {

        private ReorderableList m_nodeList = null;
        private int m_nodeIndex = -1;

        public void Draw(SerializedObject serializedObject)
        {
            if (serializedObject == null) return;
            if (m_nodeList == null) SetupNodeList(serializedObject);
            m_nodeList.DoLayoutList();
        }

        private void SetupNodeList(SerializedObject serializedObject)
        {
            var nodeListProperty = serializedObject.FindProperty("m_nodeList");
            UnityEngine.Assertions.Assert.IsNotNull(nodeListProperty, "Quest Machine: Internal error - m_nodeList is null.");
            if (nodeListProperty == null) return;
            m_nodeList = new ReorderableList(serializedObject, nodeListProperty, true, true, false, false);
            m_nodeList.drawHeaderCallback = OnDrawNodeHeader;
            m_nodeList.drawElementCallback = OnDrawNodeElement;
            m_nodeList.onSelectCallback = OnSelectNode;
            m_nodeList.onReorderCallback = OnReorderNode;
        }

        private void OnDrawNodeHeader(Rect rect)
        {
            var fieldWidth = (rect.width - 14) / 2;
            EditorGUI.LabelField(new Rect(rect.x + 14, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), "ID");
            EditorGUI.LabelField(new Rect(rect.x + 14 + fieldWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), "Internal Name");
        }

        private void OnDrawNodeElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < m_nodeList.serializedProperty.arraySize)) return;
            var element = m_nodeList.serializedProperty.GetArrayElementAtIndex(index);
            var idProperty = element.FindPropertyRelative("m_id");
            var internalNameProperty = element.FindPropertyRelative("m_internalName");
            UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id is null.");
            UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName is null.");
            if (idProperty == null || internalNameProperty == null) return;
            EditorGUI.BeginDisabledGroup(true);
            try
            {
                EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                    StringFieldDrawer.GetStringFieldValue(idProperty));
                EditorGUI.TextField(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight),
                    StringFieldDrawer.GetStringFieldValue(internalNameProperty));
            }
            finally
            {
                EditorGUI.EndDisabledGroup();
            }
        }

        private void OnSelectNode(ReorderableList list)
        {
            m_nodeIndex = list.index;
        }

        private void OnReorderNode(ReorderableList list)
        {
            var oldIndex = m_nodeIndex;
            var newIndex = list.index;
            var nodesProperty = m_nodeList.serializedProperty;
            if (oldIndex == 0 || newIndex == 0)
            {
                // Never allow Start to move; move back:
                nodesProperty.MoveArrayElement(newIndex, oldIndex);
            }
            else
            {
                // Otherwise adjust node index references in all nodes' connections:
                for (int i = 0; i < nodesProperty.arraySize; i++)
                {
                    var nodeProperty = nodesProperty.GetArrayElementAtIndex(i);
                    var childIndexListProperty = (nodeProperty != null) ? nodeProperty.FindPropertyRelative("m_childIndexList") : null;
                    UpdateChildIndexList(childIndexListProperty, oldIndex, newIndex);
                }
            }
        }

        private void UpdateChildIndexList(SerializedProperty childIndexListProperty, int oldChildIndexValue, int newChildIndexValue)
        {
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList is null.");
            if (childIndexListProperty == null) return;
            for (int i = 0; i < childIndexListProperty.arraySize; i++)
            {
                var childIndexProperty = childIndexListProperty.GetArrayElementAtIndex(i);
                if (childIndexProperty.intValue == oldChildIndexValue)
                {
                    childIndexProperty.intValue = newChildIndexValue;
                }
                else
                {
                    if (childIndexProperty.intValue >= oldChildIndexValue)
                    {
                        childIndexProperty.intValue--;
                    }
                    if (childIndexProperty.intValue >= newChildIndexValue)
                    {
                        childIndexProperty.intValue++;
                    }
                }
            }
        }

    }
}
