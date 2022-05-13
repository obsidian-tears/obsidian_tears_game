// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(EntityType), true)]
    public class EntityTypeEditor : Editor
    {

        private ReorderableList m_parentList = null;
        private ReorderableList m_urgencyFunctionList = null;
        private ReorderableList m_actionList = null;
        private ReorderableList m_driveValueList = null;

        private void OnEnable()
        {
            SetupParentList();
            SetupUrgencyFunctionList();
            SetupActionList();
            SetupDriveValueList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawTopInfo();
            DrawParentList();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawUrgencyFunctionList();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawActionList();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawDriveValueList();
            QuestEditorUtility.EditorGUILayoutVerticalSpace(2);
            DrawRewardMultipliers();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTopInfo()
        {
            var imageProperty = serializedObject.FindProperty("m_image");
            Sprite image = (imageProperty != null && imageProperty.objectReferenceValue != null && imageProperty.objectReferenceValue.GetType() == typeof(Sprite))
                ? (Sprite)imageProperty.objectReferenceValue : null;
            if (image != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(image.texture, GUILayout.Width(32), GUILayout.Height(32));
            }
            EditorGUILayout.HelpBox("Entity types are abstract definitions of entities. Every entity in the game world has an entity type that defines " +
                "its attributes such as its faction with other entities and actions that can be performed on it. To assign an entity type to an entity, " +
                "inspect the entity in its scene.", MessageType.None);
            if (image != null)
            {
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isUnique"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_displayName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_pluralDisplayName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_image"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_level"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_faction"));
        }

        #region Parent List

        private void SetupParentList()
        {
            m_parentList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_parents"), true, true, true, true);
            m_parentList.drawHeaderCallback += OnDrawParentListHeader;
            m_parentList.drawElementCallback += OnDrawParentListElement;
            //--- Beta testers prefer manual assignment, so don't use dropdown: m_parentList.onAddDropdownCallback += OnParentListAddDropdown;
            m_parentList.onRemoveCallback += QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull;
        }

        private void DrawParentList()
        {
            QuestEditorPrefs.entityTypeParentsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Parents", "Parent entity types from which this entity type inherits urgency functions, actions, and drive values.", QuestEditorPrefs.entityTypeParentsFoldout);
            if (!QuestEditorPrefs.entityTypeParentsFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("Parent entity types from which this entity type inherits urgency functions, actions, and drive values.", MessageType.None);
                m_parentList.DoLayoutList();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void OnDrawParentListHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 12, rect.y, rect.width, rect.height), "Parent");
        }

        private void OnDrawParentListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var parentsProperty = serializedObject.FindProperty("m_parents");
            if (parentsProperty == null || !(0 <= index && index < parentsProperty.arraySize)) return;
            var property = parentsProperty.GetArrayElementAtIndex(index);
            if (property == null) return;
            EditorGUI.PropertyField(rect, property, GUIContent.none, true);
        }

        private void OnParentListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var entityTypeList = AssetInfoLists.GetList(typeof(EntityType));
            if (entityTypeList == null) return;
            for (int i = 0; i < entityTypeList.Count; i++)
            {
                if (entityTypeList[i] == null) continue;
                var entityType = EditorUtility.InstanceIDToObject(entityTypeList[i].instanceID) as EntityType;
                if (entityType == null) continue;
                var myEntityType = target as EntityType;
                var isInList = myEntityType != null && myEntityType.parents != null && myEntityType.parents.Contains(entityType);
                if (isInList) continue;
                menu.AddItem(new GUIContent(entityTypeList[i].pathAndName), false, OnClickAddParentList, entityType);
            }
            menu.ShowAsContext();
        }

        private void OnClickAddParentList(object data)
        {
            var entityType = data as EntityType;
            if (entityType == null) return;
            serializedObject.Update();
            var parentsProperty = serializedObject.FindProperty("m_parents");
            if (parentsProperty == null) return;
            parentsProperty.arraySize++;
            parentsProperty.GetArrayElementAtIndex(parentsProperty.arraySize - 1).objectReferenceValue = entityType;
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Urgency Function List

        private void SetupUrgencyFunctionList()
        {
            m_urgencyFunctionList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_urgencyFunctions"), true, true, true, true);
            m_urgencyFunctionList.drawHeaderCallback += OnDrawUrgencyFunctionListHeader;
            m_urgencyFunctionList.drawElementCallback += OnDrawUrgencyFunctionListElement;
            //--- Beta testers prefer manual assignment, so don't use dropdown: m_urgencyFunctionList.onAddDropdownCallback += OnUrgencyFunctionListAddDropdown;
            m_urgencyFunctionList.onRemoveCallback += QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull;
        }

        private void DrawUrgencyFunctionList()
        {
            QuestEditorPrefs.entityTypeUrgencyFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Urgency Functions", "Factors that indicate how urgent it is for the quest giver to generate a quest about this entity type.", QuestEditorPrefs.entityTypeUrgencyFoldout);
            if (!QuestEditorPrefs.entityTypeUrgencyFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                if (m_urgencyFunctionList.count == 0)
                {
                    EditorGUILayout.HelpBox("Factors that indicate how urgent it is for the quest giver to generate a quest about this entity type. If you want quest givers to be able to generate a quest about this entity type, add at least one urgency function.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Factors that indicate how urgent it is for the quest giver to generate a quest about this entity type.", MessageType.None);
                }
                m_urgencyFunctionList.DoLayoutList();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void OnDrawUrgencyFunctionListHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 12, rect.y, rect.width, rect.height), "Urgency Function");
        }

        private void OnDrawUrgencyFunctionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var arrayProperty = serializedObject.FindProperty("m_urgencyFunctions");
            if (arrayProperty == null || !(0 <= index && index < arrayProperty.arraySize)) return;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty == null) return;
            EditorGUI.PropertyField(rect, elementProperty, GUIContent.none, true);
        }

        private void OnUrgencyFunctionListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var assets = AssetInfoLists.GetList(typeof(UrgencyFunction));
            if (assets == null) return;
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i] == null) continue;
                var asset = EditorUtility.InstanceIDToObject(assets[i].instanceID) as UrgencyFunction;
                if (asset == null) continue;
                var isInList = (target as EntityType).urgencyFunctions.Contains(asset);
                if (isInList) continue;
                menu.AddItem(new GUIContent(assets[i].pathAndName), false, OnClickAddUrgencyFunctionList, asset);
            }
            menu.ShowAsContext();
        }

        private void OnClickAddUrgencyFunctionList(object data)
        {
            var urgencyFunction = data as UrgencyFunction;
            if (urgencyFunction == null) return;
            serializedObject.Update();
            var arrayProperty = serializedObject.FindProperty("m_urgencyFunctions");
            if (arrayProperty == null) return;
            arrayProperty.arraySize++;
            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = urgencyFunction;
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Action List

        private void SetupActionList()
        {
            m_actionList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_actions"), true, true, true, true);
            m_actionList.drawHeaderCallback += OnDrawActionListHeader;
            m_actionList.drawElementCallback += OnDrawActionListElement;
            //--- Beta testers prefer manual assignment: m_actionList.onAddDropdownCallback += OnActionListAddDropdown;
            m_actionList.onRemoveCallback += QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull;
        }

        private void DrawActionList()
        {
            QuestEditorPrefs.entityTypeActionsFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Actions", "Actions that can be done to this entity type.", QuestEditorPrefs.entityTypeActionsFoldout);
            if (!QuestEditorPrefs.entityTypeActionsFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                if (m_actionList.count == 0)
                {
                    EditorGUILayout.HelpBox("Actions that can be done to this entity type. If you want quest givers to be able to ask the player to do something to this entity type in a quest, add at least one action. Actions defined on this entity type's parents can also be done to this entity type.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Actions that can be done to this entity type. Actions defined on this entity type's parents can also be done to this entity type.", MessageType.None);
                }
                m_actionList.DoLayoutList();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxCountInAction"));
        }

        private void OnDrawActionListHeader(Rect rect)
        {
            EditorGUI.LabelField(new Rect(rect.x + 12, rect.y, rect.width, rect.height), "Action");
        }

        private void OnDrawActionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var arrayProperty = serializedObject.FindProperty("m_actions");
            if (arrayProperty == null || !(0 <= index && index < arrayProperty.arraySize)) return;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty == null) return;
            EditorGUI.PropertyField(rect, elementProperty, GUIContent.none, true);
        }

        private void OnActionListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var assets = AssetInfoLists.GetList(typeof(Action));
            if (assets == null) return;
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i] == null) continue;
                var asset = EditorUtility.InstanceIDToObject(assets[i].instanceID) as Action;
                if (asset == null) continue;
                var isInList = (target as EntityType).actions.Contains(asset);
                if (isInList) continue;
                menu.AddItem(new GUIContent(assets[i].pathAndName), false, OnClickAddActionList, asset);
            }
            menu.ShowAsContext();
        }

        private void OnClickAddActionList(object data)
        {
            var action = data as Action;
            if (action == null) return;
            serializedObject.Update();
            var arrayProperty = serializedObject.FindProperty("m_actions");
            if (arrayProperty == null) return;
            arrayProperty.arraySize++;
            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = action;
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region DriveValue List

        private void SetupDriveValueList()
        {
            m_driveValueList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_driveValues"), true, true, true, true);
            m_driveValueList.drawHeaderCallback += OnDrawDriveValueListHeader;
            m_driveValueList.drawElementCallback += OnDrawDriveValueListElement;
            //--- Beta testers prefer manual assignment: m_driveValueList.onAddDropdownCallback += OnDriveValueListAddDropdown;
            m_driveValueList.onRemoveCallback += QuestEditorUtility.RemoveReorderableListElementWithoutLeavingNull;
        }

        private void DrawDriveValueList()
        {
            QuestEditorPrefs.entityTypeDriveFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Drive Values", "Drive values associated with this entity type.", QuestEditorPrefs.entityTypeDriveFoldout);
            if (!QuestEditorPrefs.entityTypeDriveFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();

                EditorGUILayout.HelpBox("Drive values associated with this entity type. Drive values influence how entities are used in quests. Design-time values are shown below, not runtime values.", MessageType.None);
                m_driveValueList.DoLayoutList();
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        private void OnDrawDriveValueListHeader(Rect rect)
        {
            var fieldWidth = rect.width / 2;
            EditorGUI.LabelField(new Rect(12 + rect.x, rect.y, fieldWidth, rect.height), "Drive");
            EditorGUI.LabelField(new Rect(12 + rect.x + fieldWidth, rect.y, fieldWidth, rect.height), "Value");
        }

        private void OnDrawDriveValueListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var arrayProperty = serializedObject.FindProperty("m_driveValues");
            if (arrayProperty == null || !(0 <= index && index < arrayProperty.arraySize)) return;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(index);
            if (elementProperty == null) return;
            var driveProperty = elementProperty.FindPropertyRelative("m_drive");
            var valueProperty = elementProperty.FindPropertyRelative("m_value");
            if (driveProperty == null || valueProperty == null) return;
            var fieldWidth = rect.width / 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), driveProperty, GUIContent.none, true);
            EditorGUI.PropertyField(new Rect(rect.x + fieldWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), valueProperty, GUIContent.none, true);
        }

        private void OnDriveValueListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var assets = AssetInfoLists.GetList(typeof(Drive));
            if (assets == null) return;
            for (int i = 0; i < assets.Count; i++)
            {
                if (assets[i] == null) continue;
                var asset = EditorUtility.InstanceIDToObject(assets[i].instanceID) as Drive;
                if (asset == null) continue;
                var isInList = (target as EntityType).driveValues.Find(x => x != null && x.drive == asset) != null;
                if (isInList) continue;
                menu.AddItem(new GUIContent(assets[i].pathAndName), false, OnClickAddDriveValueList, asset);
            }
            menu.ShowAsContext();
        }

        private void OnClickAddDriveValueList(object data)
        {
            var drive = data as Drive;
            if (drive == null) return;
            serializedObject.Update();
            var arrayProperty = serializedObject.FindProperty("m_driveValues");
            if (arrayProperty == null) return;
            arrayProperty.arraySize++;
            var elementProperty = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
            var driveProperty = (elementProperty != null) ? elementProperty.FindPropertyRelative("m_drive") : null;
            if (driveProperty != null) driveProperty.objectReferenceValue = drive;
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Reward Multipliers

        private void DrawRewardMultipliers()
        {
            QuestEditorPrefs.entityTypeRewardMultipliersFoldout = QuestEditorUtility.EditorGUILayoutFoldout("Reward Multipliers", "Scale points used to give rewards.", QuestEditorPrefs.entityTypeRewardMultipliersFoldout);
            if (!QuestEditorPrefs.entityTypeRewardMultipliersFoldout) return;
            try
            {
                QuestEditorUtility.EditorGUILayoutBeginGroup();
                EditorGUILayout.HelpBox("Reward systems give rewards based on an entity's level. Using the multipliers below, " +
                    "entities can scale reward points. For example, a boss enemy might scale Currency by 10 x its level.", MessageType.None);
                var multipliers = serializedObject.FindProperty("m_rewardMultipliers");
                for (int i = 0; i < multipliers.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(multipliers.GetArrayElementAtIndex(i), new GUIContent(((RewardMultiplier)i).ToString()), true);
                }
            }
            finally
            {
                QuestEditorUtility.EditorGUILayoutEndGroup();
            }
        }

        #endregion

    }

}