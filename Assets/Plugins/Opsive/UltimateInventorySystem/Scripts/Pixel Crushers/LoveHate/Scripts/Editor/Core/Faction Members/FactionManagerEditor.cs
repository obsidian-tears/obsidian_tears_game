// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This script reserves the possibility of a custom editor for FactionManager.
    /// It's simple enough right now that it doesn't merit a custom editor.
    /// </summary>
    [CustomEditor(typeof(FactionManager), true)]
    [CanEditMultipleObjects]
    public class FactionManagerEditor : Editor
    {

        private static bool s_factionDatabaseFoldout;

        private Editor m_factionDatabaseEditor = null;
        private float m_timeToUpdate = 0;
        private const float UpdateFrequency = 0.5f;

        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (Application.isEditor && Time.realtimeSinceStartup > m_timeToUpdate)
            {
                m_timeToUpdate = Time.realtimeSinceStartup + UpdateFrequency;
                Repaint();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Draw faction database at runtime:
            if (!Application.isPlaying) return;
            s_factionDatabaseFoldout = EditorGUILayout.Foldout(s_factionDatabaseFoldout, "Faction Database");
            if (!s_factionDatabaseFoldout) return;
            var factionDatabaseProperty = serializedObject.FindProperty("factionDatabase");
            if (factionDatabaseProperty == null || factionDatabaseProperty.objectReferenceValue == null) return;
            if (m_factionDatabaseEditor == null) m_factionDatabaseEditor = Editor.CreateEditor(factionDatabaseProperty.objectReferenceValue);
            EditorGUI.indentLevel++;
            m_factionDatabaseEditor.OnInspectorGUI();
            EditorGUI.indentLevel--;
        }
    }

}
