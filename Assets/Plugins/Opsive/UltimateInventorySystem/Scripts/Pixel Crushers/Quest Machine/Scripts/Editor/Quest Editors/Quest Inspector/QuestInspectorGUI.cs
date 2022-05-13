// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Performs the main GUI work for the custom Quest inspector, QuestEditor.
    /// </summary>
    [Serializable]
    public class QuestInspectorGUI
    {

        private QuestMainInspectorGUI mainInspectorGUI { get; set; }
        private QuestNodeInspectorGUI nodeInspectorGUI { get; set; }
        private SerializedObject currentSerializedObject { get; set; }

        public QuestInspectorGUI()
        {
            mainInspectorGUI = null;
            nodeInspectorGUI = null;
            currentSerializedObject = null;
        }

        public void Draw(SerializedObject serializedObject)
        {
            if (serializedObject == null) return;
            if (serializedObject != currentSerializedObject)
            {
                currentSerializedObject = serializedObject;
                mainInspectorGUI = null;
                nodeInspectorGUI = null;
            }
            var nodeListProperty = serializedObject.FindProperty("m_nodeList");
            if (nodeListProperty == null) return;
            var isNodeSelected = (0 <= QuestEditorWindow.selectedNodeListIndex && QuestEditorWindow.selectedNodeListIndex < nodeListProperty.arraySize);
            if (!isNodeSelected)
            {
                if (mainInspectorGUI == null) mainInspectorGUI = new QuestMainInspectorGUI();
                mainInspectorGUI.Draw(serializedObject);
            }
            else
            {
                if (nodeInspectorGUI == null || nodeInspectorGUI.nodeIndex != QuestEditorWindow.selectedNodeListIndex)
                {
                    nodeInspectorGUI = new QuestNodeInspectorGUI(QuestEditorWindow.selectedNodeListIndex);
                }
                var nodeProperty = nodeListProperty.GetArrayElementAtIndex(QuestEditorWindow.selectedNodeListIndex);
                nodeInspectorGUI.Draw(serializedObject, nodeProperty, QuestEditorWindow.selectedNodeListIndex);
            }
        }

    }
}
