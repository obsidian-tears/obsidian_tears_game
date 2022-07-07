// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(DomainType), true)]
    public class DomainTypeEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Domain types are the abstract definitions of domains, which are locations where entities exist. " +
                "Quest givers generate plans based on domain types, since not all domains may be present in the current scene. " +
                "To assign this domain type to a domain, inspect the domain in its scene.", MessageType.None);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_displayName"));
            serializedObject.ApplyModifiedProperties();
        }

    }

}