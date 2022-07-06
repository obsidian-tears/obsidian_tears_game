// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(QuestGeneratorEntity), true)]
    public class QuestGeneratorEntityEditor : Editor
    {

#if DEBUG_QUEST_EDITOR
        protected static bool defaultInspectorFoldout = false;
#endif

        protected QuestContentListInspectorGUI rewardsContentGUI { get; set; }
        protected ReorderableList rewardSystemList { get; set; }
        protected bool mustExitGUI { get; set; }

        protected virtual void OnEnable()
        {
            rewardsContentGUI = new QuestContentListInspectorGUI(new GUIContent("Dialogue for Rewards Section", "Show this UI content above the list of rewards offered for a quest."), QuestContentCategory.Dialogue);
            rewardSystemList = new ReorderableList(serializedObject, serializedObject.FindProperty("m_rewardSystems"), true, true, true, true);
            rewardSystemList.drawHeaderCallback += OnDrawRewardSystemListHeader;
            rewardSystemList.drawElementCallback += OnDrawRewardSystemListElement;
            rewardSystemList.onAddDropdownCallback += OnRewardSystemListAddDropdown;
            rewardSystemList.onRemoveCallback += OnRewardSystemListRemove;
        }

        public override void OnInspectorGUI()
        {
#if DEBUG_QUEST_EDITOR
            defaultInspectorFoldout = EditorGUILayout.Foldout(defaultInspectorFoldout, "Default Inspector");
            if (defaultInspectorFoldout) base.OnInspectorGUI();
#endif
            mustExitGUI = false;

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_entityType"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_questGroup"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_domainType"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_domains"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_goalSelectionMode"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_generateQuestOnStart"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_maxQuestsToGenerate"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_requireReturnToComplete"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_generateAbandonableQuests"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_rewardsUIContents"), true);
            rewardSystemList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            DrawGenerateQuestButton();

            if (mustExitGUI)
            {
                EditorGUIUtility.ExitGUI(); // Prevents Inspector from trying to draw just-removed component.
            }
        }

        protected void DrawGenerateQuestButton()
        {
            var questGeneratorEntity = target as QuestGeneratorEntity;
            if (Application.isPlaying && questGeneratorEntity != null)
            {
                EditorGUI.BeginDisabledGroup(!(questGeneratorEntity.gameObject.activeInHierarchy && questGeneratorEntity.enabled));
                try
                {
                    if (GUILayout.Button("Generate Quest"))
                    {
                        questGeneratorEntity.GenerateQuest();
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        private void OnDrawRewardSystemListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Reward Systems (applied in this order)");
        }

        private void OnDrawRewardSystemListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!(0 <= index && index < rewardSystemList.count)) return;
            var element = rewardSystemList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);
        }

        private void OnRewardSystemListRemove(ReorderableList list)
        {
            var element = (0 <= list.index && list.index < list.count) ? list.serializedProperty.GetArrayElementAtIndex(list.index) : null;
            var rewardSystem = (element != null) ? (element.objectReferenceValue as RewardSystem) : null;
            var removeComponent = false;
            if (rewardSystem != null)
            {
                var rewardSystemName = rewardSystem.GetType().Name;
                var option = EditorUtility.DisplayDialogComplex("Remove Reward System",
                    "Do you want to remove the " + rewardSystemName + " only from this list or remove the entire component from the GameObject?",
                    "Remove From List", "Remove Component", "Cancel");
                if (option == 2) return; // Cancel.
                removeComponent = (option == 1);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            if (removeComponent)
            {
                DestroyImmediate(rewardSystem);
                mustExitGUI = true;
            }
        }

        private void OnRewardSystemListAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Refresh From Components"), false, OnRefreshRewardSystemList);
            menu.AddSeparator(string.Empty);
            var subtypes = QuestEditorUtility.GetSubtypes<RewardSystem>();
            for (int i = 0; i < subtypes.Count; i++)
            {
                var subtype = subtypes[i];
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(subtype.Name)), false, OnAddRewardSystemType, subtype);
            }
            menu.ShowAsContext();
        }

        private void OnRefreshRewardSystemList()
        {
            var questGeneratorEntity = target as QuestGeneratorEntity;
            Undo.RecordObject(questGeneratorEntity.gameObject, "Add Reward System");
            var rewardSystems = questGeneratorEntity.rewardSystems;
            rewardSystems.RemoveAll(x => x == null);
            foreach (var rewardSystem in questGeneratorEntity.GetComponentsInChildren<RewardSystem>())
            {
                if (!rewardSystems.Contains(rewardSystem) && rewardSystem.enabled)
                {
                    rewardSystems.Add(rewardSystem);
                }
            }
            EditorUtility.SetDirty(questGeneratorEntity);
        }

        private void OnAddRewardSystemType(object data)
        {
            var questGeneratorEntity = target as QuestGeneratorEntity;
            Undo.RecordObject(questGeneratorEntity.gameObject, "Add Reward System");
            var type = data as Type;
            questGeneratorEntity.rewardSystems.Add(questGeneratorEntity.gameObject.AddComponent(type) as RewardSystem);
            EditorUtility.SetDirty(questGeneratorEntity);
        }
    }
}
