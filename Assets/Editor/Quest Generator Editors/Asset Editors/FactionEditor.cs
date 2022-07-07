// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace PixelCrushers.QuestMachine
{

    [CustomEditor(typeof(Faction), true)]
    public class FactionEditor : Editor
    {

        private ReorderableList m_list = null;

        private void OnEnable()
        {
            SetupList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Factions define relationships between entities. Each relationship has an affinity value in the range [-100,+100] " + 
                "that indicates how negatively or positively the faction feels toward the other. Quest givers' relationships influence the quests that the " +
                "quest giver generates. For example, a quest giver might generate an 'attack' quest targeting an enemy with negative affinity, or it " +
                "might generate a 'heal' quest targeting a friend with positive affinity. You can assign a faction to an entity by inspecting its " +
                "entity type.", MessageType.None);
            serializedObject.Update();
            m_list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void SetupList()
        {
            m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("m_relationships"), true, true, true, true);
            m_list.drawHeaderCallback += OnDrawHeader;
            m_list.drawElementCallback += OnDrawElement;
            //--- Beta testers prefer manual assignment: m_list.onAddDropdownCallback += OnAddDropdown;
        }

        private void OnDrawHeader(Rect rect)
        {
            var fieldWidth = (rect.width - 12) / 2;
            EditorGUI.LabelField(new Rect(12 + rect.x, rect.y, fieldWidth, rect.height), new GUIContent("Other Faction", "The other faction for whom this faction holds a negative or positive feeling."));
            EditorGUI.LabelField(new Rect(12 + rect.x + fieldWidth, rect.y, fieldWidth, rect.height), new GUIContent("Affinity", "The degree of negative or positive feeling for the faction, in the range [-100,+100]."));
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var relationshipsProperty = serializedObject.FindProperty("m_relationships");
            if (relationshipsProperty == null || !(0 <= index && index < relationshipsProperty.arraySize)) return;
            var relationshipProperty = relationshipsProperty.GetArrayElementAtIndex(index);
            if (relationshipProperty == null) return;
            var factionProperty = relationshipProperty.FindPropertyRelative("m_faction");
            var affinityProperty = relationshipProperty.FindPropertyRelative("m_affinity");
            if (factionProperty == null || affinityProperty == null) return;
            var fieldWidth = rect.width / 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), factionProperty, GUIContent.none, true);
            EditorGUI.PropertyField(new Rect(rect.x + fieldWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), affinityProperty, GUIContent.none, true);
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var factionList = AssetInfoLists.GetList(typeof(Faction));
            if (factionList == null) return;
            for (int i = 0; i < factionList.Count; i++)
            {
                if (factionList[i] == null) continue;
                var faction = EditorUtility.InstanceIDToObject(factionList[i].instanceID) as Faction;
                if (faction == null) continue;
                var isInList = (target as Faction).relationships.Find(x => x != null && x.faction == faction) != null;
                if (isInList) continue;
                menu.AddItem(new GUIContent(factionList[i].pathAndName), false, OnClickAdd, faction);
            }
            menu.ShowAsContext();
        }

        private void OnClickAdd(object data)
        {
            var faction = data as Faction;
            if (faction == null) return;
            serializedObject.Update();
            var relationshipsProperty = serializedObject.FindProperty("m_relationships");
            if (relationshipsProperty == null) return;
            relationshipsProperty.arraySize++;
            var relationshipProperty = relationshipsProperty.GetArrayElementAtIndex(relationshipsProperty.arraySize - 1);
            if (relationshipProperty == null) return;
            relationshipProperty.FindPropertyRelative("m_faction").objectReferenceValue = faction;
            serializedObject.ApplyModifiedProperties();
        }

    }

}