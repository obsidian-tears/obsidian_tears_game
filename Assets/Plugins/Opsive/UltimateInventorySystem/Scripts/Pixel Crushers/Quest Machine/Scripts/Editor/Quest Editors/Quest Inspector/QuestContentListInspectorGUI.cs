// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    public class QuestContentListInspectorGUI
    {

        private GUIContent m_guiContent = null;
        private ReorderableList m_list = null;
        private QuestContent m_selectedQuestContent = null;
        private SerializedObject m_serializedObject = null;
        private bool m_isAsset = false;
        private Editor m_contentEditor = null;
        private bool m_needToClearFocus = false;

        public QuestContentListInspectorGUI(GUIContent guiContent, QuestContentCategory category)
        {
            m_guiContent = guiContent;
        }

        public void Draw(SerializedObject serializedObject, SerializedProperty contentListProperty, bool isAsset)
        {
            if (contentListProperty == null || !contentListProperty.isArray) return;
            if (m_needToClearFocus)
            {
                GUIUtility.keyboardControl = 0;
                m_needToClearFocus = false;
            }
            m_serializedObject = serializedObject;
            m_isAsset = isAsset;
            if (m_list == null) SetupReorderableList(serializedObject, contentListProperty);
            if (m_list != null) m_list.DoLayoutList();
            DrawSelectedContent();
        }

        private void SetupReorderableList(SerializedObject serializedObject, SerializedProperty contentListProperty)
        {
            if (contentListProperty == null) return;
            m_list = new ReorderableList(serializedObject, contentListProperty, true, true, true, true);
            m_list.drawHeaderCallback = OnDrawHeader;
            m_list.drawElementCallback = OnDrawElement;
            m_list.onSelectCallback = OnSelectElement;
            m_list.onRemoveCallback = OnRemoveElement;
            m_list.onAddDropdownCallback = OnAddDropdown;
            m_selectedQuestContent = null;
            m_contentEditor = null;
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
            var content = element.objectReferenceValue as QuestContent;
            if (content == null) return;
            EditorGUI.LabelField(rect, content.GetEditorName());
        }

        private void OnSelectElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            m_selectedQuestContent = isIndexValid ? (list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestContent) : null;
            m_contentEditor = null;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            if (!isIndexValid) return;
            var content = list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue as QuestContent;
            QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull(list);
            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            if (content is ButtonQuestContent) RemoveSubassets(content as ButtonQuestContent);
            AssetUtility.DeleteFromAsset(content, QuestEditorWindow.selectedQuest);
            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            OnSelectElement(list);
        }

        private void RemoveSubassets(ButtonQuestContent buttonQuestContent)
        {
            if (buttonQuestContent == null || buttonQuestContent.actionList == null) return;
            for (int i = 0; i < buttonQuestContent.actionList.Count; i++)
            {
                var content = buttonQuestContent.actionList[i];
                AssetUtility.DeleteFromAsset(content, QuestEditorWindow.selectedQuest);
            }
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var subtypes = QuestEditorUtility.GetSubtypes<QuestContent>();
            subtypes.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));
            var menu = new GenericMenu();
            for (int i = 0; i < subtypes.Count; i++)
            {
                var subtype = subtypes[i];
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(subtype.Name).Replace("Quest Content", string.Empty)), false, OnAddQuestContentType, subtype);
            }
            menu.ShowAsContext();
        }

        private void OnAddQuestContentType(object data)
        {
            var isSelectedQuest = m_isAsset;
            if (isSelectedQuest)
            {
                QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            }
            var type = data as Type;
            var content = ScriptableObjectUtility.CreateScriptableObject(type);
            content.name = type.Name;
            m_selectedQuestContent = content as QuestContent;
            m_contentEditor = null;
            if (isSelectedQuest)
            {
                AssetUtility.AddToAsset(content, QuestEditorWindow.selectedQuestSerializedObject.targetObject);
                QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            }
            try
            {
                m_list.serializedProperty.arraySize++;
                m_list.index = m_list.serializedProperty.arraySize - 1;
                m_list.serializedProperty.GetArrayElementAtIndex(m_list.serializedProperty.arraySize - 1).objectReferenceValue = content;
                m_list.serializedProperty.serializedObject.ApplyModifiedProperties();
                m_serializedObject.ApplyModifiedProperties();
            }
            catch (System.NullReferenceException e)
            {
#if UNITY_EDITOR
                Debug.LogError("Unity's AssetDatabase couldn't add the content subasset to the quest. The project's AssetDatabase may contain a corrupt asset. Exception: " + e.Message);
#endif
            }
            if (isSelectedQuest)
            {
                QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
                //AssetDatabase.SaveAssets();
            }
            m_needToClearFocus = true;
        }

        private void DrawSelectedContent()
        {
            if (m_selectedQuestContent == null) return;
            if (m_contentEditor == null) m_contentEditor = Editor.CreateEditor(m_selectedQuestContent);
            QuestEditorUtility.EditorGUILayoutBeginIndent();
            m_contentEditor.OnInspectorGUI();
            QuestEditorUtility.EditorGUILayoutEndIndent();
        }

    }
}
