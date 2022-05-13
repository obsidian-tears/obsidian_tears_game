//// Copyright (c) Pixel Crushers. All rights reserved.

//using UnityEngine;
//using UnityEditor;

//namespace PixelCrushers.QuestMachine
//{

//    /// <summary>
//    /// Custom inspector for BasicUIContent.
//    /// </summary>
//    [CustomEditor(typeof(BasicUIContent), true)]
//    public class BasicUIContentEditor : Editor
//    {

//        private SerializedProperty m_icon;
//        private SerializedProperty m_headingText;
//        private SerializedProperty m_bodyText;

//        private void OnEnable()
//        {
//            m_icon = serializedObject.FindProperty("m_icon");
//            m_headingText = serializedObject.FindProperty("m_headingText");
//            m_bodyText = serializedObject.FindProperty("m_bodyText");
//        }

//        public override void OnInspectorGUI()
//        {
//            serializedObject.Update();
//            //QuestEditorPrefs.questGiverDialogueContentFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Dialogue Content", "Quest giver-specific dialogue content.", QuestEditorPrefs.questGiverDialogueContentFoldout);
//            //if (!QuestEditorPrefs.questGiverDialogueContentFoldout) return;

//            try
//            {
//                QuestEditorUtility.EditorGUILayoutBeginGroup();
//                EditorGUILayout.LabelField("yh");
//                if (m_icon != null) EditorGUILayout.PropertyField(m_icon, true);
//                if (m_headingText != null) EditorGUILayout.PropertyField(m_headingText, true);
//                if (m_bodyText != null) EditorGUILayout.PropertyField(m_bodyText, true);
//            }
//            finally
//            {
//                QuestEditorUtility.EditorGUILayoutEndGroup();
//                serializedObject.ApplyModifiedProperties();
//            }
//        }

//    }

//}
