// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector GUI when a QuestNode (and not the Quest itself) 
    /// is selected in the Quest Editor window.
    /// </summary>
    [Serializable]
    public class QuestNodeInspectorGUI
    {

        public int nodeIndex { get; private set; }

        private QuestConditionSetInspectorGUI questConditionsInspectorGUI = null;
        private QuestNodeStateInfoInspectorGUI inactiveStateInfoInspectorGUI = null;
        private QuestNodeStateInfoInspectorGUI activeStateInfoInspectorGUI = null;
        private QuestNodeStateInfoInspectorGUI trueStateInfoInspectorGUI = null;

        public QuestNodeInspectorGUI(int nodeIndex)
        {
            this.nodeIndex = nodeIndex;
        }

        public void Draw(SerializedObject serializedObject, SerializedProperty nodeProperty, int nodeIndex)
        {
            if (serializedObject == null || nodeProperty == null) return;
            DrawMainInfo(serializedObject, nodeProperty);
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawStateInfo(serializedObject, nodeProperty, nodeIndex);
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawConnectionInfo(serializedObject, nodeProperty);
        }

        private void DrawMainInfo(SerializedObject serializedObject, SerializedProperty nodeProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(serializedObject, "Quest Machine: Internal error - serializedObject is null.");

            QuestEditorPrefs.nodeMainInfoFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Quest Node Info", "Quest node main info.", QuestEditorPrefs.nodeMainInfoFoldout);
            if (!QuestEditorPrefs.nodeMainInfoFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                var idProperty = nodeProperty.FindPropertyRelative("m_id");
                var internalNameProperty = nodeProperty.FindPropertyRelative("m_internalName");
                var stateProperty = nodeProperty.FindPropertyRelative("m_state");
                var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
                var isOptionalProperty = nodeProperty.FindPropertyRelative("m_isOptional");
                var speakerProperty = nodeProperty.FindPropertyRelative("m_speaker");
                UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id is null.");
                UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName is null.");
                UnityEngine.Assertions.Assert.IsNotNull(stateProperty, "Quest Machine: Internal error - m_state is null.");
                UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType is null.");
                UnityEngine.Assertions.Assert.IsNotNull(isOptionalProperty, "Quest Machine: Internal error - m_isOptional is null.");
                UnityEngine.Assertions.Assert.IsNotNull(isOptionalProperty, "Quest Machine: Internal error - m_speaker is null.");
                if (idProperty == null || internalNameProperty == null || nodeTypeProperty == null ||
                    isOptionalProperty == null || speakerProperty == null || stateProperty == null) return;

                EditorGUILayout.PropertyField(idProperty, true);
                EditorGUILayout.PropertyField(internalNameProperty, true);
                var prevState = stateProperty.enumValueIndex;
                EditorGUILayout.PropertyField(stateProperty, new GUIContent("Current State", "Current state of this node."));
                var newState = stateProperty.enumValueIndex;
                if (Application.isPlaying && newState != prevState)
                {
                    // State changed in editor at runtime. Perform runtime state change with all the associated processing:
                    stateProperty.enumValueIndex = prevState;
                    serializedObject.ApplyModifiedProperties();
                    var questNode = QuestEditorWindow.selectedQuest.nodeList[QuestEditorWindow.selectedNodeListIndex];
                    questNode.SetState((QuestNodeState)newState);
                    serializedObject.Update();
                }
                var nodeType = (QuestNodeType)nodeTypeProperty.enumValueIndex;
                if (nodeType == QuestNodeType.Passthrough || nodeType == QuestNodeType.Condition)
                {
                    EditorGUILayout.PropertyField(isOptionalProperty);
                }
                EditorGUILayout.PropertyField(speakerProperty);
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void DrawStateInfo(SerializedObject serializedObject, SerializedProperty nodeProperty, int nodeIndex)
        {
            UnityEngine.Assertions.Assert.IsNotNull(serializedObject, "Quest Machine: Internal error - serializedObject is null.");
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - nodeProperty is null.");

            //if (GUILayout.Button(new GUIContent("States", "Settings specific to each state that the quest node can be in."), GUI.skin.GetStyle(QuestEditorStyles.CollapsibleHeaderButtonStyleName)))
            //{
            //    QuestEditorPrefs.ToggleStateInfoFoldout(nodeIndex);
            //}
            //if (!QuestEditorPrefs.GetStateInfoFoldout(nodeIndex)) return;

            var foldout = QuestEditorPrefs.GetStateInfoFoldout(nodeIndex);
            var newFoldout = QuestEditorUtility.EditorGUILayoutFoldout("States", "Settings specific to each state that the quest node can be in.", foldout);
            if (newFoldout != foldout)
            {
                QuestEditorPrefs.ToggleStateInfoFoldout(nodeIndex);
            }
            if (!newFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("This section contains information specific to each state that the node can be in.", MessageType.None);

                var stateInfoListProperty = nodeProperty.FindPropertyRelative("m_stateInfoList");
                if (stateInfoListProperty.arraySize == 0) stateInfoListProperty.arraySize = System.Enum.GetNames(typeof(QuestNodeState)).Length;

                if (inactiveStateInfoInspectorGUI == null) inactiveStateInfoInspectorGUI = new QuestNodeStateInfoInspectorGUI();
                if (activeStateInfoInspectorGUI == null) activeStateInfoInspectorGUI = new QuestNodeStateInfoInspectorGUI();
                if (trueStateInfoInspectorGUI == null) trueStateInfoInspectorGUI = new QuestNodeStateInfoInspectorGUI();
                inactiveStateInfoInspectorGUI.Draw(serializedObject, stateInfoListProperty.GetArrayElementAtIndex((int)QuestNodeState.Inactive), nodeIndex, QuestNodeState.Inactive);
                activeStateInfoInspectorGUI.Draw(serializedObject, stateInfoListProperty.GetArrayElementAtIndex((int)QuestNodeState.Active), nodeIndex, QuestNodeState.Active);
                trueStateInfoInspectorGUI.Draw(serializedObject, stateInfoListProperty.GetArrayElementAtIndex((int)QuestNodeState.True), nodeIndex, QuestNodeState.True);
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void DrawConnectionInfo(SerializedObject serializedObject, SerializedProperty nodeProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(serializedObject, "Quest Machine: Internal error - serializedObject is null.");
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - nodeProperty is null.");

            // Only applicable to Condition nodes:
            var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
            UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType is null.");
            if (nodeTypeProperty == null) return;
            var nodeType = (QuestNodeType)nodeTypeProperty.enumValueIndex;
            if (nodeType != QuestNodeType.Condition) return;

            QuestEditorPrefs.trueConnectionFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Conditions", "Conditions that must be true for the node to become true.", QuestEditorPrefs.trueConnectionFoldout);
            if (!QuestEditorPrefs.trueConnectionFoldout) return;

            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("The conditions below must be true for the node to become true.", MessageType.None);
                if (questConditionsInspectorGUI == null) questConditionsInspectorGUI = new QuestConditionSetInspectorGUI();
                questConditionsInspectorGUI.Draw(nodeProperty.FindPropertyRelative("m_conditionSet"));
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

    }
}
