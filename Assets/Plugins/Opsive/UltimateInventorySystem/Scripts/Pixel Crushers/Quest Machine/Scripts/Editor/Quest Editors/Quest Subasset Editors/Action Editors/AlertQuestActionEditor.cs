// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for AlertQuestAction assets.
    /// </summary>
    [CustomEditor(typeof(AlertQuestAction), true)]
    public class AlertQuestActionEditor : QuestSubassetEditor
    {

        private QuestContentListInspectorGUI contentListDrawer { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            contentListDrawer = new QuestContentListInspectorGUI(new GUIContent("Alert Text", "This UI content is shown in the alert UI when this state is enabled."), QuestContentCategory.Alert);
        }

        protected override void Draw()
        {
            if (serializedObject == null) return;
            EditorGUILayout.HelpBox("This action shows content in the alert UI. You can use it to alert the player of quest state changes.", MessageType.None);
            var contentListProperty = serializedObject.FindProperty("m_contentList");
            UnityEngine.Assertions.Assert.IsNotNull(contentListProperty, "Quest Machine: Internal error - m_contentList is null.");
            if (contentListProperty == null) return;
            contentListDrawer.Draw(serializedObject, contentListProperty, true);
        }

    }
}
