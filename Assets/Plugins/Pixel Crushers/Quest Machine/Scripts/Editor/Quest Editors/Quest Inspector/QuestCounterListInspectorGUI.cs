// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Editor GUI for a list of QuestCounter objects.
    /// </summary>
    public class QuestCounterListInspectorGUI
    {

        public const int InitialMaxCounterValue = 999;

        private ReorderableList m_list = null;
        private SerializedProperty m_selectedCounter = null;

        private static string s_counterClipboard = string.Empty;

        public void Draw(SerializedObject serializedObject, SerializedProperty countersProperty)
        {
            if (countersProperty == null) return;
            if (m_list == null) SetupReorderableList(countersProperty);
            if (m_list != null) m_list.DoLayoutList();
            DrawSelectedUIContent();
        }

        private void SetupReorderableList(SerializedProperty countersProperty)
        {
            if (countersProperty == null) return;
            m_list = new ReorderableList(QuestEditorWindow.selectedQuestSerializedObject, countersProperty, true, true, true, true);
            m_list.drawHeaderCallback = OnDrawHeader;
            m_list.drawElementCallback = OnDrawElement;
            m_list.onSelectCallback = OnSelectElement;
            m_list.onRemoveCallback = OnRemoveElement;
            m_list.onAddCallback = OnAddElement;
            m_selectedCounter = null;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Counters", "Counters used in this quest."));
            if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
            {
                var clickRect = rect;
                if (m_list.serializedProperty.arraySize == 0)
                { // If empty, allow click in 'List is Empty' area:
                    clickRect.height += 1.5f * EditorGUIUtility.singleLineHeight;
                }
                if (clickRect.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    ShowContextMenu(-1);
                }
            }
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < m_list.serializedProperty.arraySize)) return;
            var element = m_list.serializedProperty.GetArrayElementAtIndex(index);
            if (element == null) return;
            var currentValueProperty = element.FindPropertyRelative("m_currentValue");
            var currentValue = (currentValueProperty != null) ? currentValueProperty.intValue : 0;
            EditorGUI.LabelField(rect, StringFieldDrawer.GetStringFieldValue(element.FindPropertyRelative("m_name")) + ": " + currentValue);
            if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                ShowContextMenu(index);
            }
        }

        private void ShowContextMenu(int index)
        {
            var menu = new GenericMenu();
            if (index == -1)
            {
                menu.AddDisabledItem(new GUIContent("Copy Counter"));
            }
            else
            {
                menu.AddItem(new GUIContent("Copy Counter"), false, () =>
                {
                    s_counterClipboard = JsonUtility.ToJson(QuestEditorWindow.selectedQuest.counterList[index]);
                });
            }
            if (string.IsNullOrEmpty(s_counterClipboard))
            {
                menu.AddDisabledItem(new GUIContent("Paste Counter As New"));                
            }
            else
            {
                menu.AddItem(new GUIContent("Paste Counter As New"), false, () => 
                {
                    var newCounter = JsonUtility.FromJson<QuestCounter>(s_counterClipboard);
                    if (newCounter != null)
                    {
                        QuestEditorWindow.selectedQuest.counterList.Add(newCounter);
                    }
                });
            }
            menu.ShowAsContext();
        }

        private void OnSelectElement(ReorderableList list)
        {
            var isIndexValid = (0 <= list.index && list.index < list.count);
            m_selectedCounter = isIndexValid ? list.serializedProperty.GetArrayElementAtIndex(list.index) : null;
        }

        private void OnRemoveElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            OnSelectElement(list);
        }

        private void OnAddElement(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            var newCounterProperty = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (newCounterProperty != null)
            {
                var counterNameProperty = newCounterProperty.FindPropertyRelative("m_name");
                if (counterNameProperty != null)
                {
                    StringFieldDrawer.SetStringFieldValue(counterNameProperty, string.Empty);
                }
                var maxValueProperty = newCounterProperty.FindPropertyRelative("m_maxValue");
                if (maxValueProperty != null)
                {
                    maxValueProperty.intValue = InitialMaxCounterValue;
                }
                var messageEventListProperty = newCounterProperty.FindPropertyRelative("m_messageEventList");
                if (messageEventListProperty != null)
                {
                    messageEventListProperty.ClearArray();
                }
            }
            OnSelectElement(list);
        }

        private void DrawSelectedUIContent()
        {
            if (m_selectedCounter == null) return;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_selectedCounter);
            EditorGUI.indentLevel--;
        }

    }
}
