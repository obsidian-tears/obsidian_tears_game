// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for ParentQuestCondition assets.
    /// </summary>
    [CustomEditor(typeof(ParentQuestCondition), true)]
    public class ParentQuestConditionEditor : QuestSubassetEditor
    {

        protected override void Draw()
        {
            if (serializedObject == null) return;
            var parentCountModeProperty = serializedObject.FindProperty("m_parentCountMode");
            var minParentCountProperty = serializedObject.FindProperty("m_minParentCount");
            UnityEngine.Assertions.Assert.IsNotNull(parentCountModeProperty, "Quest Machine: Internal error - m_parentCountMode is null.");
            UnityEngine.Assertions.Assert.IsNotNull(minParentCountProperty, "Quest Machine: Internal error - m_minParentCount is null.");
            if (parentCountModeProperty == null || minParentCountProperty == null)
            {
                Debug.LogError("Quest Machine: Internal error in ParentQuestConditionEditor. Please contact the developer.");
                return;
            }
            EditorGUILayout.HelpBox("This condition requires a specified number of parent nodes to be true. Set Parent Count Mode to specify how many parent nodes must be true.", MessageType.None);
            EditorGUILayout.PropertyField(parentCountModeProperty);
            if (parentCountModeProperty.enumValueIndex == (int)ConditionCountMode.Min)
            {
                EditorGUILayout.PropertyField(minParentCountProperty);
            }
        }

    }
}
