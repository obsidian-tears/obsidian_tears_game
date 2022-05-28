// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for QuestGiver.
    /// </summary>
    [CustomEditor(typeof(QuestMachineID), true)]
    public class QuestMachineIDEditor : Editor
    {
        protected bool hasOtherID;

        protected virtual void OnEnable()
        {
            var questMachineID = target as QuestMachineID;
            hasOtherID = (questMachineID.GetComponent<QuestEntity>() != null) || (questMachineID.GetComponent<IdentifiableQuestListContainer>() != null);

        }

        public override void OnInspectorGUI()
        {
            if (hasOtherID)
            {
                EditorGUILayout.HelpBox("This GameObject has another component that provides an ID and Display Name. Quest Machine will ignore this component.", MessageType.Warning);
            }
            base.OnInspectorGUI();
        }
    }
}
