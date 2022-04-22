// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Custom editor for FactionMember.
    /// </summary>
    [CustomEditor(typeof(FactionMember), true)]
    [CanEditMultipleObjects]
    public class FactionMemberEditor : Editor
    {

        private FactionMember m_member;
        private int[] m_factionIDList;
        private string[] m_factionNameList;
        private ReorderableList m_parentList = null;
        private ReorderableList m_relationshipList = null;
        private ReorderableList m_traitList = null;
        private ReorderableList m_shortTermMemoryList = null;
        private ReorderableList m_longTermMemoryList = null;
        private ReorderableList m_inheritedRelationshipsList = null;
        private List<InheritedRelationship> m_inheritedRelationships = null;
        private Faction myFaction = null;

        private static bool showInheritedRelationships = false;

        private void OnEnable()
        {
            m_member = target as FactionMember;
            if (m_member == null) return;
            m_member.FindResources();
            UpdateFactionList();
            InitReadonlyInfo();
        }

        private void UpdateFactionList()
        {
            var idList = new List<int>();
            var nameList = new List<string>();
            if (m_member.factionDatabase != null)
            {
                for (int i = 0; i < m_member.factionDatabase.factions.Length; i++)
                {
                    var faction = m_member.factionDatabase.factions[i];
                    idList.Add(faction.id);
                    nameList.Add(faction.name);
                }
            }
            m_factionIDList = idList.ToArray();
            m_factionNameList = nameList.ToArray();
        }

        private int GetFactionListIndex()
        {
            if (m_member.factionDatabase != null)
            {
                for (int i = 0; i < m_factionIDList.Length; i++)
                {
                    if (m_member.factionID == m_factionIDList[i])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        #region Inspector GUI

        public override void OnInspectorGUI()
        {
            if (m_member == null) return;
            Undo.RecordObject(target, "FactionMember");
            DrawCustomGUI();
        }

        private void DrawCustomGUI()
        {
            var newManager = EditorGUILayout.ObjectField(new GUIContent("Faction Manager"), m_member.factionManager, typeof(FactionManager), true) as FactionManager;
            if (newManager != m_member.factionManager)
            {
                m_member.factionManager = newManager;
                if (newManager != null) m_member.factionDatabase = newManager.factionDatabase;
            }
            var newDatabase = EditorGUILayout.ObjectField(new GUIContent("Faction Database"), m_member.factionDatabase, typeof(FactionDatabase), true) as FactionDatabase;
            if (newDatabase != m_member.factionDatabase)
            {
                m_member.factionDatabase = newDatabase;
                UpdateFactionList();
            }
            var factionListIndex = GetFactionListIndex();
            var newFactionListIndex = EditorGUILayout.Popup("Faction", factionListIndex, m_factionNameList);
            if (newFactionListIndex != factionListIndex)
            {
                m_member.factionID = m_factionIDList[newFactionListIndex];
            }

            serializedObject.Update();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pad"), true);
            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();

            m_member.eyes = EditorGUILayout.ObjectField(new GUIContent("Eyes", "If set, perform witness visibility raycasts from this transform instead of GameObject's transform."), m_member.eyes, typeof(Transform), true) as Transform;
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sightLayerMask"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("impressionability"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("traitAlignmentImportance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("arousalImportance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("deedImpactThreshold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("acclimatizationCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("powerDifferenceCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxMemories"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shortTermMemoryDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("longTermMemoryDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("memoryCleanupFrequency"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sortMemories"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("debugEvalFunc"));
            serializedObject.ApplyModifiedProperties();

            DrawReadonlyInfo();
        }

        private void InitReadonlyInfo()
        {
            myFaction = GetMyFaction();
            InitParentList();
            InitRelationshipLists();

            InitMemoryLists();
            InitTraitList();
        }

        private void DrawReadonlyInfo()
        {
            if (GetMyFaction() != myFaction)
            {
                InitReadonlyInfo();
            }
            DrawTraitList();
            DrawParentList();
            DrawRelationshipList();
            if (Application.isPlaying)
            {
                DrawMemoryLists();
            }
        }

        private Faction GetFaction(int factionID)
        {
            if (m_member == null || m_member.factionManager == null || m_member.factionManager.factionDatabase == null) return null;
            return m_member.factionManager.factionDatabase.GetFaction(factionID);
        }

        private Faction GetMyFaction()
        {
            return (m_member == null) ? null : GetFaction(m_member.factionID);
        }

        private void InitParentList()
        {
            if (myFaction == null || myFaction.parents == null) return;
            m_parentList = new ReorderableList(myFaction.parents, typeof(int), false, true, false, false);
            m_parentList.drawHeaderCallback += OnDrawParentListHeader;
            m_parentList.drawElementCallback += OnDrawParentListItem;
        }

        private void DrawParentList()
        {
            if (m_parentList == null) return;
            EditorGUI.BeginDisabledGroup(true);
            m_parentList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
        }

        private void OnDrawParentListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Parents");
        }

        private void OnDrawParentListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (myFaction == null || myFaction.parents == null) return;
            if (!(0 <= index && index < myFaction.parents.Length)) return;
            var parent = GetFaction(myFaction.parents[index]);
            if (parent == null) return;
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            EditorGUI.TextField(rect, parent.name);
        }

        private void InitRelationshipLists()
        {
            if (myFaction == null || myFaction.relationships == null) return;
            m_relationshipList = new ReorderableList(myFaction.relationships, typeof(Relationship), false, true, false, false);
            m_relationshipList.drawHeaderCallback += OnDrawRelationshipListHeader;
            m_relationshipList.drawElementCallback += OnDrawRelationshipListItem;
            m_inheritedRelationshipsList = null;
            m_inheritedRelationships = null;
        }

        private void InitTraitList()
        {
            if (myFaction == null) return;
            m_traitList = new ReorderableList(myFaction.traits, typeof(float), false, true, false, false);
            m_traitList.drawHeaderCallback += OnDrawTraitsListHeader;
            m_traitList.drawElementCallback += OnDrawTraitsListItem;
        }

        private void DrawTraitList()
        {
            if (m_member.factionDatabase == null) return;
            if (m_traitList == null) InitTraitList();
            if (m_traitList == null) return;
            m_traitList.DoLayoutList();
        }

        private void OnDrawTraitsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Traits");
        }
        private void OnDrawTraitsListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (myFaction == null) return;
            if (!(0 <= index && index < myFaction.traits.Length)) return;

            if (!(0 <= index && index < m_member.factionDatabase.personalityTraitDefinitions.Length)) return;
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = Mathf.Clamp(rect.width / 4, 80, 200);
            var valueWidth = rect.width - nameWidth - 4;
            var labelRect = new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.TextField(labelRect, m_member.factionDatabase.personalityTraitDefinitions[index].name);
            myFaction.traits[index] = EditorGUI.Slider(fieldRect, GUIContent.none, myFaction.traits[index], -100, 100);
        }

        private void DrawRelationshipList()
        {
            if (m_relationshipList == null) return;
            EditorGUI.BeginDisabledGroup(true);
            m_relationshipList.DoLayoutList();
            EditorGUI.EndDisabledGroup();

            DrawInheritedRelationships();
        }

        private void OnDrawRelationshipListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Relationships");
        }

        private void OnDrawRelationshipListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (myFaction == null || myFaction.relationships == null) return;
            if (!(0 <= index && index < myFaction.relationships.Count)) return;
            var relationship = myFaction.relationships[index];
            if (relationship == null) return;
            var subject = GetFaction(relationship.factionID);
            if (subject == null) return;
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = Mathf.Clamp(rect.width / 4, 80, 200);
            var valueWidth = rect.width - nameWidth - 4;
            var labelRect = new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.TextField(labelRect, subject.name);
            relationship.affinity = EditorGUI.Slider(fieldRect, GUIContent.none, relationship.affinity, -100, 100);
        }

        private void DrawInheritedRelationships()
        {
            EditorGUILayout.BeginHorizontal();
            showInheritedRelationships = EditorGUILayout.ToggleLeft(new GUIContent("Also Show Inherited Relationships", "Also show relationships inherited from parents and ancestors."), showInheritedRelationships);
            if (showInheritedRelationships)
            {
                if (GUILayout.Button("Refresh", GUILayout.Width(64)))
                {
                    m_inheritedRelationshipsList = null;
                }
            }
            EditorGUILayout.EndHorizontal();
            if (showInheritedRelationships)
            {
                if (m_inheritedRelationships == null || m_inheritedRelationshipsList == null)
                {
                    SetupInheritedRelationshipList();
                }
                EditorGUI.BeginDisabledGroup(true);
                m_inheritedRelationshipsList.DoLayoutList();
                EditorGUI.EndDisabledGroup();
            }
        }

        private void SetupInheritedRelationshipList()
        {
            serializedObject.ApplyModifiedProperties();
            m_inheritedRelationships = InheritedRelationship.GetInheritedRelationships(m_member.factionDatabase, m_member.factionID);
            m_inheritedRelationshipsList = new ReorderableList(m_inheritedRelationships, typeof(Relationship), false, true, false, false);
            m_inheritedRelationshipsList.drawHeaderCallback = OnDrawInheritedRelationshipsListHeader;
            m_inheritedRelationshipsList.drawElementCallback = OnDrawInheritedRelationshipsListElement;
        }

        private void OnDrawInheritedRelationshipsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Inherited Relationships");
        }

        private void OnDrawInheritedRelationshipsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = rect.width / 2;
            var valueWidth = rect.width - nameWidth - 4;
            EditorGUI.TextField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                m_inheritedRelationships[index].name);
            EditorGUI.Slider(
                new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
                GUIContent.none, m_inheritedRelationships[index].affinity, -100, 100);
        }

        private void InitMemoryLists()
        {
            if (m_member == null) return;
            m_shortTermMemoryList = new ReorderableList(m_member.shortTermMemory, typeof(Rumor), false, true, false, false);
            m_shortTermMemoryList.drawHeaderCallback += OnDrawShortTermMemoryListHeader;
            m_shortTermMemoryList.drawElementCallback += OnDrawShortTermMemoryListItem;
            m_longTermMemoryList = new ReorderableList(m_member.longTermMemory, typeof(Rumor), false, true, false, false);
            m_longTermMemoryList.drawHeaderCallback += OnDrawLongTermMemoryListHeader;
            m_longTermMemoryList.drawElementCallback += OnDrawLongTermMemoryListItem;
        }

        private void DrawMemoryLists()
        {
            if (m_shortTermMemoryList == null || m_longTermMemoryList == null) return;
            EditorGUI.BeginDisabledGroup(true);
            m_shortTermMemoryList.DoLayoutList();
            m_longTermMemoryList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
        }

        private void OnDrawShortTermMemoryListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Short Term Memory");
        }

        private void OnDrawLongTermMemoryListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Long Term Memory");
        }

        private void OnDrawShortTermMemoryListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawMemoryListItem(rect, index, isActive, isFocused, m_member.shortTermMemory);
        }

        private void OnDrawLongTermMemoryListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawMemoryListItem(rect, index, isActive, isFocused, m_member.longTermMemory);
        }

        private void DrawMemoryListItem(Rect rect, int index, bool isActive, bool isFocused, List<Rumor> memory)
        {
            if (memory == null) return;
            if (!(0 <= index && index < memory.Count)) return;
            var rumor = memory[index];
            var actor = m_member.factionManager.GetFactionSilent(rumor.actorFactionID);
            var target = m_member.factionManager.GetFactionSilent(rumor.targetFactionID);
            if (actor == null || target == null) return;
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var fieldWidth = (rect.width - 6) / 7;
            var actorRect = new Rect(rect.x, rect.y, 2 * fieldWidth, EditorGUIUtility.singleLineHeight);
            var tagRect = new Rect(rect.x + (2 * fieldWidth) + 2, rect.y, 2 * fieldWidth, EditorGUIUtility.singleLineHeight);
            var targetRect = new Rect(rect.x + (4 * fieldWidth) + 4, rect.y, 2 * fieldWidth, EditorGUIUtility.singleLineHeight);
            var countRect = new Rect(rect.x + (6 * fieldWidth) + 6, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.TextField(actorRect, actor.name);
            EditorGUI.TextField(tagRect, rumor.tag);
            EditorGUI.TextField(targetRect, target.name);
            if (rumor.count > 0)
            {
                EditorGUI.IntField(countRect, rumor.count);
            }
        }

        #endregion

    }

}
