// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a ConditionSet.
    /// </summary>
    public class QuestConditionSetInspectorGUI
    {

        private ReorderableList m_list = null;
        private QuestCondition m_selectedCondition = null;
        private Editor m_conditionEditor = null;

        public void Draw(SerializedProperty conditionSetProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(conditionSetProperty, "Quest Machine: Internal error - conditionSet is null in QuestConditionSetInspectorGUI.");
            if (conditionSetProperty == null) return;

            var countModeProperty = conditionSetProperty.FindPropertyRelative("m_conditionCountMode");
            UnityEngine.Assertions.Assert.IsNotNull(countModeProperty, "Quest Machine: Internal error - m_conditionCountMode is null in QuestConditionSetInspectorGUI.");
            if (countModeProperty == null) return;

            EditorGUILayout.PropertyField(countModeProperty);
            if (countModeProperty.enumValueIndex == (int)ConditionCountMode.Min)
            {
                var minCountProperty = conditionSetProperty.FindPropertyRelative("m_minConditionCount");
                UnityEngine.Assertions.Assert.IsNotNull(minCountProperty, "Quest Machine: Internal error - m_minConditionCount is null in QuestConditionSetInspectorGUI.");
                if (minCountProperty == null) return;
                EditorGUILayout.PropertyField(minCountProperty);
            }

            if (m_list == null) SetupReorderableList(conditionSetProperty);
            if (m_list != null) m_list.DoLayoutList();

            DrawSelectedCondition();
        }

        private void SetupReorderableList(SerializedProperty conditionSetProperty)
        {
            if (conditionSetProperty == null) return;
            var conditionListProperty = conditionSetProperty.FindPropertyRelative("m_conditionList");
            UnityEngine.Assertions.Assert.IsNotNull(conditionListProperty, "Quest Machine: Internal error - m_conditionList is null in QuestConditionSetInspectorGUI.");
            if (conditionListProperty == null) return;
            m_list = new ReorderableList(QuestEditorWindow.selectedQuestSerializedObject, conditionListProperty, true, true, true, true);
            m_list.drawHeaderCallback = OnDrawHeader;
            m_list.drawElementCallback = OnDrawElement;
            m_list.onSelectCallback = OnSelectElement;
            m_list.onRemoveCallback = OnRemoveElement;
            m_list.onAddDropdownCallback = OnAddDropdown;
            m_selectedCondition = null;
            m_conditionEditor = null;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Conditions");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < m_list.serializedProperty.arraySize)) return;
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;
            var condition = element.objectReferenceValue as QuestCondition;
            if (condition == null) return;
            EditorGUI.LabelField(rect, condition.GetEditorName());
        }

        private void OnSelectElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            m_selectedCondition = isIndexValid ? (list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestCondition) : null;
            m_conditionEditor = null;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            if (!isIndexValid) return;
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            var condition = list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestCondition;
            QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull(list);
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            AssetUtility.DeleteFromAsset(condition, QuestEditorWindow.selectedQuest);
            OnSelectElement(list);
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var subtypes = QuestEditorUtility.GetSubtypes<QuestCondition>();
            subtypes.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
            var menu = new GenericMenu();
            for (int i = 0; i < subtypes.Count; i++)
            {
                var subtype = subtypes[i];
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(subtype.Name).Replace("Quest Condition", string.Empty)), false, OnAddQuestConditionType, subtype);
            }
            menu.ShowAsContext();
        }

        private void OnAddQuestConditionType(object data)
        {
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            var type = data as Type;
            var condition = ScriptableObjectUtility.CreateScriptableObject(type);
            condition.name = type.Name;
            m_selectedCondition = condition as QuestCondition;
            m_conditionEditor = null;
            if (QuestEditorWindow.selectedQuest != null)
            {
                AssetUtility.AddToAsset(condition, QuestEditorWindow.selectedQuest);
                m_selectedCondition.SetRuntimeReferences(QuestEditorWindow.selectedQuest, null);
            }
            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            try
            {
                m_list.serializedProperty.arraySize++;
                m_list.index = m_list.serializedProperty.arraySize - 1;
                m_list.serializedProperty.GetArrayElementAtIndex(m_list.serializedProperty.arraySize - 1).objectReferenceValue = condition;
                m_list.serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (System.NullReferenceException e)
            {
#if UNITY_EDITOR
                Debug.LogError("Unity's AssetDatabase couldn't add the condition subasset to the quest. The project's AssetDatabase may contain a corrupt asset. Exception: " + e.Message);
#endif
            }
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            //AssetDatabase.SaveAssets();
        }

        private void DrawSelectedCondition()
        {
            if (m_selectedCondition == null) return;
            if (m_conditionEditor == null) m_conditionEditor = Editor.CreateEditor(m_selectedCondition);
            QuestEditorUtility.EditorGUILayoutBeginIndent();
            m_conditionEditor.OnInspectorGUI();
            QuestEditorUtility.EditorGUILayoutEndIndent();
        }

    }
}
