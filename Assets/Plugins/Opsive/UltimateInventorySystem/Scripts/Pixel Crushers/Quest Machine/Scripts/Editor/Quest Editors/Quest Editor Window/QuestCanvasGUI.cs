// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// This class draws the GUI canvas for a Quest in the Quest Editor window.
    /// </summary>
    public class QuestCanvasGUI
    {

        #region Private Fields

        public const float CanvasWidth = 10000;
        public const float CanvasHeight = 10000;

        private float m_zoom = 1;
        private Rect m_zoomArea;
        private Rect m_nodeEditorVisibleRect;
        private Vector2 m_mousePos;
        private bool m_connecting = false;
        private bool m_draggingNodes = false;
        private bool m_draggingCanvas = false;
        private bool m_lassoing = false;
        private Vector2 m_lassoStart, m_lassoEnd;
        private Rect m_lassoRect;

        private Quest m_quest;
        private SerializedObject m_questSerializedObject;
        private SerializedProperty m_nodeListProperty;
        private QuestEditorWizard m_wizard = null;
        private string jsonFilename;

        public SerializedObject questSerializedObject { get { return m_questSerializedObject; } }

        public Vector2 canvasScrollPosition
        {
            get { return QuestEditorWindow.instance.canvasScrollPosition; }
            set { QuestEditorWindow.instance.canvasScrollPosition = value; }
        }

        #endregion

        #region Setup

        public void AssignQuest(Quest quest)
        {
            if (quest != m_quest && m_quest != null) canvasScrollPosition = Vector2.zero;
            m_quest = quest;
            m_questSerializedObject = (quest != null) ? new SerializedObject(quest) : null;
            m_nodeListProperty = (m_questSerializedObject != null) ? m_questSerializedObject.FindProperty("m_nodeList") : null;
        }

        public bool IsQuestAssigned()
        {
            return m_questSerializedObject != null && m_questSerializedObject.targetObject != null;
        }

        #endregion

        #region High Level Drawing

        public virtual void Draw(Rect position)
        {
            if (!AreReferencesValid()) return;
            m_questSerializedObject.Update();
            DrawQuestTitle();
            if (m_wizard != null)
            {
                DrawWizard();
            }
            else
            {
                DrawQuestCanvas(position);
            }
            m_questSerializedObject.ApplyModifiedProperties();
            DrawGearMenu(position);
        }

        protected bool AreReferencesValid()
        {
            UnityEngine.Assertions.Assert.IsNotNull(m_questSerializedObject, "Quest Machine: Internal error - m_questSerializedObject is null.");
            if (m_questSerializedObject == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(m_questSerializedObject.targetObject, "Quest Machine: Internal error - m_questSerializedObject target object is null.");
            if (m_questSerializedObject.targetObject == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(m_nodeListProperty, "Quest Machine: Internal error - m_nodeList property is null.");
            if (m_nodeListProperty == null) return false;
            UnityEngine.Assertions.Assert.IsNotNull(QuestEditorWindow.instance, "Quest Machine: Internal error - QuestEditorWindow.instance is null.");
            if (QuestEditorWindow.instance == null) return false;
            return true;
        }

        private void DrawQuestTitle()
        {
            var titleProperty = m_questSerializedObject.FindProperty("m_title");
            UnityEngine.Assertions.Assert.IsNotNull(titleProperty, "Quest Machine: Internal error - m_title property is null.");
            if (titleProperty == null) return;
            var displayName = StringFieldDrawer.GetStringFieldValue(titleProperty);
            if (string.IsNullOrEmpty(displayName))
            {
                var idProperty = m_questSerializedObject.FindProperty("m_id");
                if (idProperty != null) displayName = StringFieldDrawer.GetStringFieldValue(idProperty);
                if (string.IsNullOrEmpty(displayName)) displayName = m_questSerializedObject.targetObject.name;
            }
            if (m_quest.isInstance) displayName += " (runtime: " + m_quest.GetState() + ")";
            EditorGUILayout.LabelField(displayName, QuestEditorStyles.questNameGUIStyle);
        }

        private void DrawQuestCanvas(Rect position)
        {
            var canvasScrollView = new Vector2(CanvasWidth, CanvasHeight);
            m_zoomArea = new Rect(0, 0, position.width, position.height);
            if (m_zoom > 1)
            {
                m_zoomArea = new Rect(0, 0, position.width + ((m_zoom - 1) * position.width), position.height + ((m_zoom - 1) * position.height));
            }
            m_nodeEditorVisibleRect = new Rect(canvasScrollPosition.x, canvasScrollPosition.y, position.width / m_zoom, position.height / m_zoom);

            // Test: Inset to see cutoff.
            //m_nodeEditorVisibleRect = new Rect(m_nodeEditorVisibleRect.x + 40, m_nodeEditorVisibleRect.y + 40, m_nodeEditorVisibleRect.width - 80, m_nodeEditorVisibleRect.height - 80);

            EditorGUIZoomArea.Begin(m_zoom, m_zoomArea);
            try
            {
                // Hide scrollbars:
                GUIStyle verticalScrollbar = GUI.skin.verticalScrollbar;
                GUIStyle horizontalScrollbar = GUI.skin.horizontalScrollbar;
                GUI.skin.verticalScrollbar = GUIStyle.none;
                GUI.skin.horizontalScrollbar = GUIStyle.none;

                canvasScrollPosition = GUI.BeginScrollView(new Rect(0, 0, (1 / m_zoom) * position.width, (1 / m_zoom) * position.height), canvasScrollPosition, new Rect(0, 0, canvasScrollView.x, canvasScrollView.y), false, false);
                HandleInput(position);
                DrawConnectionArrows();
                DrawNodes();
                DrawLasso();
                GUI.EndScrollView();

                GUI.skin.verticalScrollbar = verticalScrollbar;
                GUI.skin.horizontalScrollbar = horizontalScrollbar;
            }
            finally
            {
                EditorGUIZoomArea.End();
            }
        }

        #endregion

        #region Draw Content

        private void DrawConnectionArrows()
        {
            ValidateStartNode();
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                if (nodeProperty == null) continue;
                var nodeRect = GetCanvasRect(nodeProperty);
                var parentRect = new Rect(nodeRect.xMin, nodeRect.yMax - EditorGUIUtility.singleLineHeight, nodeRect.width, EditorGUIUtility.singleLineHeight);
                var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
                DrawConnectionsToChildren(parentRect, childIndexListProperty);
            }
        }

        private void DrawConnectionsToChildren(Rect sourceRect, SerializedProperty childIndexListProperty)
        {
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null.");
            if (childIndexListProperty == null) return;
            var isSourceRectVisible = sourceRect.Overlaps(m_nodeEditorVisibleRect);
            for (int i = 0; i < childIndexListProperty.arraySize; i++)
            {
                var childIndex = childIndexListProperty.GetArrayElementAtIndex(i).intValue;
                if (0 <= childIndex && childIndex < m_nodeListProperty.arraySize)
                {
                    var destNodeRect = GetCanvasRect(m_nodeListProperty.GetArrayElementAtIndex(childIndex));
                    var isDestRectVisible = destNodeRect.Overlaps(m_nodeEditorVisibleRect);
                    if (isSourceRectVisible || isDestRectVisible)
                    {
                        var destRect = new Rect(destNodeRect.x, destNodeRect.y, destNodeRect.width, EditorGUIUtility.singleLineHeight);
                        DrawNodeCurve(sourceRect, destRect, QuestEditorStyles.ConnectorColor);
                    }
                }
            }
        }

        private void ValidateStartNode()
        {
            if (m_nodeListProperty.arraySize > 0) return;
            m_questSerializedObject.ApplyModifiedProperties();
            var quest = m_questSerializedObject.targetObject as Quest;
            if (quest != null) quest.Initialize();
            m_questSerializedObject.Update();
        }

        private void DrawNodes()
        {
            if (Event.current.type != EventType.Repaint) return;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                DrawNode(i, m_nodeListProperty.GetArrayElementAtIndex(i));
            }
        }

        private void DrawNode(int nodeIndex, SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return;
            var nodeRectProperty = GetCanvasRectProperty(nodeProperty);
            if (nodeRectProperty == null) return;
            if (!nodeRectProperty.rectValue.Overlaps(m_nodeEditorVisibleRect)) return; // Skip if not visible.

            var stateProperty = nodeProperty.FindPropertyRelative("m_state");
            UnityEngine.Assertions.Assert.IsNotNull(stateProperty, "Quest Machine: Internal error - quest node m_state property is null.");
            if (stateProperty == null) return;
            var nodeState = (QuestNodeState)stateProperty.enumValueIndex;
            var prevContentColor = GUI.contentColor;
            var prevBkColor = GUI.backgroundColor;
            switch (nodeState)
            {
                case QuestNodeState.Inactive:
                    // Use default color; don't set GUI.backgroundColor.
                    break;
                case QuestNodeState.Active:
                    GUI.backgroundColor = QuestEditorStyles.ActiveNodeColor;
                    break;
                case QuestNodeState.True:
                    GUI.backgroundColor = QuestEditorStyles.TrueNodeColor;
                    break;
            }
            var isSelected = (nodeIndex == QuestEditorWindow.selectedNodeListIndex) ||
                QuestEditorWindow.selectedNodeListIndices.Contains(nodeIndex);
            DrawNodeBox(nodeProperty, isSelected);
            GUI.contentColor = prevContentColor;
            GUI.backgroundColor = prevBkColor;
        }

        private void DrawNodeBox(SerializedProperty nodeProperty, bool isSelected)
        {
            //UnityEngine.Assertions.Assert.IsTrue(0 <= id && id < m_nodeListProperty.arraySize, "Quest Machine: Internal error - node ID is outside m_nodeList range.");
            if (nodeProperty == null) return;
            var canvasRect = GetCanvasRect(nodeProperty);
            var rect = canvasRect;
            var nodeType = GetNodeType(nodeProperty);
            var text = GetNodeText(nodeProperty);

            const float headerHeight = 15f;
            var barWidth = rect.width - 5;
            var barRect = new Rect(rect.x + 2, rect.y + headerHeight, barWidth, QuestEditorStyles.nodeBarHeight);
            var textRect = new Rect(barRect.x, barRect.y, barRect.width, barRect.height);

            QuestEditorStyles.questNodeWindowGUIStyle.Draw(rect, GUIContent.none, false, isSelected, isSelected, false);
            GUI.Label(barRect, text, QuestEditorStyles.GetNodeBarGUIStyle(nodeType));
            GUI.Label(textRect, text, QuestEditorStyles.nodeTextGUIStyle);

            if (nodeType != QuestNodeType.Start)
            { 
                // Top connector dot.
                GUI.DrawTexture(new Rect(rect.x + (rect.width / 2) - (QuestEditorStyles.connectorImage.width / 2),
                    rect.y - 12 + headerHeight, QuestEditorStyles.connectorImage.width, QuestEditorStyles.connectorImage.height), QuestEditorStyles.connectorImage);
            }
            if (nodeType != QuestNodeType.Success && nodeType != QuestNodeType.Failure)
            { 
                // Bottom connector dot.
                GUI.DrawTexture(new Rect(rect.x + (rect.width / 2) - (QuestEditorStyles.connectorImage.width / 2),
                    rect.y + canvasRect.height - 4 - QuestEditorStyles.connectorImage.height, QuestEditorStyles.connectorImage.width, QuestEditorStyles.connectorImage.height), QuestEditorStyles.connectorImage);

                if (nodeType == QuestNodeType.Passthrough)
                {
                    // Passthrough arrow to distinguish from condition nodes.
                    GUI.DrawTexture(new Rect(rect.x + rect.width - QuestEditorStyles.passthroughImage.width,
                        rect.y + canvasRect.height - QuestEditorStyles.passthroughImage.height, 
                        QuestEditorStyles.passthroughImage.width, QuestEditorStyles.passthroughImage.height), 
                        QuestEditorStyles.passthroughImage);
                }
            }
        }

        private string GetNodeText(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return string.Empty;
            var internalNameProperty = nodeProperty.FindPropertyRelative("m_internalName");
            UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName property is null.");
            if (internalNameProperty == null) return string.Empty;
            var text = StringFieldDrawer.GetStringFieldValue(internalNameProperty);
            if (string.IsNullOrEmpty(text))
            {
                var idProperty = nodeProperty.FindPropertyRelative("m_id");
                UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id property is null.");
                if (idProperty == null) return string.Empty;
                text = StringFieldDrawer.GetStringFieldValue(idProperty);
            }
            return text;
        }

        private QuestNodeType GetNodeType(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return QuestNodeType.Start;
            var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
            UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType property is null.");
            if (nodeTypeProperty == null) return QuestNodeType.Start;
            return (QuestNodeType)nodeTypeProperty.enumValueIndex;
        }

        private SerializedProperty GetCanvasRectProperty(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return null;
            var canvasRectProperty = nodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - m_canvasRect property is null.");
            if (canvasRectProperty == null) return null;
            if (canvasRectProperty.rectValue.width < QuestEditorStyles.nodeWidth)
            {
                var nodeType = GetNodeType(nodeProperty);
                var isShortNode = (nodeType == QuestNodeType.Success || nodeType == QuestNodeType.Failure);
                var nodeHeight = isShortNode ? QuestEditorStyles.shortNodeHeight : QuestEditorStyles.nodeHeight;
                canvasRectProperty.rectValue = new Rect(canvasRectProperty.rectValue.x, canvasRectProperty.rectValue.y, QuestEditorStyles.nodeWidth, nodeHeight);
            }
            return canvasRectProperty;
        }

        private Rect GetCanvasRect(SerializedProperty parentProperty)
        {
            var canvasRectProperty = GetCanvasRectProperty(parentProperty);
            return (canvasRectProperty != null) ? canvasRectProperty.rectValue : new Rect(50, 30, QuestEditorStyles.nodeWidth, QuestEditorStyles.nodeHeight);
        }

        #endregion

        #region Handle Input

        private void HandleInput(Rect position)
        {
            m_mousePos = Event.current.mousePosition;
            var isNodeSelected = QuestEditorWindow.selectedNodeListIndex != -1;
            var mouseDown = Event.current.type == EventType.MouseDown;
            var mouseUp = Event.current.type == EventType.MouseUp;
            var leftMouse = Event.current.button == 0;
            var rightMouse = Event.current.button == 1;
            var middleMouse = Event.current.button == 2;
            var leftMouseDown = mouseDown && leftMouse;
            var leftMouseUp = mouseUp && leftMouse;
            var rightMouseDown = mouseDown && rightMouse;
            //var rightMouseUp = mouseUp && rightMouse;
            var middleMouseDown = mouseDown && middleMouse;
            //var middleMouseUp = mouseUp && middleMouse;
            var alt = Event.current.alt;
            var shift = Event.current.shift;
            var ctrl = Event.current.control;
            var drag = Event.current.type == EventType.MouseDrag && Event.current.delta.magnitude > 0;
            var inputWheel = (Event.current.type == EventType.ScrollWheel && !QuestEditorPrefs.zoomLock);
            var deleteKey = (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete && isNodeSelected);

            if (inputWheel)
            {
                ZoomWithWheel();
                Event.current.Use();
            }
            else if (leftMouseDown && !(alt || ctrl))
            {
                m_connecting = m_draggingCanvas = false;
                var clickedOnNode = ClickOnCanvas(position);
                if (!m_connecting && QuestEditorWindow.selectedNodeListIndex != -1)
                {
                    m_draggingNodes = true;
                }
                if (!clickedOnNode)
                {
                    StartLasso();
                }
            }
            else if (leftMouseUp)
            {
                m_draggingNodes = m_draggingCanvas = false;
                if (m_connecting) EndConnection();
                if (m_lassoing) EndLasso();
            }
            else if (rightMouseDown || (leftMouseDown && ctrl))
            {
                m_connecting = m_draggingNodes = m_draggingCanvas = m_lassoing = false;
                if (IsMouseOnNode()) ClickOnCanvas(position);
                ShowContextMenu();
            }
            else if (middleMouseDown || (leftMouseDown && alt))
            {
                m_draggingCanvas = true;
            }
            else if (deleteKey)
            {
                DeleteNodes(QuestEditorWindow.selectedNodeListIndex);
            }
            else if (drag)
            {
                if (m_draggingNodes) DragNodes();
                else if (m_draggingCanvas) PanWithMouse();
                else if (m_lassoing) UpdateLasso();
            }
            if (m_connecting && QuestEditorWindow.selectedNodeListIndex != -1)
            {
                DrawInProgressConnectorLine();
            }
        }

        private void ZoomWithWheel()
        {
            m_zoom -= Event.current.delta.y / 100f;
        }

        private void PanWithMouse()
        {
            if (Event.current.type == EventType.MouseDrag && Event.current.delta.magnitude > 0)
            {
                canvasScrollPosition -= Event.current.delta;
                canvasScrollPosition = new Vector2(Mathf.Clamp(canvasScrollPosition.x, 0, Mathf.Infinity),
                    Mathf.Clamp(canvasScrollPosition.y, 0, Mathf.Infinity));
                QuestEditorWindow.RepaintNow();
            }
        }

        private bool IsMouseOnNode()
        {
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos)) return true;
            }
            return false;
        }

        private bool ClickOnCanvas(Rect position) // Return true if clicked on node.
        {
            if (GetGearMenuRect(position).Contains(m_mousePos)) return false;
            var clickedOnNode = false;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    clickedOnNode = true;
                    if (Event.current.shift && QuestEditorWindow.selectedNodeListIndices.Count > 0)
                    {
                        if (QuestEditorWindow.selectedNodeListIndices.Contains(i))
                        {
                            QuestEditorWindow.selectedNodeListIndices.Remove(i);
                        }
                        else if (!QuestEditorWindow.selectedNodeListIndices.Contains(i))
                        {
                            QuestEditorWindow.selectedNodeListIndices.Add(i);
                        }
                    }
                    else if (!QuestEditorWindow.selectedNodeListIndices.Contains(i))
                    {
                        QuestEditorWindow.selectedNodeListIndex = i;
                        QuestEditorWindow.selectedNodeListIndices.Clear();
                        QuestEditorWindow.selectedNodeListIndices.Add(i);
                    }
                    if (QuestEditorWindow.selectedNodeListIndices.Count == 1)
                    {
                        var nodeType = GetNodeType(nodeProperty);
                        if (nodeType != QuestNodeType.Success && nodeType != QuestNodeType.Failure)
                        {
                            var connectRect = new Rect(nodeRect.x, nodeRect.y + nodeRect.height - QuestEditorStyles.connectorImage.height - 8, nodeRect.width, QuestEditorStyles.connectorImage.height + 8);
                            if (connectRect.Contains(m_mousePos)) m_connecting = true;
                        }
                    }
                    break;
                }
            }
            if (!clickedOnNode) QuestEditorWindow.selectedNodeListIndex = -1;
            QuestEditorWindow.SetSelectionToQuest();
            QuestEditorWindow.RepaintNow();
            QuestEditorWindow.RepaintCurrentEditorNow();
            return clickedOnNode;
        }

        private void DrawInProgressConnectorLine()
        {
            Rect mouseRect = new Rect(m_mousePos.x, m_mousePos.y, 10, 10);
            var selectedNodeProperty = m_nodeListProperty.GetArrayElementAtIndex(QuestEditorWindow.selectedNodeListIndex);
            var selectedNodeRect = GetCanvasRect(selectedNodeProperty);
            var sourceRect = new Rect(selectedNodeRect.x, selectedNodeRect.y + selectedNodeRect.height - EditorGUIUtility.singleLineHeight, selectedNodeRect.width, EditorGUIUtility.singleLineHeight);
            DrawNodeCurve(sourceRect, mouseRect, QuestEditorStyles.NewConnectorColor);
            QuestEditorWindow.RepaintNow();
        }

        private void EndConnection()
        {
            int clickedIndex = -1;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    clickedIndex = i;
                    break;
                }
            }
            if (clickedIndex != -1 && clickedIndex != QuestEditorWindow.selectedNodeListIndex)
            {
                AddConnection(QuestEditorWindow.selectedNodeListIndex, clickedIndex);
            }
            m_connecting = false;
        }

        private void AddConnection(int sourceIndex, int destIndex)
        {
            if (sourceIndex == destIndex) return; // Disallow parenting to self.
            if (!(0 <= sourceIndex && sourceIndex < m_nodeListProperty.arraySize && 0 <= destIndex && destIndex < m_nodeListProperty.arraySize)) return;
            var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(sourceIndex);
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - node property is null in AddConnection().");
            if (nodeProperty == null) return;
            var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in AddConnection().");
            if (childIndexListProperty == null) return;
            for (int i = 0; i < childIndexListProperty.arraySize; i++)
            {
                if (childIndexListProperty.GetArrayElementAtIndex(i).intValue == destIndex)
                {
                    return; // Don't allow duplicates.
                }
            }
            childIndexListProperty.arraySize++;
            childIndexListProperty.GetArrayElementAtIndex(childIndexListProperty.arraySize - 1).intValue = destIndex;
        }

        private void ClearConnections(int sourceIndex)
        {
            ClearConnections(m_nodeListProperty.GetArrayElementAtIndex(sourceIndex));
        }

        private void ClearConnections(SerializedProperty nodeProperty)
        {
            if (nodeProperty == null) return;
            var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
            UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in ClearConnections().");
            if (childIndexListProperty == null) return;
            childIndexListProperty.arraySize = 0;
        }

        #endregion

        #region Lasso

        private void StartLasso()
        {
            m_lassoing = true;
            m_lassoStart = m_lassoEnd = m_mousePos;
            m_lassoRect = new Rect(m_mousePos, Vector2.zero);
        }

        private void UpdateLasso()
        {
            m_lassoEnd = m_mousePos;
            QuestEditorWindow.RepaintNow();
        }

        private void DrawLasso()
        {
            if (!m_lassoing) return;
            var width = Mathf.Abs(m_lassoStart.x - m_lassoEnd.x);
            var height = Mathf.Abs(m_lassoStart.y - m_lassoEnd.y);
            if (width >= 1 && height >= 1)
            {
                m_lassoRect = new Rect(Mathf.Min(m_lassoStart.x, m_lassoEnd.x), Mathf.Min(m_lassoStart.y, m_lassoEnd.y), width, height);
                if (!EditorGUIUtility.isProSkin) GUI.color = new Color(1, 1, 1, 0.5f); // Box style in personal skin is opaque, so make it semi-transparent.
                GUI.Box(m_lassoRect, GUIContent.none);
                if (!EditorGUIUtility.isProSkin) GUI.color = Color.white;
            }
        }

        private void EndLasso()
        {
            m_lassoing = false;
            QuestEditorWindow.selectedNodeListIndex = -1;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (m_lassoRect.Overlaps(nodeRect))
                {
                    if (QuestEditorWindow.selectedNodeListIndex == -1) QuestEditorWindow.selectedNodeListIndex = i;
                    QuestEditorWindow.selectedNodeListIndices.Add(i);
                }
            }
            QuestEditorWindow.RepaintNow();
        }

        #endregion

        #region Context Menu

        private void ShowContextMenu()
        {
            EditorGUIZoomArea.End(); // Stop zoom so we can place menu properly.

            int clickedIndex = -1;
            m_connecting = false;
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                var nodeRect = GetCanvasRect(nodeProperty);
                if (nodeRect.Contains(m_mousePos))
                {
                    clickedIndex = i;
                    QuestEditorWindow.selectedNodeListIndex = i;
                    break;
                }
            }

            GenericMenu menu = new GenericMenu();

            if (Application.isPlaying && m_quest.isInstance)
            {
                menu.AddItem(new GUIContent("Set State/Inactive"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.Inactive));
                menu.AddItem(new GUIContent("Set State/Active"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.Active));
                menu.AddItem(new GUIContent("Set State/True"), false, ContextCallback, new CallbackArgs(CallbackType.SetState, clickedIndex, QuestNodeState.True));
            }
            else
            { // Not play mode:
                menu.AddItem(new GUIContent("New Node/Passthrough"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Passthrough, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Condition"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Condition, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Success"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Success, Event.current.mousePosition, clickedIndex));
                menu.AddItem(new GUIContent("New Node/Failure"), false, ContextCallback, new CallbackArgs(CallbackType.Add, QuestNodeType.Failure, Event.current.mousePosition, clickedIndex));
                menu.AddSeparator("");
                if (clickedIndex >= 0)
                { // Right-clicked on a node:
                    menu.AddItem(new GUIContent("Clear Connections"), false, ContextCallback, new CallbackArgs(CallbackType.ClearConnections, clickedIndex));
                    if (QuestEditorWindow.selectedNodeListIndices.Count > 1)
                    {
                        menu.AddItem(new GUIContent("Delete Nodes"), false, ContextCallback, new CallbackArgs(CallbackType.Delete, clickedIndex));
                    }
                    else if (QuestEditorWindow.selectedNodeListIndex > 0)
                    {
                        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, new CallbackArgs(CallbackType.Delete, clickedIndex));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Delete Node"));
                    }
                    if (QuestEditorWindow.selectedNodeListIndex >= 0)
                    { 
                        menu.AddItem(new GUIContent("Copy"), false, ContextCallback, new CallbackArgs(CallbackType.Copy, clickedIndex));
                    }
                    else 
                    {
                        menu.AddDisabledItem(new GUIContent("Copy")); 
                    }
                    if (!IsClipboardEmpty())
                    {
                        menu.AddItem(new GUIContent("Paste"), false, ContextCallback, new CallbackArgs(CallbackType.Paste, clickedIndex));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }
                }
                else 
                { // Right-clicked on empty canvas:
                    menu.AddDisabledItem(new GUIContent("Copy"));
                    if (!IsClipboardEmpty())
                    {
                        menu.AddItem(new GUIContent("Paste"), false, ContextCallback, new CallbackArgs(CallbackType.Paste, clickedIndex));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    }
                }
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Wizard/Counter Requirement..."), false, OpenCounterRequirementWizard, clickedIndex);
            menu.AddItem(new GUIContent("Wizard/Message Requirement..."), false, OpenMessageRequirementWizard, clickedIndex);
            menu.AddItem(new GUIContent("Wizard/Return to QuestGiver..."), false, OpenReturnToQuestGiverWizard, clickedIndex);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Arrange Nodes"), false, ContextCallback, new CallbackArgs(CallbackType.ArrangeNodes, 0));
            AddCanvasControlMenuItems(menu);
            menu.ShowAsContext();

            EditorGUIZoomArea.Begin(m_zoom, m_zoomArea); // Resume zoom.
        }

        private enum CallbackType { Add, Delete, ClearConnections, SetState, ArrangeNodes, Copy, Paste }

        private struct CallbackArgs
        {
            public CallbackType callbackType;
            public int clickedIndex;
            public QuestNodeType questNodeType;
            public Vector2 mousePosition;
            public QuestNodeState newState;

            public CallbackArgs(CallbackType callbackType, int clickedIndex)
            {
                this.callbackType = callbackType;
                this.clickedIndex = clickedIndex;
                this.questNodeType = QuestNodeType.Success;
                this.mousePosition = Vector2.zero;
                this.newState = QuestNodeState.Inactive;
            }

            public CallbackArgs(CallbackType callbackType, QuestNodeType questNodeType, Vector2 mousePosition, int clickedIndex)
            {
                this.callbackType = callbackType;
                this.clickedIndex = clickedIndex;
                this.questNodeType = questNodeType;
                this.mousePosition = mousePosition;
                this.newState = QuestNodeState.Inactive;
            }

            public CallbackArgs(CallbackType callbackType, int clickedIndex, QuestNodeState newState)
            {
                this.callbackType = callbackType;
                this.clickedIndex = -1;
                this.questNodeType = QuestNodeType.Success;
                this.mousePosition = Vector2.zero;
                this.newState = newState;
            }
        }

        void ContextCallback(object obj)
        {
            if (obj == null || obj.GetType() != typeof(CallbackArgs) || m_nodeListProperty == null) return;

            var args = (CallbackArgs)obj;

            m_nodeListProperty.serializedObject.Update();

            switch (args.callbackType)
            {
                case CallbackType.Add:
                    AddNode(args.questNodeType, args.mousePosition, args.clickedIndex);
                    break;
                case CallbackType.ClearConnections:
                    ClearConnections(args.clickedIndex);
                    break;
                case CallbackType.Delete:
                    DeleteNodes(args.clickedIndex);
                    break;
                case CallbackType.SetState:
                    var quest = m_questSerializedObject.targetObject as Quest;
                    if (quest != null && quest.nodeList != null && 0 <= QuestEditorWindow.selectedNodeListIndex && QuestEditorWindow.selectedNodeListIndex < quest.nodeList.Count)
                    {
                        var node = quest.nodeList[QuestEditorWindow.selectedNodeListIndex];
                        if (node == null) break;
                        node.SetState(args.newState, Application.isPlaying);
                    }
                    break;
                case CallbackType.ArrangeNodes:
                    ConfirmAndArrangeNodes();
                    break;
                case CallbackType.Copy:
                    CopyToClipboard();
                    break;
                case CallbackType.Paste:
                    PasteFromClipboard();
                    break;
            }
            m_nodeListProperty.serializedObject.ApplyModifiedProperties();
        }

        private void ConfirmAndArrangeNodes()
        {
            if (QuestEditorWindow.selectedNodeListIndices.Count <= 1)
            {
                if (!EditorUtility.DisplayDialog("Arrange Nodes", "Auto-layout nodes?", "OK", "Cancel")) return;
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Arrange Nodes", "Auto-layout the selected nodes?", "OK", "Cancel")) return;
            }
            QuestEditorUtility.ArrangeNodes(m_questSerializedObject.targetObject as Quest, QuestEditorWindow.selectedNodeListIndices);
            QuestEditorWindow.RepaintNow();
        }

        private void AddNode(QuestNodeType questNodeType, Vector2 mousePosition, int parentIndex)
        {
            var parentNodeProperty = (parentIndex >= 0) ? m_nodeListProperty.GetArrayElementAtIndex(parentIndex) : null;
            m_nodeListProperty.arraySize++;
            QuestEditorWindow.selectedNodeListIndex = m_nodeListProperty.arraySize - 1;
            var childIndex = m_nodeListProperty.arraySize - 1;
            var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(childIndex);
            UnityEngine.Assertions.Assert.IsNotNull(nodeProperty, "Quest Machine: Internal error - node property is null in AddNode().");
            if (nodeProperty == null) return;
            var idProperty = nodeProperty.FindPropertyRelative("m_id");
            UnityEngine.Assertions.Assert.IsNotNull(idProperty, "Quest Machine: Internal error - m_id property is null in AddNode().");
            if (idProperty == null) return;
            var internalNameProperty = nodeProperty.FindPropertyRelative("m_internalName");
            UnityEngine.Assertions.Assert.IsNotNull(internalNameProperty, "Quest Machine: Internal error - m_internalName property is null in AddNode().");
            if (internalNameProperty == null) return;
            var stateProperty = nodeProperty.FindPropertyRelative("m_state");
            UnityEngine.Assertions.Assert.IsNotNull(stateProperty, "Quest Machine: Internal error - m_state property is null in AddNode().");
            if (stateProperty == null) return;
            var nodeTypeProperty = nodeProperty.FindPropertyRelative("m_nodeType");
            UnityEngine.Assertions.Assert.IsNotNull(nodeTypeProperty, "Quest Machine: Internal error - m_nodeType property is null in AddNode().");
            if (nodeTypeProperty == null) return;
            var isOptionalProperty = nodeProperty.FindPropertyRelative("m_isOptional");
            UnityEngine.Assertions.Assert.IsNotNull(isOptionalProperty, "Quest Machine: Internal error - m_isOptional property is null in AddNode().");
            if (isOptionalProperty == null) return;
            var stateInfoListProperty = nodeProperty.FindPropertyRelative("m_stateInfoList");
            UnityEngine.Assertions.Assert.IsNotNull(stateInfoListProperty, "Quest Machine: Internal error - m_stateInfoList property is null in AddNode().");
            if (stateInfoListProperty == null) return;
            var conditionSetProperty = nodeProperty.FindPropertyRelative("m_conditionSet");
            UnityEngine.Assertions.Assert.IsNotNull(conditionSetProperty, "Quest Machine: Internal error - m_conditionSet property is null in AddNode().");
            if (conditionSetProperty == null) return;
            var conditionListProperty = conditionSetProperty.FindPropertyRelative("m_conditionList");
            UnityEngine.Assertions.Assert.IsNotNull(conditionListProperty, "Quest Machine: Internal error - m_conditionList property is null in AddNode().");
            if (conditionListProperty == null) return;
            var conditionCountModeProperty = conditionSetProperty.FindPropertyRelative("m_conditionCountMode");
            UnityEngine.Assertions.Assert.IsNotNull(conditionCountModeProperty, "Quest Machine: Internal error - m_conditionCountMode property is null in AddNode().");
            if (conditionCountModeProperty == null) return;
            var canvasRectProperty = nodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - m_canvasRect property is null in AddNode().");
            if (canvasRectProperty == null) return;
            var initialName = (questNodeType == QuestNodeType.Success) ? "Success"
                : ((questNodeType == QuestNodeType.Failure) ? "Failure"
                   : questNodeType.ToString() + " " + (m_nodeListProperty.arraySize - 1));
            StringFieldDrawer.SetStringFieldValue(idProperty, initialName);
            StringFieldDrawer.SetStringFieldValue(internalNameProperty, string.Empty);
            stateProperty.enumValueIndex = (int)QuestState.WaitingToStart;
            nodeTypeProperty.enumValueIndex = (int)questNodeType;
            isOptionalProperty.boolValue = false;
            stateInfoListProperty.ClearArray();
            conditionListProperty.ClearArray();
            conditionCountModeProperty.enumValueIndex = (int)ConditionCountMode.All;
            ClearConnections(nodeProperty);
            var height = (questNodeType == QuestNodeType.Success || questNodeType == QuestNodeType.Failure) ? QuestEditorStyles.shortNodeHeight : QuestEditorStyles.nodeHeight;
            var rect = (parentNodeProperty != null) ? GetRectForNewChild(parentNodeProperty, height)
                : new Rect(mousePosition.x, mousePosition.y, QuestEditorStyles.nodeWidth, height);
            canvasRectProperty.rectValue = rect;
            if (parentNodeProperty != null) AddConnection(parentIndex, childIndex);
            QuestEditorWindow.selectedNodeListIndices.Clear();
            QuestEditorWindow.selectedNodeListIndex = childIndex;
        }

        private Rect GetRectForNewChild(SerializedProperty parentNodeProperty, float height)
        {
            var canvasRectProperty = parentNodeProperty.FindPropertyRelative("m_canvasRect");
            UnityEngine.Assertions.Assert.IsNotNull(canvasRectProperty, "Quest Machine: Internal error - parent's m_canvasRect property is null in AddNode().");
            if (canvasRectProperty == null) return new Rect(0, 0, QuestEditorStyles.nodeWidth, height);
            return new Rect(canvasRectProperty.rectValue.x, canvasRectProperty.rectValue.y + canvasRectProperty.rectValue.height + 20f, QuestEditorStyles.nodeWidth, height);
        }

        private void DeleteNodes(int primaryIndex)
        {
            if (primaryIndex <= 0) return;
            if (QuestEditorWindow.selectedNodeListIndices.Count > 1)
            {
                if (!EditorUtility.DisplayDialog("Delete Quest Nodes", "Are you sure you want to delete " + QuestEditorWindow.selectedNodeListIndices.Count + " quest nodes?", "OK", "Cancel")) return;
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Delete Quest Node", "Are you sure you want to delete this quest node?", "OK", "Cancel")) return;
            }
            foreach (var index in QuestEditorWindow.selectedNodeListIndices)
            {
                RemoveParentConnectionsTo(index);
                DeleteNodeSubassets(index);
            }
            QuestEditorWindow.selectedNodeListIndices.Sort();
            for (int i = QuestEditorWindow.selectedNodeListIndices.Count - 1; i >= 0; i--)
            {
                var index = QuestEditorWindow.selectedNodeListIndices[i];
                m_nodeListProperty.DeleteArrayElementAtIndex(index);
            }
            QuestEditorWindow.selectedNodeListIndex = -1;
            QuestEditorWindow.RepaintNow();
            QuestEditorWindow.RepaintCurrentEditorNow();
        }

        private void RemoveParentConnectionsTo(int childIndex)
        {
            if (m_nodeListProperty == null) return;
            // Find all nodes that link to childIndex. Remove those links.
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                if (nodeProperty == null) continue;
                var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
                UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in RemoveParentConnections().");
                if (childIndexListProperty == null) continue;

                if (childIndexListProperty.arraySize > 0)
                {
                    for (int j = childIndexListProperty.arraySize - 1; j >= 0; j--)
                    {
                        if (childIndexListProperty.GetArrayElementAtIndex(j).intValue == childIndex)
                        {
                            childIndexListProperty.DeleteArrayElementAtIndex(j);
                        }
                    }
                }
            }
            // Since we're getting rid of the node, all higher index nodes will have their indices decremented by one.
            // Find all nodes that link to higher index nodes and decrement them by one.
            for (int i = 0; i < m_nodeListProperty.arraySize; i++)
            {
                var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(i);
                if (nodeProperty == null) continue;
                var childIndexListProperty = nodeProperty.FindPropertyRelative("m_childIndexList");
                UnityEngine.Assertions.Assert.IsNotNull(childIndexListProperty, "Quest Machine: Internal error - m_childIndexList property is null in RemoveParentConnections().");
                if (childIndexListProperty == null) continue;
                if (childIndexListProperty.arraySize > 0)
                {
                    for (int j = childIndexListProperty.arraySize - 1; j >= 0; j--)
                    {
                        if (childIndexListProperty.GetArrayElementAtIndex(j).intValue > childIndex)
                        {
                            childIndexListProperty.GetArrayElementAtIndex(j).intValue--;
                        }
                    }
                }
            }
        }

        private void DragNodes()
        {
            if (Event.current.type == EventType.MouseDrag && Event.current.delta.magnitude > 0)
            {
                foreach (var index in QuestEditorWindow.selectedNodeListIndices)
                {
                    var nodeProperty = m_nodeListProperty.GetArrayElementAtIndex(index);
                    if (nodeProperty == null) continue;
                    var nodeRectProperty = GetCanvasRectProperty(nodeProperty);
                    if (nodeRectProperty == null) return;
                    nodeRectProperty.rectValue = new Rect(Mathf.Max(0, nodeRectProperty.rectValue.x + Event.current.delta.x),
                        Mathf.Max(0, nodeRectProperty.rectValue.y + Event.current.delta.y),
                        nodeRectProperty.rectValue.width, nodeRectProperty.rectValue.height);
                }
            }
            QuestEditorWindow.RepaintNow();
        }

        private void DrawNodeCurve(Rect start, Rect end, Color color)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + 24 * Vector3.up;
            Vector3 endTan = endPos + -24 * Vector3.up;
            Color shadowCol = new Color(0, 0, 0, .06f);
            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 2);
        }

        #endregion

        #region Clipboard

        public bool IsClipboardEmpty()
        {
            return QuestEditorWindow.nodeClipboard.Count == 0;
        }

        public void CopyToClipboard()
        {
            if (m_quest == null) return;
            var nodeClipboard = QuestEditorWindow.nodeClipboard;
            var indexClipboard = QuestEditorWindow.nodeIndexClipboard;
            nodeClipboard.Clear();
            indexClipboard.Clear();
            QuestProxy.includeCanvasRect = true;
            foreach (var index in QuestEditorWindow.selectedNodeListIndices)
            {
                if (!(0 <= index && index < m_quest.nodeList.Count)) continue;
                var node = m_quest.nodeList[index];
                var nodeProxy = new QuestNodeProxy(node);
                var s = JsonUtility.ToJson(nodeProxy);
                nodeClipboard.Add(s);
                indexClipboard.Add(index);
            }
            QuestProxy.includeCanvasRect = false;
        }

        public void PasteFromClipboard()
        {
            if (m_quest == null) return;

            QuestEditorWindow.ApplyModifiedPropertiesFromSelectedQuestSerializedObject();
            Undo.RecordObject(m_quest, "Paste");

            QuestProxy.includeCanvasRect = true;
            QuestEditorWindow.selectedNodeListIndex = -1;
            var nodeClipboard = QuestEditorWindow.nodeClipboard;
            var indexClipboard = QuestEditorWindow.nodeIndexClipboard;
            if (nodeClipboard.Count == 0 || nodeClipboard.Count != indexClipboard.Count) return;

            var originalNodeCount = m_quest.nodeList.Count;
            var newNodeIndex = originalNodeCount;
            var newNodeCount = originalNodeCount + nodeClipboard.Count;

            var newIndexDict = new Dictionary<int, int>(); //<original, new>
            for (int i = 0; i < indexClipboard.Count; i++)
            {
                var originalNodeIndex = indexClipboard[i];
                newIndexDict.Add(originalNodeIndex, originalNodeCount + i);
            }

            for (int i = 0; i < nodeClipboard.Count; i++)
            {
                var originalNodeIndex = indexClipboard[i];
                var nodeProxy = JsonUtility.FromJson<QuestNodeProxy>(nodeClipboard[i]);
                if (nodeProxy == null) continue;

                var node = new QuestNode();
                nodeProxy.CopyTo(node);
                m_quest.nodeList.Add(node);
                QuestEditorAssetUtility.AddNodeSubassetsToAsset(node, QuestEditorWindow.selectedQuest);
                //node.canvasRect = new Rect(node.canvasRect.x + offset.x, node.canvasRect.y + offset.y, node.canvasRect.width, node.canvasRect.height);
                for (int j = node.childIndexList.Count - 1; j >= 0 ; j--)
                {
                    var originalChildIndex = node.childIndexList[j];
                    var newChildIndex = newIndexDict.ContainsKey(originalChildIndex) ? newIndexDict[originalChildIndex] : -1;
                    if (originalNodeCount <= newChildIndex && newChildIndex < newNodeCount)
                    {
                        node.childIndexList[j] = newChildIndex;
                    }
                    else
                    {
                        node.childIndexList.RemoveAt(j);
                    }
                }

                if (QuestEditorWindow.selectedNodeListIndex == -1)
                {
                    QuestEditorWindow.selectedNodeListIndex = newNodeIndex;
                }
                QuestEditorWindow.selectedNodeListIndices.Add(newNodeIndex);
                newNodeIndex++;
            }

            // Offset pasted nodes by mouse position:
            var minX = CanvasWidth;
            var minY = CanvasHeight;
            for (int i = originalNodeCount; i < m_quest.nodeList.Count; i++)
            {
                var node = m_quest.nodeList[i];
                minX = Mathf.Min(minX, node.canvasRect.x);
                minY = Mathf.Min(minY, node.canvasRect.y);
            }
            for (int i = originalNodeCount; i < m_quest.nodeList.Count; i++)
            {
                var node = m_quest.nodeList[i];
                node.canvasRect = new Rect(node.canvasRect.x - minX + m_mousePos.x, node.canvasRect.y - minY + m_mousePos.y, node.canvasRect.width, node.canvasRect.height);
            }
            
            QuestProxy.includeCanvasRect = false;

            QuestEditorWindow.UpdateSelectedQuestSerializedObject();
            EditorUtility.SetDirty(m_quest);
        }

        #endregion

        #region Wizards

        private void OpenCounterRequirementWizard(object data)
        {
            m_wizard = new QuestEditorCounterWizard((int)data);
        }

        private void OpenMessageRequirementWizard(object data)
        {
            m_wizard = new QuestEditorMessageWizard((int)data);
        }

        private void OpenReturnToQuestGiverWizard(object data)
        {
            m_wizard = new QuestEditorReturnWizard((int)data);
        }

        private void DrawWizard()
        {
            if (m_questSerializedObject == null)
            {
                m_wizard = null;
                return;
            }
            m_questSerializedObject.ApplyModifiedProperties();
            if (!m_wizard.Draw())
            {
                m_wizard = null;
            }
            m_questSerializedObject.Update();
        }

        #endregion

        #region Gear Menu

        protected Rect GetGearMenuRect(Rect position)
        {
            return new Rect(position.width - 2 - MoreEditorGuiUtility.GearWidth, 2, MoreEditorGuiUtility.GearWidth, MoreEditorGuiUtility.GearHeight);
        }

        protected virtual void DrawGearMenu(Rect position)
        {
            if (MoreEditorGuiUtility.DoGearMenu(GetGearMenuRect(position)))
            {
                EditorGUIZoomArea.End(); // Stop zoom so we can place menu properly.
                var menu = new GenericMenu();
                AddCanvasControlMenuItems(menu);
                AddExtraGearMenuItems(menu);
                if (Application.isPlaying) AddRuntimeGearMenuItems(menu);
                AddQuestListGearMenuItems(menu);
                menu.ShowAsContext();
                EditorGUIZoomArea.Begin(m_zoom, m_zoomArea); // Resume zoom.
            }
        }

        protected virtual void AddCanvasControlMenuItems(GenericMenu menu)
        {
            if (m_quest == null)
            {
                menu.AddDisabledItem(new GUIContent("Quest Properties..."));
            }
            else
            {
                menu.AddItem(new GUIContent("Quest Properties..."), false, InspectQuestProperties);
            }
            menu.AddItem(new GUIContent("Pan/Top Left"), false, PanTopLeft, null);
            menu.AddItem(new GUIContent("Zoom/Lock"), QuestEditorPrefs.zoomLock, ToggleZoomLock, null);
            menu.AddItem(new GUIContent("Zoom/25%"), false, Zoom, 0.25f);
            menu.AddItem(new GUIContent("Zoom/50%"), false, Zoom, 0.5f);
            menu.AddItem(new GUIContent("Zoom/100%"), false, Zoom, 1f);
            menu.AddItem(new GUIContent("Zoom/150%"), false, Zoom, 1.5f);
            menu.AddItem(new GUIContent("Zoom/200%"), false, Zoom, 2f);
        }

        protected virtual void AddExtraGearMenuItems(GenericMenu menu)
        {
            if (m_quest == null)
            {
                menu.AddDisabledItem(new GUIContent("Text/Tags to Text Table..."));
                menu.AddDisabledItem(new GUIContent("Text/Move Text to Text Table..."));
                menu.AddDisabledItem(new GUIContent("Export\u2215Import/Export to JSON..."));
                menu.AddDisabledItem(new GUIContent("Export\u2215Import/Import from JSON..."));
            }
            else
            {
                menu.AddItem(new GUIContent("Text/Tags to Text Table..."), false, OpenTagsToTextTableWizard);
                menu.AddItem(new GUIContent("Text/Move Text to Text Table..."), false, OpenTextToTextTableWizard);
                menu.AddItem(new GUIContent("Export\u2215Import/Export to JSON..."), false, ExportToJson);
                menu.AddItem(new GUIContent("Export\u2215Import/Import from JSON..."), false, ImportFromJson);
                if (!Application.isPlaying && m_wizard == null)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Wizard/Counter Requirement..."), false, OpenCounterRequirementWizard, -1);
                    menu.AddItem(new GUIContent("Wizard/Message Requirement..."), false, OpenMessageRequirementWizard, -1);
                    menu.AddItem(new GUIContent("Wizard/Return to QuestGiver..."), false, OpenReturnToQuestGiverWizard, -1);
                }
            }
            if (m_quest == null || m_quest.isInstance)
            {
                menu.AddDisabledItem(new GUIContent("Debug/Delete Unused Subassets"));
            }
            else
            {
                menu.AddItem(new GUIContent("Debug/Delete Unused Subassets"), false, DeleteUnusedSubassets);
            }
        }

        protected virtual void AddRuntimeGearMenuItems(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Refresh Frequency/0.5 sec", "Refresh window every 0.5 second."),
                Mathf.Approximately(0.5f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 0.5f);
            menu.AddItem(new GUIContent("Refresh Frequency/1 sec", "Refresh window every 1 second."),
                Mathf.Approximately(1f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 1f);
            menu.AddItem(new GUIContent("Refresh Frequency/5 sec", "Refresh window every 5 seconds."),
                Mathf.Approximately(5f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 5f);
            menu.AddItem(new GUIContent("Refresh Frequency/10 sec", "Refresh window every 10 seconds."),
                Mathf.Approximately(10f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 10f);
            menu.AddItem(new GUIContent("Refresh Frequency/Never", "Never refresh at runtime."),
                Mathf.Approximately(0f, QuestEditorPrefs.runtimeRepaintFrequency), SetRuntimeRepaintFrequency, 0f);
            if (m_quest != null && m_quest.isProcedurallyGenerated)
            {
                menu.AddItem(new GUIContent("Save As Asset...", "Save this procedurally-generated quest as an asset."), false, SaveGeneratedQuestAsAsset);
            }
        }

        protected struct SwitchQuestData
        {
            public QuestListContainer questListContainer;
            public int questIndex;
            public SwitchQuestData(QuestListContainer questListContainer, int questIndex)
            {
                this.questListContainer = questListContainer;
                this.questIndex = questIndex;
            }
        }

        private void AddQuestListGearMenuItems(GenericMenu menu)
        {
            if (QuestEditorWindow.selectedQuestListContainer == null) return;
            var questListContainer = QuestEditorWindow.selectedQuestListContainer;
            if (questListContainer.questList == null) return;
            menu.AddSeparator(string.Empty);
            for (int i = 0; i < questListContainer.questList.Count; i++)
            {
                var quest = questListContainer.questList[i];
                if (quest == null)
                {
                    menu.AddDisabledItem(new GUIContent("(unassigned)"));
                }
                else
                {
                    var s = StringField.GetStringValue(quest.title);
                    if (string.IsNullOrEmpty(s)) s = StringField.GetStringValue(quest.id);
                    if (string.IsNullOrEmpty(s)) s = quest.name;
                    menu.AddItem(new GUIContent(s), quest == m_quest, SwitchToQuest, new SwitchQuestData(questListContainer, i));
                }
            }
        }

        private void SwitchToQuest(object data)
        {
            var switchQuestData = (SwitchQuestData)data;
            QuestEditorWindow.instance.SelectQuest(switchQuestData.questListContainer, switchQuestData.questIndex);
        }

        protected void Pan(float x, float y)
        {
            canvasScrollPosition = new Vector2(x / m_zoom, y / m_zoom);
        }

        protected void Zoom(float zoom)
        {
            m_zoom = zoom;
        }

        protected void PanTopLeft(object data)
        {
            Pan(0, 0);
        }

        protected void Zoom(object data)
        {
            m_zoom = (float)data;
        }

        protected void ToggleZoomLock(object data)
        {
            QuestEditorPrefs.zoomLock = !QuestEditorPrefs.zoomLock;
        }

        protected void SetRuntimeRepaintFrequency(object data)
        {
            QuestEditorPrefs.runtimeRepaintFrequency = (float)data;
        }

        private void SaveGeneratedQuestAsAsset()
        {
            var filename = EditorUtility.SaveFilePanelInProject("Save Quest As", "New Quest", "asset", "Save quest as");
            if (string.IsNullOrEmpty(filename)) return;
            QuestEditorAssetUtility.SaveQuestAsAsset(m_quest, filename, true);
        }

        private void InspectQuestProperties()
        {
            QuestEditorWindow.selectedNodeListIndex = -1;
            QuestEditorWindow.SetSelectionToQuest();
            QuestEditorWindow.RepaintNow();
            QuestEditorWindow.RepaintCurrentEditorNow();
        }

        private void OpenTagsToTextTableWizard()
        {
            QuestTagsToTextTableWizard.Open();
        }

        private void OpenTextToTextTableWizard()
        {
            QuestTextToTextTableWizard.Open();
        }

        private void ExportToJson()
        {
            var newJsonFilename = EditorUtility.SaveFilePanel("Export to JSON", System.IO.Path.GetDirectoryName(jsonFilename), System.IO.Path.GetFileName(jsonFilename), "json");
            if (string.IsNullOrEmpty(newJsonFilename)) return;
            jsonFilename = newJsonFilename;
            var originalIncludeCanvasRect = QuestProxy.includeCanvasRect;
            QuestProxy.includeCanvasRect = true;
            var proxy = new QuestProxy(m_quest);
            QuestProxy.includeCanvasRect = originalIncludeCanvasRect;
            System.IO.File.WriteAllText(jsonFilename, JsonUtility.ToJson(proxy));
            Debug.Log(m_quest.title + " saved to " + jsonFilename);
        }

        private void ImportFromJson()
        {
            var newJsonFilename = EditorUtility.OpenFilePanel("Import from JSON", System.IO.Path.GetDirectoryName(jsonFilename), "json");
            if (string.IsNullOrEmpty(newJsonFilename)) return;
            var json = System.IO.File.ReadAllText(newJsonFilename);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError(newJsonFilename + " is empty.");
                return;
            }
            var proxy = JsonUtility.FromJson<QuestProxy>(json);
            if (proxy == null)
            {
                Debug.LogError(newJsonFilename + " is not a valid JSON format.");
                return;
            }
            if (!EditorUtility.DisplayDialog("Import " + System.IO.Path.GetFileName(newJsonFilename), "This will replace the quest in the editor with the contents of the JSON file. Proceed?", "Replace", "Cancel"))
            {
                return;
            }
            jsonFilename = newJsonFilename;
            var originalIncludeCanvasRect = QuestProxy.includeCanvasRect;
            QuestProxy.includeCanvasRect = true;
            proxy.CopyTo(m_quest);
            QuestProxy.includeCanvasRect = originalIncludeCanvasRect;
            AssignQuest(m_quest);
            Debug.Log("Imported " + jsonFilename);
        }

        #endregion

        #region Delete Subassets

        // When deleting a node, need to delete all its subassets or they will be left orphaned in the quest asset:
        private void DeleteNodeSubassets(int index)
        {
            // Serialized changes have been applied at this point, so we can work directly with m_quest:
            if (m_quest == null || !(0 <= index && index < m_quest.nodeList.Count)) return;
            var node = m_quest.nodeList[index];
            DeleteNodeConditionSubassets(node);
            DeleteNodeStateInfoListSubassets(node);
        }

        private void DeleteNodeConditionSubassets(QuestNode node)
        {
            if (node == null) return;
            DeleteSubassetsFromList(node.conditionSet.conditionList);
        }

        private void DeleteNodeStateInfoListSubassets(QuestNode node)
        {
            if (node == null) return;
            for (int i = 0; i < node.stateInfoList.Count; i++)
            {
                var stateInfo = node.stateInfoList[i];
                if (stateInfo == null) continue;
                DeleteSubassetsFromList(stateInfo.actionList);
                DeleteCategorizedContentListSubassets(stateInfo.categorizedContentList);
            }
        }

        private void DeleteCategorizedContentListSubassets(List<QuestContentSet> categorizedContentList)
        {
            if (categorizedContentList == null) return;
            for (int i = 0; i < categorizedContentList.Count; i++)
            {
                var contentSet = categorizedContentList[i];
                if (contentSet == null) continue;
                DeleteSubassetsFromList(contentSet.contentList);
            }
        }

        private void DeleteSubassetsFromList<T>(List<T> subassetList) where T : QuestSubasset
        {
            if (subassetList == null) return;
            for (int i = 0; i < subassetList.Count; i++)
            {
                var subasset = subassetList[i];
                if (subasset == null) continue;
                DeleteNestedSubassetsFromSubasset(subasset);
                AssetUtility.DeleteFromAsset(subasset, m_quest);
            }
            subassetList.Clear();
        }

        protected virtual void DeleteNestedSubassetsFromSubasset(ScriptableObject subasset)
        {
            // Special handling for subassets that can have child subassets:
            if (subasset is ButtonQuestContent)
            {
                DeleteSubassetsFromList((subasset as ButtonQuestContent).actionList);
            }
            if (subasset is AlertQuestAction)
            {
                DeleteSubassetsFromList((subasset as AlertQuestAction).contentList);
            }
        }

        // A bug in 1.0.0 - 1.0.2 orphaned subassets when deleting a node. 
        // This method scrubs them from the quest asset:
        private void DeleteUnusedSubassets()
        {
            QuestEditorAssetUtility.DeleteUnusedSubassets(m_quest);
        }

        #endregion

        #region Quest Relations in Quest Database

        [Serializable]
        private class QuestRelationRecord
        {
            public Quest quest;
            public Rect rect;
            public string label;
            public List<int> parentQuests;
            public List<int> childQuests;
        }
        private List<QuestRelationRecord> m_questRelationRecords = new List<QuestRelationRecord>();
        private QuestDatabase m_currentDatabase = null;
        private Rect m_lastWindowSize = new Rect(0, 0, 0, 0);

        public void DrawQuestRelations(QuestDatabase database)
        {
            if (database == null) return;
            if (database != m_currentDatabase || m_lastWindowSize.width != QuestEditorWindow.instance.position.width)
            {
                SetupQuestRelations(database);
            }
            EditorGUILayout.LabelField(database.name, QuestEditorStyles.questNameGUIStyle);
            for (int i = 0; i < m_questRelationRecords.Count; i++)
            {
                var record = m_questRelationRecords[i];
                var edit = GUI.Button(record.rect, record.label, QuestEditorStyles.questNodeWindowGUIStyle);
                if (edit)
                {
                    QuestEditorWindow.instance.SelectQuest(record.quest);
                    return;
                }
                for (int j = 0; j < record.childQuests.Count; j++)
                {
                    var otherRecord = m_questRelationRecords[record.childQuests[j]];
                    DrawNodeCurve(record.rect, otherRecord.rect, QuestEditorStyles.ChildRelationConnectorColor);
                }
                for (int j = 0; j < record.parentQuests.Count; j++)
                {
                    var otherRecord = m_questRelationRecords[record.parentQuests[j]];
                    DrawNodeCurve(record.rect, otherRecord.rect, QuestEditorStyles.ParentRelationConnectorColor);
                }
            }
        }

        private void SetupQuestRelations(QuestDatabase database)
        {
            m_currentDatabase = database;
            m_lastWindowSize = QuestEditorWindow.instance.position;
            var windowWidth = m_lastWindowSize.width;
            int numColumns = Mathf.Max(1, (int)(windowWidth / (QuestNode.DefaultNodeWidth * 2)));

            // Set quest record positions:
            m_questRelationRecords.Clear();
            for (int i = 0; i < database.questAssets.Count; i++)
            {
                var record = new QuestRelationRecord();
                var quest = database.questAssets[i];
                if (quest == null) continue;
                string label = StringField.GetStringValue(quest.id);
                var pos = new Vector2(QuestNode.DefaultNodeWidth + (i % numColumns) * (QuestNode.DefaultNodeWidth * 2), (1 + (i / numColumns)) * (QuestNode.DefaultNodeHeight * 2));
                var rect = new Rect(pos.x, pos.y, QuestNode.DefaultNodeWidth, QuestNode.DefaultNodeHeight);
                record.quest = quest;
                record.rect = rect;
                record.label = label;
                record.childQuests = new List<int>();
                record.parentQuests = new List<int>();
                m_questRelationRecords.Add(record);
            }

            // Determine links:
            for (int i = 0; i < m_questRelationRecords.Count; i++)
            {
                var record = m_questRelationRecords[i];
                for (int j = 0; j < m_questRelationRecords.Count; j++)
                {
                    if (i == j) continue;
                    var otherQuest = m_questRelationRecords[j].quest;
                    if (IsQuestChild(record.quest, otherQuest))
                    {
                        record.childQuests.Add(j);
                    }
                    if (IsQuestParent(record.quest, otherQuest))
                    {
                        record.parentQuests.Add(j);
                    }
                }
            }
        }

        private bool IsQuestChild(Quest quest, Quest other)
        {
            if (DoesConditionSetReferenceMe(quest, other.autostartConditionSet))
            {
                return true;
            }
            if (DoesConditionSetReferenceMe(quest, other.offerConditionSet))
            {
                return true;
            }
            foreach (var node in other.nodeList)
            {
                if (DoesConditionSetReferenceMe(quest, node.conditionSet))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoesConditionSetReferenceMe(Quest quest, QuestConditionSet conditionSet)
        {
            foreach (var condition in conditionSet.conditionList)
            {
                if (condition is QuestStateQuestCondition)
                {
                    if (StringField.Equals((condition as QuestStateQuestCondition).requiredQuestID, quest.id))
                    {
                        return true;
                    }
                }
                else if (condition is QuestNodeStateQuestCondition)
                {
                    if (StringField.Equals((condition as QuestNodeStateQuestCondition).requiredQuestID, quest.id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsQuestParent(Quest quest, Quest other)
        {
            foreach (var stateInfo in other.stateInfoList)
            {
                if (DoesActionListReferenceMe(quest, stateInfo.actionList))
                {
                    return true;
                }
            }
            foreach (var node in other.nodeList)
            {
                foreach (var stateInfo in node.stateInfoList)
                {
                    if (DoesActionListReferenceMe(quest, stateInfo.actionList))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool DoesActionListReferenceMe(Quest quest, List<QuestAction> actionList)
        {
            foreach (var action in actionList)
            {
                if (action is SetQuestStateQuestAction)
                {
                    if (StringField.Equals((action as SetQuestStateQuestAction).questID, quest.id))
                    {
                        return true;
                    }
                }
                else if (action is SetQuestNodeStateQuestAction)
                {
                    if (StringField.Equals((action as SetQuestNodeStateQuestAction).questID, quest.id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
