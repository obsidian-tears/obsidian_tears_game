// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom drawer that allows assignment of the C# interface IQuestJournalUI.
    /// </summary>
    [CustomPropertyDrawer(typeof(IQuestJournalUIInspectorFieldAttribute), true)]
    public class IQuestJournalUIInspectorFieldAttributeDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var newValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), true);
            TryAssignNewValue(property, newValue);
        }

        public static void DoLayout(SerializedProperty property, GUIContent label)
        {
            if (property == null) return;
            var newValue = EditorGUILayout.ObjectField(label, property.objectReferenceValue, typeof(UnityEngine.Object), true);
            TryAssignNewValue(property, newValue);
        }

        protected static void TryAssignNewValue(SerializedProperty property, UnityEngine.Object newValue)
        {
            if (newValue != property.objectReferenceValue)
            {
                if (newValue == null)
                {
                    property.objectReferenceValue = null;
                }
                else
                {
                    IQuestJournalUI newUI = null;
                    if (newValue is GameObject)
                    {
                        newUI = (newValue as GameObject).GetComponent(typeof(IQuestJournalUI)) as IQuestJournalUI;
                    }
                    else if (newValue is Component)
                    {
                        var go = (newValue as Component).gameObject;
                        newUI = go.GetComponent(typeof(IQuestJournalUI)) as IQuestJournalUI;
                    }
                    if (newUI != null)
                    {
                        property.objectReferenceValue = newUI as Component;
                    }
                }
            }
        }

    }
}
