// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(Drive), true)]
    public class DriveEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Drives define quest givers' personalities, which in turn influence the quests they generate. " +
                "The quest giver associates a value with each drive that specifies how important the drive is. For example, if you define a drive named " +
                "Protection, a compassionate healer might have a high value for this drive, whereas a selfish murderer would have a low value.\n\n" +
                "This drive asset just defines a new type of drive. You can specify drive values for each quest giver by inspecting the quest giver's " +
                "entity type asset.", MessageType.None);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_description"));
            serializedObject.ApplyModifiedProperties();
        }

    }

}