// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for ButtonQuestContent assets.
    /// </summary>
    [CustomEditor(typeof(ButtonQuestContent), true)]
    public class ButtonQuestUIContentEditor : IconQuestUIContentEditor
    {

        private QuestActionListInspectorGUI m_actionListDrawer = null;
        private GUIContent m_groupToggleGUIContent = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (m_actionListDrawer == null) m_actionListDrawer = new QuestActionListInspectorGUI(new GUIContent("Actions", "Actions that run when this button is clicked."));
            m_groupToggleGUIContent = new GUIContent("Group Selection", "This button belongs to a group. When one button in the group is clicked, all buttons are deactivated. Typically used for reward selection.");
        }

        protected override void Draw()
        {
            base.Draw();
            var groupNumberProperty = serializedObject.FindProperty("m_groupNumber");
            var actionListProperty = serializedObject.FindProperty("m_actionList");
            UnityEngine.Assertions.Assert.IsNotNull(groupNumberProperty, "Quest Machine: Internal error - m_groupNumber is null.");
            UnityEngine.Assertions.Assert.IsNotNull(actionListProperty, "Quest Machine: Internal error - m_actionList is null.");
            if (groupNumberProperty == null || actionListProperty == null) return;
            bool useGroup = groupNumberProperty.intValue != ButtonQuestContent.NoGroup;
            EditorGUI.BeginChangeCheck();
            useGroup = EditorGUILayout.Toggle(m_groupToggleGUIContent, useGroup);
            if (EditorGUI.EndChangeCheck())
            {
                groupNumberProperty.intValue = useGroup ? 0 : ButtonQuestContent.NoGroup;
            }
            if (useGroup)
            {
                EditorGUILayout.PropertyField(groupNumberProperty);
            }
            m_actionListDrawer.Draw(actionListProperty);
        }

    }
}
