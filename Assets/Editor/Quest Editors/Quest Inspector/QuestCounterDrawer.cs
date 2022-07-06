// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom property drawer for QuestCounter.
    /// </summary>
    [CustomPropertyDrawer(typeof(QuestCounter))]
    public class QuestCounterDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null) return base.GetPropertyHeight(property, label);

            // Height of m_name:
            var nameProperty = property.FindPropertyRelative("m_name");
            UnityEngine.Assertions.Assert.IsNotNull(nameProperty, "Quest Machine: Internal error - m_name is null.");
            if (nameProperty == null) return base.GetPropertyHeight(property, label);
            var height = StringFieldDrawer.GetHeight(nameProperty);

            // Height of current/min/max/random values:
            height += 4 * EditorGUIUtility.singleLineHeight;

            // Height of m_updateMode:
            var updateModeProperty = property.FindPropertyRelative("m_updateMode");
            UnityEngine.Assertions.Assert.IsNotNull(updateModeProperty, "Quest Machine: Internal error - m_updateMode is null.");
            if (updateModeProperty == null) return base.GetPropertyHeight(property, label);
            height += EditorGUIUtility.singleLineHeight;

            // Optional height of m_messageEventList:
            if (updateModeProperty.enumValueIndex == (int)QuestCounterUpdateMode.Messages)
            {
                var messageEventListProperty = property.FindPropertyRelative("m_messageEventList");
                UnityEngine.Assertions.Assert.IsNotNull(messageEventListProperty, "Quest Machine: Internal error - m_messageEventList is null.");
                if (messageEventListProperty == null) return base.GetPropertyHeight(property, label);
                for (int i = 0; i < messageEventListProperty.arraySize; i++)
                {
                    var messageEventProperty = messageEventListProperty.GetArrayElementAtIndex(i);
                    var senderSpecifierProperty = messageEventProperty.FindPropertyRelative("m_senderSpecifier");
                    var senderIDProperty = messageEventProperty.FindPropertyRelative("m_senderID");
                    var targetSpecifierProperty = messageEventProperty.FindPropertyRelative("m_targetSpecifier");
                    var targetIDProperty = messageEventProperty.FindPropertyRelative("m_targetID");
                    var messageProperty = messageEventProperty.FindPropertyRelative("m_message");
                    var parameterProperty = messageEventProperty.FindPropertyRelative("m_parameter");
                    if (senderSpecifierProperty != null && senderIDProperty != null && targetSpecifierProperty != null && targetIDProperty != null &&
                        messageProperty != null && parameterProperty != null)
                    {
                        height += EditorGUIUtility.singleLineHeight; // Sender specifier.
                        if (senderSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other) height += StringFieldDrawer.GetHeight(senderIDProperty);
                        height += EditorGUIUtility.singleLineHeight; // Target specifier.
                        if (targetSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other) height += StringFieldDrawer.GetHeight(targetIDProperty);
                        height += StringFieldDrawer.GetHeight(messageProperty);
                        height += StringFieldDrawer.GetHeight(parameterProperty);
                        height += EditorGUIUtility.singleLineHeight; // 'Operation' dropdown & 'Delete' button.
                    }
                }
                height += EditorGUIUtility.singleLineHeight; // 'Add' button.
            }
            return height;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            try
            {
                var nameProperty = property.FindPropertyRelative("m_name");
                UnityEngine.Assertions.Assert.IsNotNull(nameProperty, "Quest Machine: Internal error - m_name is null.");
                if (nameProperty == null) return;

                var y = rect.y;
                var nameHeight = StringFieldDrawer.GetHeight(nameProperty);
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, nameHeight),
                    nameProperty, new GUIContent("Name", "The counter's name."));
                y += nameHeight;

                var randomizeProperty = property.FindPropertyRelative("m_randomizeInitialValue");
                UnityEngine.Assertions.Assert.IsNotNull(randomizeProperty, "Quest Machine: Internal error - m_randomizeInitialValue is null.");
                if (randomizeProperty == null) return;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    randomizeProperty, new GUIContent("Randomize Initial Value", "Initialize to a random value between Min Value and Max Value."));
                y += EditorGUIUtility.singleLineHeight;

                if (randomizeProperty.boolValue) EditorGUI.BeginDisabledGroup(true);
                var currentValueProperty = property.FindPropertyRelative("m_currentValue");
                UnityEngine.Assertions.Assert.IsNotNull(currentValueProperty, "Quest Machine: Internal error - m_currentValue is null.");
                if (currentValueProperty == null) return;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    currentValueProperty, new GUIContent("Current Value", "The counter's current value."));
                y += EditorGUIUtility.singleLineHeight;
                if (randomizeProperty.boolValue) EditorGUI.EndDisabledGroup();

                var minValueProperty = property.FindPropertyRelative("m_minValue");
                UnityEngine.Assertions.Assert.IsNotNull(minValueProperty, "Quest Machine: Internal error - m_minValue is null.");
                var maxValueProperty = property.FindPropertyRelative("m_maxValue");
                UnityEngine.Assertions.Assert.IsNotNull(maxValueProperty, "Quest Machine: Internal error - m_maxValue is null.");
                if (minValueProperty == null) return;
                if (maxValueProperty == null) return;

                var isMinMaxInvalid = maxValueProperty.intValue <= minValueProperty.intValue;

                var originalColor = GUI.color;
                if (isMinMaxInvalid) GUI.color = Color.red;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    minValueProperty, new GUIContent("Min Value", "Minimum value counter can have."));
                y += EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    maxValueProperty, new GUIContent("Max Value", "Maximum value counter can have."));
                y += EditorGUIUtility.singleLineHeight;

                if (isMinMaxInvalid) GUI.color = originalColor;

                var updateModeProperty = property.FindPropertyRelative("m_updateMode");
                UnityEngine.Assertions.Assert.IsNotNull(updateModeProperty, "Quest Machine: Internal error - m_updateMode is null.");
                if (updateModeProperty == null) return;
                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight),
                    updateModeProperty, new GUIContent("Value Mode", "If Data Sync, synchronizes value with a DataSynchronizer component using the exact name specified above. If Messages, adjusts value based on Message System messages defined below."));
                y += EditorGUIUtility.singleLineHeight;

                if (updateModeProperty.enumValueIndex == (int)QuestCounterUpdateMode.Messages)
                {
                    rect = new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height);
                    int indexToDelete = -1;
                    var messageEventListProperty = property.FindPropertyRelative("m_messageEventList");
                    UnityEngine.Assertions.Assert.IsNotNull(messageEventListProperty, "Quest Machine: Internal error - m_messageEventList is null.");
                    if (messageEventListProperty == null) return;
                    for (int i = 0; i < messageEventListProperty.arraySize; i++)
                    {
                        var messageEventProperty = messageEventListProperty.GetArrayElementAtIndex(i);
                        var senderSpecifierProperty = messageEventProperty.FindPropertyRelative("m_senderSpecifier");
                        var senderIDProperty = messageEventProperty.FindPropertyRelative("m_senderID");
                        var targetSpecifierProperty = messageEventProperty.FindPropertyRelative("m_targetSpecifier");
                        var targetIDProperty = messageEventProperty.FindPropertyRelative("m_targetID");
                        var messageProperty = messageEventProperty.FindPropertyRelative("m_message");
                        var parameterProperty = messageEventProperty.FindPropertyRelative("m_parameter");
                        var operationProperty = messageEventProperty.FindPropertyRelative("m_operation");
                        var literalValueProperty = messageEventProperty.FindPropertyRelative("m_literalValue");
                        if (senderSpecifierProperty == null || senderIDProperty == null || targetSpecifierProperty == null || targetIDProperty == null ||
                            messageProperty == null || parameterProperty == null || operationProperty == null || literalValueProperty == null) continue;

                        var height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), senderSpecifierProperty,
                            new GUIContent("Sender", "Required message sender."), true);
                        if (EditorGUI.EndChangeCheck()) QuestEditorUtility.SetMessageParticipantID(senderSpecifierProperty, senderIDProperty);
                        y += height;                        

                        if (senderSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other)
                        {
                            height = StringFieldDrawer.GetHeight(senderIDProperty);
                            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), senderIDProperty,
                                new GUIContent("Sender ID", "Required message sender ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Sender must have a Quest Giver or Entity component."), true);
                            y += height;
                        }

                        height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), targetSpecifierProperty,
                            new GUIContent("Target", "Required message target."), true);
                        if (EditorGUI.EndChangeCheck()) QuestEditorUtility.SetMessageParticipantID(targetSpecifierProperty, targetIDProperty);
                        y += height;                        

                        if (targetSpecifierProperty.enumValueIndex == (int)QuestMessageParticipant.Other)
                        {
                            height = StringFieldDrawer.GetHeight(targetIDProperty);
                            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), targetIDProperty,
                                new GUIContent("Target ID", "Required message target ID, or any sender if blank. Can also be {QUESTERID} or {QUESTGIVERID}. Target must have a Quest Giver or Entity component."), true);
                            y += height;
                        }

                        height = StringFieldDrawer.GetHeight(messageProperty);
                        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), messageProperty,
                            new GUIContent("Message", "When this message and the parameter below are sent, update the counter according to the operation below."), true);
                        y += height;
                        height = StringFieldDrawer.GetHeight(parameterProperty);
                        EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, height), parameterProperty,
                            new GUIContent("Parameter", "When the message above and this parameter are sent, update the counter according to the operation below."), true);
                        y += height;
                        var fieldWidth = (rect.width - 30f) / 2f;
                        var operation = (QuestCounterMessageEvent.Operation)operationProperty.enumValueIndex;
                        if (operation == QuestCounterMessageEvent.Operation.ModifyByMessageValue || operation == QuestCounterMessageEvent.Operation.SetToMessageValue)
                        {
                            EditorGUI.PropertyField(new Rect(rect.x, y, rect.width - 30f, EditorGUIUtility.singleLineHeight), operationProperty, GUIContent.none);
                        }
                        else
                        {
                            EditorGUI.PropertyField(new Rect(rect.x, y, fieldWidth, EditorGUIUtility.singleLineHeight), operationProperty, GUIContent.none);
                            EditorGUI.PropertyField(new Rect(rect.x + fieldWidth, y, fieldWidth, EditorGUIUtility.singleLineHeight), literalValueProperty, GUIContent.none);
                        }
                        if (GUI.Button(new Rect(rect.x + rect.width - 30f, y, 30f, EditorGUIUtility.singleLineHeight), new GUIContent("-", "Delete this message event.")))
                        {
                            indexToDelete = i;
                        }
                        y += EditorGUIUtility.singleLineHeight;
                    }
                    if (indexToDelete != -1) messageEventListProperty.DeleteArrayElementAtIndex(indexToDelete);
                    if (GUI.Button(new Rect(rect.x + rect.width - 120f, y, 120f, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Add Message", "Add a new message event for the counter."), EditorStyles.miniButton))
                    {
                        messageEventListProperty.arraySize++;
                        var messageEventProperty = messageEventListProperty.GetArrayElementAtIndex(messageEventListProperty.arraySize - 1);
                        messageEventProperty.FindPropertyRelative("m_senderSpecifier").enumValueIndex = (int)QuestMessageParticipant.Any;
                        messageEventProperty.FindPropertyRelative("m_targetSpecifier").enumValueIndex = (int)QuestMessageParticipant.Any;
                        messageEventProperty.FindPropertyRelative("m_message").FindPropertyRelative("m_text").stringValue = string.Empty;
                        messageEventProperty.FindPropertyRelative("m_parameter").FindPropertyRelative("m_text").stringValue = string.Empty;
                        messageEventProperty.FindPropertyRelative("m_operation").enumValueIndex = (int)QuestCounterMessageEvent.Operation.ModifyByLiteralValue;
                        messageEventProperty.FindPropertyRelative("m_literalValue").intValue = 1;
                    }
                }
            }
            finally
            {
                EditorGUI.EndProperty();
            }
        }

    }

}
