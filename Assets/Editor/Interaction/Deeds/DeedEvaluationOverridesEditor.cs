using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for DeedOverrides.
    /// </summary>
    [CustomEditor(typeof(DeedEvaluationOverrides), true)]
    public class DeedOverridesEditor : Editor
    {

        //private DeedEvaluationOverrides m_deedOverrides;
        private ReorderableList m_deedOverrideList;
        private ReorderableList m_deedOverrideFieldList;
        private SerializedProperty m_deedOverrideInfo;
        private SerializedProperty m_targetFactionIDProperty;
        private FactionDatabase m_factionDatabase;
        private int[] m_factionIDList;
        private string[] m_factionNameList;

        private void OnEnable()
        {
            //m_deedOverrides = target as DeedEvaluationOverrides;
            m_factionDatabase = (target as MonoBehaviour).GetComponent<FactionMember>().factionDatabase;
            SetupDeedOverridesList();
            UpdateFactionList();
        }

        private void UpdateFactionList()
        {
            var idList = new List<int>();
            var nameList = new List<string>();
            if (m_factionDatabase != null)
            {
                for (int i = 0; i < m_factionDatabase.factions.Length; i++)
                {
                    var faction = m_factionDatabase.factions[i];
                    idList.Add(faction.id);
                    nameList.Add(faction.name);
                }
            }
            m_factionIDList = idList.ToArray();
            m_factionNameList = nameList.ToArray();
        }

        private int GetFactionListIndex(int factionID)
        {
            if (m_factionDatabase != null)
            {
                for (int i = 0; i < m_factionIDList.Length; i++)
                {
                    if (factionID == m_factionIDList[i])
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
            //--- For comparison with default inspector:
            //base.OnInspectorGUI();
            //EditorGUILayout.Separator();

            Undo.RecordObject(target, "DeedOverrides");
            DrawCustomGUI();
        }

        private void DrawCustomGUI()
        {
            serializedObject.Update();
            if (m_factionDatabase == null)
            {
                EditorGUILayout.HelpBox("Assign a faction database to your scene's Faction Manager or this GameObject's Faction Member.", MessageType.None);
            }
            else
            {
                DrawDeedOverridesSection();
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Deed Templates

        private void SetupDeedOverridesList()
        {
            m_deedOverrideList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("deedOverrides"),
                true, true, true, true);
            m_deedOverrideList.drawHeaderCallback = OnDrawDeedOverrideListHeader;
            m_deedOverrideList.drawElementCallback = OnDrawDeedOverrideListElement;
            m_deedOverrideList.onAddCallback = OnAddDeedOverride;
            m_deedOverrideList.onRemoveCallback = OnRemoveDeedOverride;
            m_deedOverrideList.onSelectCallback = OnSelectDeedOverride;
            m_deedOverrideFieldList = null;
            m_deedOverrideInfo = null;
        }

        private void DrawDeedOverridesSection()
        {
            m_deedOverrideList.DoLayoutList();
            if (m_deedOverrideFieldList != null)
            {
                DrawDeedOverrideSection();
            }
            else
            {
                EditorGUILayout.HelpBox("Click the double bars to the left of a template's name to edit its attributes.", MessageType.None);
            }
        }

        private void OnDrawDeedOverrideListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Deed Evaluation Overrides");
        }

        private void OnDrawDeedOverrideListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_deedOverrideList.serializedProperty.GetArrayElementAtIndex(index);
            DrawTagTargetListElement(rect, index, isActive, isFocused, element);
        }

        private void DrawTagTargetListElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element)
        {
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var descriptionWidth = rect.width - nameWidth - 4;
            var targetFactionIDProperty = element.FindPropertyRelative("targetFactionID");
            var targetFactionID = (targetFactionIDProperty != null) ? targetFactionIDProperty.intValue : -1;
            var targetFactionName = (0 <= targetFactionID && targetFactionID < m_factionNameList.Length) ? m_factionNameList[targetFactionID] : string.Empty;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("tag"), GUIContent.none);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.LabelField(
                new Rect(rect.x + rect.width - descriptionWidth, rect.y, descriptionWidth, EditorGUIUtility.singleLineHeight),
                targetFactionName);
            EditorGUI.EndDisabledGroup();
        }

        private float GetDefaultNameWidth(Rect rect)
        {
            return Mathf.Clamp(rect.width / 4, 80, 200);
        }

        private void OnAddDeedOverride(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            SetupDeedOverrideFieldList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        private void OnRemoveDeedOverride(ReorderableList list)
        {
            var tag = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("tag").stringValue;
            if (EditorUtility.DisplayDialog("Delete selected deed evaluation override?", tag, "Delete", "Cancel"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                m_deedOverrideFieldList = null;
            }
        }

        private void OnSelectDeedOverride(ReorderableList list)
        {
            if (list != null && 0 <= list.index && list.index < list.serializedProperty.arraySize)
            {
                SetupDeedOverrideFieldList(list.serializedProperty.GetArrayElementAtIndex(list.index));
            }
        }

        #endregion

        #region Deed Template

        private const int NumConcreteFields = 3;

        private void SetupDeedOverrideFieldList(SerializedProperty deedOverrideInfo)
        {
            m_deedOverrideInfo = deedOverrideInfo;
            m_targetFactionIDProperty = m_deedOverrideInfo.FindPropertyRelative("targetFactionID");
            m_deedOverrideInfo.FindPropertyRelative("traits").arraySize = m_factionDatabase.personalityTraitDefinitions.Length;
            var numFields = NumConcreteFields + m_factionDatabase.personalityTraitDefinitions.Length;
            m_deedOverrideFieldList = new ReorderableList(
                new bool[numFields], typeof(bool),
                false, true, true, false);
            m_deedOverrideFieldList.drawHeaderCallback = OnDrawDeedOverrideFieldListHeader;
            m_deedOverrideFieldList.drawElementCallback = OnDrawDeedOverrideFieldListElement;
            m_deedOverrideFieldList.onAddDropdownCallback = OnDeedOverridePresetsDropdown;
        }

        private void DrawDeedOverrideSection()
        {
            m_deedOverrideFieldList.DoLayoutList();
        }

        private void OnDrawDeedOverrideFieldListHeader(Rect rect)
        {
            var actionTag = m_deedOverrideList.serializedProperty.GetArrayElementAtIndex(m_deedOverrideList.index).FindPropertyRelative("tag").stringValue;
            var targetFactionID = (m_targetFactionIDProperty != null) ? m_targetFactionIDProperty.intValue : -1;
            var targetFactionName = (0 <= targetFactionID && targetFactionID < m_factionNameList.Length) ? m_factionNameList[targetFactionID] : string.Empty;
            EditorGUI.LabelField(rect, "Deed Evaluation Override: " + actionTag + " " + targetFactionName);
        }

        private void OnDrawDeedOverrideFieldListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var valueWidth = rect.width - nameWidth - 4;
            var labelRect = new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight);
            switch (index)
            {
                case 0:
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.TextField(labelRect, "Target Faction");
                    EditorGUI.EndDisabledGroup();
                    var factionListIndex = (m_targetFactionIDProperty != null) ? GetFactionListIndex(m_targetFactionIDProperty.intValue) : -1;
                    var newFactionListIndex = EditorGUI.Popup(fieldRect, factionListIndex, m_factionNameList);
                    if (newFactionListIndex != factionListIndex)
                    {
                        m_targetFactionIDProperty.intValue = m_factionIDList[newFactionListIndex];
                    }
                    break;
                case 1:
                    DrawDeedOverrideField(labelRect, "Impact", fieldRect, m_deedOverrideInfo.FindPropertyRelative("impact"), false);
                    break;
                case 2:
                    DrawDeedOverrideField(labelRect, "Aggression", fieldRect, m_deedOverrideInfo.FindPropertyRelative("aggression"), false);
                    break;
                default:
                    var traitIndex = index - NumConcreteFields;
                    var traitName = m_factionDatabase.personalityTraitDefinitions[traitIndex].name;
                    DrawDeedOverrideField(labelRect, traitName, fieldRect, m_deedOverrideInfo.FindPropertyRelative("traits").GetArrayElementAtIndex(traitIndex), true);
                    break;
            }
        }

        private void DrawDeedOverrideField(Rect labelRect, string labelText, Rect fieldRect, SerializedProperty element, bool slider)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(labelRect, labelText);
            EditorGUI.EndDisabledGroup();
            if (slider)
            {
                EditorGUI.Slider(fieldRect, element, -100, 100, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
            }
        }

        private void OnDeedOverridePresetsDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();
            for (int i = 0; i < m_factionDatabase.presets.Length; i++)
            {
                var preset = m_factionDatabase.presets[i];
                menu.AddItem(
                    new GUIContent(preset.name),
                    false, OnSelectDeedOverridePresetMenuItem, preset);
            }
            menu.ShowAsContext();
        }

        private void OnSelectDeedOverridePresetMenuItem(object presetObject)
        {
            var preset = presetObject as Preset;
            var actionTemplateValues = m_deedOverrideList.serializedProperty.GetArrayElementAtIndex(m_deedOverrideList.index).FindPropertyRelative("values");
            for (int i = 0; i < preset.traits.Length; i++)
            {
                actionTemplateValues.GetArrayElementAtIndex(i).floatValue = preset.traits[i];
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }

}