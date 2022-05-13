// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a list of QuestAction objects, which is typically a part
    /// of a QuestStateInfo object.
    /// </summary>
    public class QuestActionListInspectorGUI
    {

        private GUIContent m_guiContent = null;
        private ReorderableList m_list = null;
        private QuestAction m_selectedAction = null;
        private Editor m_actionEditor = null;

        public QuestActionListInspectorGUI(GUIContent guiContent)
        {
            m_guiContent = guiContent;
        }

        public void Draw(SerializedProperty actionsProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(actionsProperty, "Quest Machine: Internal error - actions list is null in QuestActionListInspectorGUI.");
            if (actionsProperty == null) return;
            if (m_list == null) SetupReorderableList(actionsProperty);
            if (m_list != null) m_list.DoLayoutList();
            DrawSelectedAction();
        }

        private void SetupReorderableList(SerializedProperty actionsProperty)
        {
            if (actionsProperty == null) return;
            m_list = new ReorderableList(QuestEditorWindow.selectedQuestSerializedObject, actionsProperty, true, true, true, true);
            m_list.drawHeaderCallback = OnDrawHeader;
            m_list.drawElementCallback = OnDrawElement;
            m_list.onSelectCallback = OnSelectElement;
            m_list.onRemoveCallback = OnRemoveElement;
            m_list.onAddDropdownCallback = OnAddDropdown;
            m_selectedAction = null;
            m_actionEditor = null;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, m_guiContent);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < m_list.serializedProperty.arraySize)) return;
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;
            var questAction = element.objectReferenceValue as QuestAction;
            if (questAction == null) return;
            EditorGUI.LabelField(rect, questAction.GetEditorName());
        }

        private void OnSelectElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            m_selectedAction = isIndexValid ? (list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestAction) : null;
            m_actionEditor = null;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            if (!isIndexValid) return;
            var action = list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestAction;
            QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull(list);
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            if (action is AlertQuestAction) RemoveSubassets(action as AlertQuestAction);
            AssetUtility.DeleteFromAsset(action, QuestEditorWindow.selectedQuest);
            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            OnSelectElement(list);
        }

        private void RemoveSubassets(AlertQuestAction alertQuestAction)
        {
            if (alertQuestAction == null || alertQuestAction.contentList == null) return;
            for (int i = 0; i < alertQuestAction.contentList.Count; i++)
            {
                var content = alertQuestAction.contentList[i];
                AssetUtility.DeleteFromAsset(content, QuestEditorWindow.selectedQuest);
            }
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var subtypes = QuestEditorUtility.GetSubtypes<QuestAction>();
            subtypes.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
            var menu = new GenericMenu();
            for (int i = 0; i < subtypes.Count; i++)
            {
                var subtype = subtypes[i];
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(subtype.Name).Replace("Quest Action", string.Empty)), false, OnAddQuestActionType, subtype);
            }
            menu.ShowAsContext();
        }

        private void OnAddQuestActionType(object data)
        {
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            var type = data as Type;
            var questAction = ScriptableObjectUtility.CreateScriptableObject(type);
            questAction.name = type.Name;
            m_selectedAction = questAction as QuestAction;
            m_actionEditor = null;
            if (QuestEditorWindow.selectedQuest != null)
            {
                AssetUtility.AddToAsset(questAction, QuestEditorWindow.selectedQuest);
                m_selectedAction.SetRuntimeReferences(QuestEditorWindow.selectedQuest, null);
            }
            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            try
            {
                m_list.serializedProperty.arraySize++;
                m_list.index = m_list.serializedProperty.arraySize - 1;
                m_list.serializedProperty.GetArrayElementAtIndex(m_list.serializedProperty.arraySize - 1).objectReferenceValue = questAction;
                m_list.serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (System.NullReferenceException e)
            {
#if UNITY_EDITOR
                Debug.LogError("Unity's AssetDatabase couldn't add the action subasset to the quest. The project's AssetDatabase may contain a corrupt asset. Exception: " + e.Message);
#endif
            }
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            //AssetDatabase.SaveAssets();
        }

        private void DrawSelectedAction()
        {
            if (m_selectedAction == null) return;
            if (m_actionEditor == null) m_actionEditor = Editor.CreateEditor(m_selectedAction);
            QuestEditorUtility.EditorGUILayoutBeginIndent();
            m_actionEditor.OnInspectorGUI();
            QuestEditorUtility.EditorGUILayoutEndIndent();
        }

    }
}
