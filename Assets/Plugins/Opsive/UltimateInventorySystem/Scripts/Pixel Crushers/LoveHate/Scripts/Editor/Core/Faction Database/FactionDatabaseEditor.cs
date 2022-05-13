// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for FactionDatabase.
    /// </summary>
    [CustomEditor(typeof(FactionDatabase), true)]
    public class FactionDatabaseEditor : Editor
    {

        protected class FactionDatabaseFoldouts
        {
            public bool personalityTraitDefsFoldout = false;
            public bool relationshipTraitDefsFoldout = false;
            public bool presetsFoldout = false;
            public bool factionsFoldout = false;
            public bool inheritedRelationshipsFoldout = false;
        }

        protected static Dictionary<UnityEngine.Object, FactionDatabaseFoldouts> foldouts = new Dictionary<UnityEngine.Object, FactionDatabaseFoldouts>();

        protected ReorderableList m_personalityTraitDefList;
        protected ReorderableList m_relationshipTraitDefList;
        protected ReorderableList m_presetList;
        protected ReorderableList m_presetTraitList;
        protected ReorderableList m_factionList;
        protected ReorderableList m_factionParentList;
        protected ReorderableList m_factionTraitList;
        protected ReorderableList m_factionRelationshipList;
        protected ReorderableList m_factionRelationshipTraitList;
        protected ReorderableList m_factionInheritedRelationshipsList;
        protected int m_personalityTraitDefIndex;
        protected int m_relationshipTraitDefIndex;
        protected List<InheritedRelationship> m_inheritedRelationships;

        protected virtual void OnEnable()
        {
            if (target == null) return;
            SetupPersonalityTraitDefList();
            SetupRelationshipTraitDefList();
            SetupPresetList();
            SetupFactionList();
        }

        #region Inspector GUI

        public override void OnInspectorGUI()
        {
#if EVALUATION_VERSION
            if (GUILayout.Button("Buy Now...")) Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/33063");
#endif

            //--- For comparison with default inspector:
            //base.OnInspectorGUI();
            //EditorGUILayout.Separator();

            if (target == null) return;
            Undo.RecordObject(target, "FactionDatabase");
            DrawCustomGUI();
        }

        protected virtual void DrawCustomGUI()
        {
            CheckFoldouts();
            serializedObject.Update();
            DrawPersonalityTraitDefSection();
            DrawRelationshipTraitDefSection();
            DrawPresetsSection();
            DrawFactionsSection();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void CheckFoldouts()
        {
            if (!foldouts.ContainsKey(target))
            {
                foldouts.Add(target, new FactionDatabaseFoldouts());
            }
        }

        #endregion

        #region Personality Trait Definition List

        protected virtual void SetupPersonalityTraitDefList()
        {
            m_personalityTraitDefList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("personalityTraitDefinitions"),
                true, true, true, true);
            m_personalityTraitDefList.drawHeaderCallback = OnDrawPersonalityTraitDefListHeader;
            m_personalityTraitDefList.drawElementCallback = OnDrawPersonalityTraitDefListElement;
            m_personalityTraitDefList.onAddCallback = OnAddPersonalityTraitDef;
            m_personalityTraitDefList.onRemoveCallback = OnRemovePersonalityTraitDef;
            m_personalityTraitDefList.onSelectCallback = OnSelectPersonalityTraitDef;
            m_personalityTraitDefList.onReorderCallback = OnReorderPersonalityTraitDef;
        }

        protected virtual void DrawPersonalityTraitDefSection()
        {
            foldouts[target].personalityTraitDefsFoldout = EditorGUILayout.Foldout(foldouts[target].personalityTraitDefsFoldout, "Personality Traits");
            if (foldouts[target].personalityTraitDefsFoldout)
            {
                m_personalityTraitDefList.DoLayoutList();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("traitInheritanceType"));
            }
        }

        protected virtual void OnDrawPersonalityTraitDefListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Personality Traits");
        }

        protected virtual void OnDrawPersonalityTraitDefListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_personalityTraitDefList != null && 0 <= index && index <= m_personalityTraitDefList.serializedProperty.arraySize)
            {
                var element = m_personalityTraitDefList.serializedProperty.GetArrayElementAtIndex(index);
                DrawNameDescriptionListElement(rect, index, isActive, isFocused, true, element);
            }
        }

        protected virtual void DrawNameDescriptionListElement(Rect rect, int index, bool isActive, bool isFocused, bool isEditable, SerializedProperty element)
        {
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var descriptionWidth = rect.width - nameWidth - 4;
            if (isEditable)
            {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name"), GUIContent.none);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.TextField(
                    new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name").stringValue);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - descriptionWidth, rect.y, descriptionWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("description"), GUIContent.none);
        }

        protected float GetDefaultNameWidth(Rect rect)
        {
            return Mathf.Clamp(rect.width / 4, 80, 200);
        }

        protected virtual void OnAddPersonalityTraitDef(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = string.Empty;
            element.FindPropertyRelative("description").stringValue = string.Empty;
            NotifyPresetsAddTrait();
            NotifyFactionsAddTrait();
        }

        protected virtual void OnRemovePersonalityTraitDef(ReorderableList list)
        {
            var traitName = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("name").stringValue;
            if (EditorUtility.DisplayDialog("Delete selected personality trait?", traitName, "Delete", "Cancel"))
            {
                NotifyPresetsRemoveTrait(list.index);
                NotifyFactionsRemoveTrait(list.index);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }

        protected virtual void OnSelectPersonalityTraitDef(ReorderableList list)
        {
            m_personalityTraitDefIndex = list.index;
        }

        protected virtual void OnReorderPersonalityTraitDef(ReorderableList list)
        {
            NotifyPresetsReorderTrait(m_personalityTraitDefIndex, list.index);
            NotifyFactionsReorderTrait(m_personalityTraitDefIndex, list.index);
        }

        #endregion

        #region Relationship Trait Definition List

        protected const string AffinityTraitName = "Affinity";

        protected virtual void SetupRelationshipTraitDefList()
        {
            m_relationshipTraitDefList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("relationshipTraitDefinitions"),
                true, true, true, true);
            m_relationshipTraitDefList.drawHeaderCallback = OnDrawRelationshipTraitDefListHeader;
            m_relationshipTraitDefList.drawElementCallback = OnDrawRelationshipTraitDefListElement;
            m_relationshipTraitDefList.onAddCallback = OnAddRelationshipTraitDef;
            m_relationshipTraitDefList.onRemoveCallback = OnRemoveRelationshipTraitDef;
            m_relationshipTraitDefList.onSelectCallback = OnSelectRelationshipTraitDef;
            m_relationshipTraitDefList.onReorderCallback = OnReorderRelationshipTraitDef;
        }

        protected virtual void DrawRelationshipTraitDefSection()
        {
            foldouts[target].relationshipTraitDefsFoldout = EditorGUILayout.Foldout(foldouts[target].relationshipTraitDefsFoldout, "Relationship Traits");
            if (foldouts[target].relationshipTraitDefsFoldout)
            {
                m_relationshipTraitDefList.DoLayoutList();
                DrawRelationshipInheritanceTypeDropdown();
            }
        }

        protected virtual void OnDrawRelationshipTraitDefListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Relationship Traits");
        }

        protected virtual void OnDrawRelationshipTraitDefListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_relationshipTraitDefList.serializedProperty.GetArrayElementAtIndex(index);
            DrawNameDescriptionListElement(rect, index, isActive, isFocused, index > 0, element);
        }

        protected virtual void OnAddRelationshipTraitDef(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = string.Empty;
            element.FindPropertyRelative("description").stringValue = string.Empty;
            NotifyRelationshipsAddTrait();
        }

        protected virtual void OnRemoveRelationshipTraitDef(ReorderableList list)
        {
            var traitName = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("name").stringValue;
            if (string.Equals(traitName, AffinityTraitName)) return;
            if (EditorUtility.DisplayDialog("Delete selected relationship trait?", traitName, "Delete", "Cancel"))
            {
                NotifyRelationshipsRemoveTrait(list.index);
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
            }
        }

        protected virtual void OnSelectRelationshipTraitDef(ReorderableList list)
        {
            m_relationshipTraitDefIndex = list.index;
        }

        protected virtual void OnReorderRelationshipTraitDef(ReorderableList list)
        {
            if (list.serializedProperty.arraySize < 1) return;
            NotifyRelationshipsReorderTrait(m_relationshipTraitDefIndex, list.index);

            // Make sure Affinity is always at top:
            var firstName = list.serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue;
            if (!string.Equals(firstName, AffinityTraitName))
            {
                int affinityIndex = -1;
                string affinityDescription = string.Empty;
                for (int i = 0; i < list.serializedProperty.arraySize; i++)
                {
                    var iName = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                    if (string.Equals(iName, AffinityTraitName))
                    {
                        affinityIndex = i;
                        affinityDescription = list.serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("description").stringValue;
                        break;
                    }
                }
                if (affinityIndex != -1)
                {
                    list.serializedProperty.DeleteArrayElementAtIndex(affinityIndex);
                    list.serializedProperty.InsertArrayElementAtIndex(0);
                    list.serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = AffinityTraitName;
                    list.serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("description").stringValue = affinityDescription;
                    NotifyRelationshipsReorderTrait(affinityIndex, 0);
                }
            }
        }

        protected virtual void DrawRelationshipInheritanceTypeDropdown()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("relationshipInheritanceType"));
            var changed = EditorGUI.EndChangeCheck();
            if (changed && m_factionInheritedRelationshipsList != null)
            {
                SetupInheritedRelationshipList();
            }

        }

        #endregion

        #region Preset List

        protected virtual void SetupPresetList()
        {
            m_presetList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("presets"),
                true, true, true, true);
            m_presetList.drawHeaderCallback = OnDrawPresetListHeader;
            m_presetList.drawElementCallback = OnDrawPresetListElement;
            m_presetList.onAddCallback = OnAddPreset;
            m_presetList.onRemoveCallback = OnRemovePreset;
            m_presetList.onSelectCallback = OnSelectPreset;
            m_presetTraitList = null;
        }

        protected virtual void DrawPresetsSection()
        {
            foldouts[target].presetsFoldout = EditorGUILayout.Foldout(foldouts[target].presetsFoldout, "Presets");
            if (foldouts[target].presetsFoldout)
            {
                m_presetList.DoLayoutList();
                if (m_presetTraitList == null)
                {
                    if (m_presetList.serializedProperty.arraySize > 0)
                    {
                        EditorGUILayout.HelpBox("Click the double bars to the left of a preset's name to edit its traits.", MessageType.None);
                    }
                }
                else
                {
                    m_presetTraitList.DoLayoutList();
                }
            }
        }

        protected virtual void OnDrawPresetListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Presets");
        }

        protected virtual void OnDrawPresetListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_presetList.serializedProperty.GetArrayElementAtIndex(index);
            DrawNameDescriptionListElement(rect, index, isActive, isFocused, true, element);
        }

        protected virtual void OnAddPreset(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = string.Empty;
            element.FindPropertyRelative("description").stringValue = string.Empty;
            var values = element.FindPropertyRelative("traits");
            values.arraySize = m_personalityTraitDefList.serializedProperty.arraySize;
            for (int j = 0; j < m_personalityTraitDefList.serializedProperty.arraySize; j++)
            {
                values.GetArrayElementAtIndex(j).floatValue = 0;
            }
            SetupPresetTraitList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        protected virtual void OnRemovePreset(ReorderableList list)
        {
            var presetName = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("name").stringValue;
            if (EditorUtility.DisplayDialog("Delete selected preset?", presetName, "Delete", "Cancel"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                m_presetTraitList = null;
            }
        }

        protected virtual void OnSelectPreset(ReorderableList list)
        {
            SetupPresetTraitList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        protected virtual void NotifyPresetsAddTrait()
        {
            var traitDefs = serializedObject.FindProperty("personalityTraitDefinitions");
            var presets = serializedObject.FindProperty("presets");
            for (int i = 0; i < presets.arraySize; i++)
            {
                var preset = presets.GetArrayElementAtIndex(i);
                preset.FindPropertyRelative("traits").arraySize = traitDefs.arraySize;
            }
        }

        protected virtual void NotifyPresetsRemoveTrait(int index)
        {
            var presets = serializedObject.FindProperty("presets");
            for (int i = 0; i < presets.arraySize; i++)
            {
                var preset = presets.GetArrayElementAtIndex(i);
                preset.FindPropertyRelative("traits").DeleteArrayElementAtIndex(index);
            }
        }

        protected virtual void NotifyPresetsReorderTrait(int oldIndex, int newIndex)
        {
            var presets = serializedObject.FindProperty("presets");
            for (int i = 0; i < presets.arraySize; i++)
            {
                var preset = presets.GetArrayElementAtIndex(i);
                var values = preset.FindPropertyRelative("traits");
                var value = values.GetArrayElementAtIndex(oldIndex).floatValue;
                values.DeleteArrayElementAtIndex(oldIndex);
                values.InsertArrayElementAtIndex(newIndex);
                values.GetArrayElementAtIndex(newIndex).floatValue = value;
            }
        }

        #endregion

        #region Preset Trait List

        protected virtual void SetupPresetTraitList(SerializedProperty preset)
        {
            m_presetTraitList = new ReorderableList(
                serializedObject, preset.FindPropertyRelative("traits"),
                false, true, false, false);
            m_presetTraitList.drawHeaderCallback = OnDrawPresetTraitListHeader;
            m_presetTraitList.drawElementCallback = OnDrawPresetTraitListElement;
        }

        protected virtual void OnDrawPresetTraitListHeader(Rect rect)
        {
            var presetName = m_presetList.serializedProperty.GetArrayElementAtIndex(m_presetList.index).FindPropertyRelative("name").stringValue;
            EditorGUI.LabelField(rect, "Preset: " + presetName);
        }

        protected virtual void OnDrawPresetTraitListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_presetTraitList.serializedProperty.GetArrayElementAtIndex(index);
            DrawPersonalityTraitListElement(rect, index, isActive, isFocused, element);
        }

        protected virtual void DrawPersonalityTraitListElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var valueWidth = rect.width - nameWidth - 4;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                m_personalityTraitDefList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue);
            EditorGUI.EndDisabledGroup();
            EditorGUI.Slider(
                new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
                element, -100, 100, GUIContent.none);
        }

        #endregion

        #region Faction List

        protected virtual void SetupFactionList()
        {
            m_factionList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("factions"),
                true, true, true, true);
            m_factionList.drawHeaderCallback = OnDrawFactionListHeader;
            m_factionList.drawElementCallback = OnDrawFactionListElement;
            m_factionList.onAddCallback = OnAddFaction;
            m_factionList.onRemoveCallback = OnRemoveFaction;
            m_factionList.onSelectCallback = OnSelectFaction;
            SetupFactionEditList();
        }

        protected virtual void DrawFactionsSection()
        {
            foldouts[target].factionsFoldout = EditorGUILayout.Foldout(foldouts[target].factionsFoldout, "Factions");
            if (foldouts[target].factionsFoldout)
            {
                m_factionList.DoLayoutList();
                if (m_factionTraitList == null)
                {
                    if (m_factionList.serializedProperty.arraySize > 0)
                    {
                        EditorGUILayout.HelpBox("Click the double bars to the left of a faction's name to edit it.", MessageType.None);
                    }
                }
                else
                {
                    DrawFactionEditSection();
                }
            }
        }

        protected virtual void OnDrawFactionListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Factions");
        }

        protected virtual void OnDrawFactionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_factionList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var colorWidth = 56;
            var descriptionWidth = rect.width - nameWidth - colorWidth - 6;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - descriptionWidth - colorWidth - 2, rect.y, descriptionWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("description"), GUIContent.none);
            var color = element.FindPropertyRelative("color");
            color.intValue = EditorGUI.Popup(
                new Rect(rect.x + rect.width - colorWidth, rect.y, colorWidth, EditorGUIUtility.singleLineHeight),
                color.intValue, Faction.ColorNames);

        }

        protected virtual void OnAddFaction(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            InitializeFactionElement(element, string.Empty, string.Empty);
            SetupFactionEditList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        protected virtual void InitializeFactionElement(SerializedProperty element, string name, string description)
        {
            var id = serializedObject.FindProperty("nextID").intValue;
            serializedObject.FindProperty("nextID").intValue++;
            element.FindPropertyRelative("id").intValue = id;
            element.FindPropertyRelative("name").stringValue = string.Empty;
            element.FindPropertyRelative("description").stringValue = string.Empty;
            var values = element.FindPropertyRelative("traits");
            values.arraySize = m_personalityTraitDefList.serializedProperty.arraySize;
            for (int j = 0; j < m_personalityTraitDefList.serializedProperty.arraySize; j++)
            {
                values.GetArrayElementAtIndex(j).floatValue = 0;
            }
            element.FindPropertyRelative("parents").arraySize = 0;
            element.FindPropertyRelative("relationships").arraySize = 0;
        }

        protected virtual void OnRemoveFaction(ReorderableList list)
        {
            var factionName = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("name").stringValue;
            if (EditorUtility.DisplayDialog("Delete selected faction?", factionName, "Delete", "Cancel"))
            {
                RemoveFaction(list);
            }
        }

        protected virtual void RemoveFaction(ReorderableList list)
        {
            var faction = serializedObject.FindProperty("factions").GetArrayElementAtIndex(list.index);
            if (faction != null)
            {
                var factionID = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index).FindPropertyRelative("id").intValue;
                NotifyFactionsRemoveFaction(factionID);
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            m_factionTraitList = null;
        }

        protected virtual void OnSelectFaction(ReorderableList list)
        {
            SetupFactionEditList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        protected virtual void NotifyFactionsAddTrait()
        {
            var traits = serializedObject.FindProperty("personalityTraitDefinitions");
            var factions = serializedObject.FindProperty("factions");
            for (int i = 0; i < factions.arraySize; i++)
            {
                var faction = factions.GetArrayElementAtIndex(i);
                faction.FindPropertyRelative("traits").arraySize = traits.arraySize;
            }
        }

        protected virtual void NotifyFactionsRemoveTrait(int index)
        {
            var factions = serializedObject.FindProperty("factions");
            for (int i = 0; i < factions.arraySize; i++)
            {
                var faction = factions.GetArrayElementAtIndex(i);
                faction.FindPropertyRelative("traits").DeleteArrayElementAtIndex(index);
            }
        }

        protected virtual void NotifyFactionsReorderTrait(int oldIndex, int newIndex)
        {
            var factions = serializedObject.FindProperty("factions");
            for (int i = 0; i < factions.arraySize; i++)
            {
                var faction = factions.GetArrayElementAtIndex(i);
                var values = faction.FindPropertyRelative("traits");
                var value = values.GetArrayElementAtIndex(oldIndex).floatValue;
                values.DeleteArrayElementAtIndex(oldIndex);
                values.InsertArrayElementAtIndex(newIndex);
                values.GetArrayElementAtIndex(newIndex).floatValue = value;
            }
        }

        protected virtual void NotifyFactionsRemoveFaction(int index)
        {
            var factions = serializedObject.FindProperty("factions");
            if (factions == null) return;
            for (int i = 0; i < factions.arraySize; i++)
            {
                var faction = factions.GetArrayElementAtIndex(i);
                NotifyParentsRemoveFaction(faction.FindPropertyRelative("parents"), index);
                NotifyRelationshipsRemoveFaction(faction.FindPropertyRelative("relationships"), index);
            }
        }

        protected virtual void NotifyParentsRemoveFaction(SerializedProperty parents, int index)
        {
            if (parents == null) return;
            var arraySize = parents.arraySize;
            if (arraySize <= 0) return;
            for (int i = arraySize - 1; i >= 0; i--)
            {
                if (parents.GetArrayElementAtIndex(i).intValue == index)
                {
                    parents.DeleteArrayElementAtIndex(i);
                }
            }
        }

        protected virtual void NotifyRelationshipsRemoveFaction(SerializedProperty relationships, int index)
        {
            if (relationships == null) return;
            var arraySize = relationships.arraySize;
            if (arraySize <= 0) return;
            for (int i = arraySize - 1; i >= 0; i--)
            {
                var relationship = relationships.GetArrayElementAtIndex(i);
                if (relationship.FindPropertyRelative("factionID").intValue == index)
                {
                    relationships.DeleteArrayElementAtIndex(i);
                }
            }
        }

        #endregion

        #region Faction Edit Section

        protected virtual void SetupFactionEditList()
        {
            m_factionTraitList = null;
            m_factionParentList = null;
            m_factionRelationshipList = null;
            m_factionRelationshipTraitList = null;
        }

        protected virtual void SetupFactionEditList(SerializedProperty faction)
        {
            // Personality traits:
            m_factionTraitList = new ReorderableList(
                serializedObject, faction.FindPropertyRelative("traits"),
                false, true, true, false);
            m_factionTraitList.drawHeaderCallback = OnDrawFactionTraitListHeader;
            m_factionTraitList.drawElementCallback = OnDrawFactionTraitListElement;
            m_factionTraitList.onAddDropdownCallback = OnAddFactionTraitsDropdown;

            // Parents:
            m_factionParentList = new ReorderableList(
                serializedObject, faction.FindPropertyRelative("parents"),
                true, true, true, true);
            m_factionParentList.drawHeaderCallback = OnDrawFactionParentListHeader;
            m_factionParentList.drawElementCallback = OnDrawFactionParentListElement;
            m_factionParentList.onAddDropdownCallback = OnAddFactionParentDropdown;

            // Relationships:
            m_factionRelationshipList = new ReorderableList(
                serializedObject, faction.FindPropertyRelative("relationships"),
                true, true, true, true);
            m_factionRelationshipList.drawHeaderCallback = OnDrawFactionRelationshipListHeader;
            m_factionRelationshipList.drawElementCallback = OnDrawFactionRelationshipListElement;
            m_factionRelationshipList.onAddDropdownCallback = OnAddFactionRelationshipDropdown;
            m_factionRelationshipList.onSelectCallback = OnSelectFactionRelationship;
            m_factionRelationshipList.onRemoveCallback = OnRemoveFactionRelationship;

            m_factionRelationshipTraitList = null;
            m_factionInheritedRelationshipsList = null;
            m_inheritedRelationships = null;
        }

        protected virtual void DrawFactionEditSection()
        {
            EditorGUILayout.LabelField("Faction: " + m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index).FindPropertyRelative("name").stringValue);
            m_factionParentList.DoLayoutList();
            m_factionTraitList.DoLayoutList();
            m_factionRelationshipList.DoLayoutList();
            if (m_factionRelationshipTraitList != null)
            {
                m_factionRelationshipTraitList.DoLayoutList();
            }
            DrawInheritedRelationshipsSection();
            DrawPercentJudgeParentsSection();
        }

        protected SerializedProperty FindFaction(int factionID)
        {
            for (int i = 0; i < m_factionList.serializedProperty.arraySize; i++)
            {
                var element = m_factionList.serializedProperty.GetArrayElementAtIndex(i);
                if (element.FindPropertyRelative("id").intValue == factionID)
                {
                    return element;
                }
            }
            return null;
        }

        //----- Parents: -----

        protected virtual void OnDrawFactionParentListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Parents");
        }

        protected virtual void OnDrawFactionParentListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var parentID = m_factionParentList.serializedProperty.GetArrayElementAtIndex(index).intValue;
            var parent = FindFaction(parentID);
            if (parent == null) return;
            rect.y += 2;
            var m_nameWidth = Mathf.Clamp(rect.width / 4, 80, 200);
            var descriptionWidth = rect.width - m_nameWidth - 4;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, m_nameWidth, EditorGUIUtility.singleLineHeight),
                parent.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - descriptionWidth, rect.y, descriptionWidth, EditorGUIUtility.singleLineHeight),
                parent.FindPropertyRelative("description"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void OnAddFactionParentDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();
            for (int i = 0; i < m_factionList.serializedProperty.arraySize; i++)
            {
                if (i != m_factionList.index)
                {
                    var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(i);
                    var factionID = faction.FindPropertyRelative("id").intValue;
                    var factionName = faction.FindPropertyRelative("name").stringValue;
                    menu.AddItem(
                        new GUIContent(factionName),
                        false, OnSelectFactionParentMenuItem, factionID);
                }
            }
            menu.ShowAsContext();
        }

        protected virtual void OnSelectFactionParentMenuItem(object parentID)
        {
            var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index);
            var parents = faction.FindPropertyRelative("parents");
            parents.arraySize++;
            parents.GetArrayElementAtIndex(parents.arraySize - 1).intValue = (int)parentID;
            serializedObject.ApplyModifiedProperties();
        }

        //----- Faction Personality Traits: -----

        protected virtual void OnDrawFactionTraitListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Personality Traits");
        }

        protected virtual void OnDrawFactionTraitListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_factionTraitList.serializedProperty.GetArrayElementAtIndex(index);
            DrawPersonalityTraitListElement(rect, index, isActive, isFocused, element);
        }

        protected virtual void OnAddFactionTraitsDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();
            var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index);
            var hasParents = (faction.FindPropertyRelative("parents").arraySize > 0);
            if (hasParents)
            {
                menu.AddItem(new GUIContent("(Average from parents)"), false, OnSelectAverageTraitsMenuItem, faction.FindPropertyRelative("id").intValue);
                menu.AddItem(new GUIContent("(Sum from parents)"), false, OnSelectSumTraitsMenuItem, faction.FindPropertyRelative("id").intValue);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("(Average from parents)"));
                menu.AddDisabledItem(new GUIContent("(Sum from parents)"));
            }
            for (int i = 0; i < m_presetList.serializedProperty.arraySize; i++)
            {
                var preset = m_presetList.serializedProperty.GetArrayElementAtIndex(i);
                var presetName = preset.FindPropertyRelative("name").stringValue;
                menu.AddItem(
                    new GUIContent(presetName),
                    false, OnSelectFactionPresetMenuItem, preset);
            }
            menu.ShowAsContext();
        }

        protected virtual void OnSelectFactionPresetMenuItem(object preset)
        {
            var presetTraits = (preset as SerializedProperty).FindPropertyRelative("traits");
            var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index);
            var factionTraits = faction.FindPropertyRelative("traits");
            for (int i = 0; i < presetTraits.arraySize; i++)
            {
                factionTraits.GetArrayElementAtIndex(i).floatValue = presetTraits.GetArrayElementAtIndex(i).floatValue;
            }
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSelectAverageTraitsMenuItem(object factionID)
        {
            serializedObject.ApplyModifiedProperties();
            (target as FactionDatabase).InheritTraitsFromParents((int)factionID, FactionInheritanceType.Average);
            serializedObject.Update();
        }

        protected virtual void OnSelectSumTraitsMenuItem(object factionID)
        {
            serializedObject.ApplyModifiedProperties();
            (target as FactionDatabase).InheritTraitsFromParents((int)factionID, FactionInheritanceType.Sum);
            serializedObject.Update();
        }

        //----- Relationships: -----

        protected virtual void OnDrawFactionRelationshipListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Relationships");
        }

        protected virtual void OnDrawFactionRelationshipListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var relationship = m_factionRelationshipList.serializedProperty.GetArrayElementAtIndex(index);
            var otherFaction = FindFaction(relationship.FindPropertyRelative("factionID").intValue);
            if (otherFaction == null) return;
            rect.y += 2;
            var m_nameWidth = Mathf.Clamp(rect.width / 4, 80, 200);
            var valueWidth = rect.width - m_nameWidth - 18;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, m_nameWidth, EditorGUIUtility.singleLineHeight),
                otherFaction.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.EndDisabledGroup();
            var inheritable = relationship.FindPropertyRelative("inheritable");
            inheritable.boolValue = EditorGUI.Toggle(
                new Rect(rect.x + m_nameWidth + 2, rect.y, 16, EditorGUIUtility.singleLineHeight),
                new GUIContent(string.Empty, "Inheritable"), inheritable.boolValue);
            var traits = relationship.FindPropertyRelative("traits");
            if (traits.arraySize != m_relationshipTraitDefList.serializedProperty.arraySize) traits.arraySize = m_relationshipTraitDefList.serializedProperty.arraySize;
            EditorGUI.Slider(
                new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
                traits.GetArrayElementAtIndex(0), -100, 100, GUIContent.none);
        }

        protected virtual void OnAddFactionRelationshipDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();
            for (int i = 0; i < m_factionList.serializedProperty.arraySize; i++)
            {
                var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(i);
                var factionID = faction.FindPropertyRelative("id").intValue;
                var factionName = faction.FindPropertyRelative("name").stringValue;
                menu.AddItem(
                    new GUIContent(factionName),
                    false, OnSelectFactionRelationshipMenuItem, factionID);
            }
            menu.ShowAsContext();
        }

        protected virtual void OnSelectFactionRelationshipMenuItem(object factionID)
        {
            var faction = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index);
            var relationships = faction.FindPropertyRelative("relationships");
            relationships.arraySize++;
            var relationship = relationships.GetArrayElementAtIndex(relationships.arraySize - 1);
            relationship.FindPropertyRelative("factionID").intValue = (int)factionID;
            relationship.FindPropertyRelative("inheritable").boolValue = true;
            var traits = relationship.FindPropertyRelative("traits");
            if (traits.arraySize != m_relationshipTraitDefList.serializedProperty.arraySize) traits.arraySize = m_relationshipTraitDefList.serializedProperty.arraySize;
            traits.GetArrayElementAtIndex(0).floatValue = 0;
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void NotifyRelationshipsAddTrait()
        {
            var traits = serializedObject.FindProperty("relationshipTraitDefinitions");
            var factions = serializedObject.FindProperty("factions");
            for (int factionIndex = 0; factionIndex < factions.arraySize; factionIndex++)
            {
                var faction = factions.GetArrayElementAtIndex(factionIndex);
                var relationships = faction.FindPropertyRelative("relationships");
                for (int relationshipIndex = 0; relationshipIndex < relationships.arraySize; relationshipIndex++)
                {
                    var relationship = relationships.GetArrayElementAtIndex(relationshipIndex);
                    relationship.FindPropertyRelative("traits").arraySize = traits.arraySize;
                }
            }
        }

        protected virtual void NotifyRelationshipsRemoveTrait(int index)
        {
            var factions = serializedObject.FindProperty("factions");
            for (int factionIndex = 0; factionIndex < factions.arraySize; factionIndex++)
            {
                var faction = factions.GetArrayElementAtIndex(factionIndex);
                var relationships = faction.FindPropertyRelative("relationships");
                for (int relationshipIndex = 0; relationshipIndex < relationships.arraySize; relationshipIndex++)
                {
                    var relationship = relationships.GetArrayElementAtIndex(relationshipIndex);
                    relationship.FindPropertyRelative("traits").DeleteArrayElementAtIndex(index);
                }
            }
        }

        protected virtual void NotifyRelationshipsReorderTrait(int oldIndex, int newIndex)
        {
            var factions = serializedObject.FindProperty("factions");
            for (int factionIndex = 0; factionIndex < factions.arraySize; factionIndex++)
            {
                var faction = factions.GetArrayElementAtIndex(factionIndex);
                var relationships = faction.FindPropertyRelative("relationships");
                for (int relationshipIndex = 0; relationshipIndex < relationships.arraySize; relationshipIndex++)
                {
                    var relationship = relationships.GetArrayElementAtIndex(relationshipIndex);
                    var values = relationship.FindPropertyRelative("traits");
                    var value = values.GetArrayElementAtIndex(oldIndex).floatValue;
                    values.DeleteArrayElementAtIndex(oldIndex);
                    values.InsertArrayElementAtIndex(newIndex);
                    values.GetArrayElementAtIndex(newIndex).floatValue = value;
                }
            }
        }

        protected virtual void OnSelectFactionRelationship(ReorderableList list)
        {
            SetupFactionRelationshipTraitList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        protected virtual void OnRemoveFactionRelationship(ReorderableList list)
        {
            m_factionRelationshipTraitList = null;
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }

        //----- Relationship traits (for currently-selected relationship): -----

        protected virtual void SetupFactionRelationshipTraitList(SerializedProperty relationship)
        {
            m_factionRelationshipTraitList = new ReorderableList(
                serializedObject, relationship.FindPropertyRelative("traits"),
                false, true, false, false);
            m_factionRelationshipTraitList.drawHeaderCallback = OnDrawFactionRelationshipTraitListHeader;
            m_factionRelationshipTraitList.drawElementCallback = OnDrawFactionRelationshipTraitListElement;
        }

        protected virtual void OnDrawFactionRelationshipTraitListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Relationship Traits");
        }

        protected virtual void OnDrawFactionRelationshipTraitListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_factionRelationshipTraitList.serializedProperty.GetArrayElementAtIndex(index);
            DrawRelationshipTraitListElement(rect, index, isActive, isFocused, element);
        }

        protected virtual void DrawRelationshipTraitListElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var valueWidth = rect.width - nameWidth - 4;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                m_relationshipTraitDefList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue);
            EditorGUI.EndDisabledGroup();
            EditorGUI.Slider(
                new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
                element, -100, 100, GUIContent.none);
        }

        //----- Inherited relationships (for currently-selected faction): -----

        protected virtual void DrawInheritedRelationshipsSection()
        {
            EditorGUI.indentLevel++;
            foldouts[target].inheritedRelationshipsFoldout = EditorGUILayout.Foldout(foldouts[target].inheritedRelationshipsFoldout, "Inherited Relationships");
            if (foldouts[target].inheritedRelationshipsFoldout)
            {
                if (m_inheritedRelationships == null || m_factionInheritedRelationshipsList == null)
                {
                    SetupInheritedRelationshipList();
                }
                m_factionInheritedRelationshipsList.DoLayoutList();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Refresh", GUILayout.Width(64)))
                {
                    SetupInheritedRelationshipList();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        protected virtual void SetupInheritedRelationshipList()
        {
            serializedObject.ApplyModifiedProperties();
            var factionID = m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index).FindPropertyRelative("id").intValue;
            m_inheritedRelationships = InheritedRelationship.GetInheritedRelationships(target as FactionDatabase, factionID);
            m_factionInheritedRelationshipsList = new ReorderableList(m_inheritedRelationships, typeof(Relationship), false, true, false, false);
            m_factionInheritedRelationshipsList.drawHeaderCallback = OnDrawInheritedRelationshipsListHeader;
            m_factionInheritedRelationshipsList.drawElementCallback = OnDrawInheritedRelationshipsListElement;
        }

        protected virtual void OnDrawInheritedRelationshipsListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Inherited Relationships");
        }

        protected virtual void OnDrawInheritedRelationshipsListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = rect.width / 2;
            var valueWidth = rect.width - nameWidth - 4;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                m_inheritedRelationships[index].name);
            EditorGUI.Slider(
                new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
                GUIContent.none, m_inheritedRelationships[index].affinity, -100, 100);
            EditorGUI.EndDisabledGroup();
        }

        //----- Percent judge parents -----

        public void DrawPercentJudgeParentsSection()
        {
            EditorGUILayout.Slider(m_factionList.serializedProperty.GetArrayElementAtIndex(m_factionList.index).FindPropertyRelative("percentJudgeParents"),
                0, 100, new GUIContent("% Judge Parents", "If nonzero, when adjusting a relationship to a subject at runtime, also adjust relationships to the subject's parents by this percent"));
        }

        #endregion

        #region Context Menu

        protected static string jsonFilename;

        [MenuItem("CONTEXT/FactionDatabase/CSV Export\u2215Import...")]
        protected static void CSVImportExport(MenuCommand command)
        {
            var window = FactionDatabaseImportExportCSVWindow.Open();
            window.SetDatabase(command.context as FactionDatabase);
        }

        [MenuItem("CONTEXT/FactionDatabase/Export JSON...")]
        protected static void ExportJSON(MenuCommand command)
        {
            var db = command.context as FactionDatabase;
            if (db == null) return;
            var filename = EditorUtility.SaveFilePanel("Export JSON", jsonFilename, "FactionDatabase.json", "json");
            if (string.IsNullOrEmpty(filename)) return;
            jsonFilename = filename;
            File.WriteAllText(jsonFilename, JsonUtility.ToJson(db, true));
            Debug.Log("Exported " + db.name + " to " + jsonFilename + ".", db);
        }

        [MenuItem("CONTEXT/FactionDatabase/Import JSON...")]
        protected static void ImportJSON(MenuCommand command)
        {
            var db = command.context as FactionDatabase;
            if (db == null) return;
            var filename = EditorUtility.OpenFilePanel("Import JSON", Path.GetDirectoryName(jsonFilename), "json");
            if (string.IsNullOrEmpty(filename)) return;
            jsonFilename = filename;
            var contents = File.ReadAllText(jsonFilename);
            JsonUtility.FromJsonOverwrite(contents, db);
            EditorUtility.SetDirty(db);
            Debug.Log("Imported " + db.name + " from " + jsonFilename + ".", db);
        }

        #endregion

    }

}
