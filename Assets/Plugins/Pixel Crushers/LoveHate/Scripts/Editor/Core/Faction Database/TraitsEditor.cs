// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;  
using UnityEditor;  
using UnityEditorInternal;
using System;

namespace PixelCrushers.LoveHate
{

	/// <summary>
	/// Custom editor for Traits.
	/// </summary>
	[CustomEditor(typeof(Traits), true)]
	[CanEditMultipleObjects]
	public class TraitsEditor : Editor
	{
		
		private Traits m_traits;
		private ReorderableList m_traitList;

		private void OnEnable()
		{
			m_traits = target as Traits;
			m_traits.FindResources();
			SetupTraitList();
		}
		
		public override void OnInspectorGUI()
		{
			//base.OnInspectorGUI();
			//EditorGUILayout.Separator();
			//EditorGUILayout.LabelField("CUSTOM EDITOR:");
			
			Undo.RecordObject(target, "Traits");
			DrawCustomGUI();
		}
		
		private void DrawCustomGUI()
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("factionDatabase"));
			DrawTraitList();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawTraitList()
		{
			if (m_traits.factionDatabase == null) return;
			var numTraits = m_traits.factionDatabase.personalityTraitDefinitions.Length;
			if (m_traits.traits == null) m_traits.traits = new float[numTraits];
			if (m_traits.traits.Length != numTraits) Array.Resize(ref m_traits.traits, numTraits);
			if (m_traitList.serializedProperty.arraySize != numTraits) m_traitList.serializedProperty.arraySize = numTraits;
			m_traitList.DoLayoutList();
		}

		private void SetupTraitList()
		{
			m_traitList = new ReorderableList(
				serializedObject, serializedObject.FindProperty("traits"), 
				false, true, true, false);
			m_traitList.drawHeaderCallback = OnDrawTraitListHeader;
			m_traitList.drawElementCallback = OnDrawTraitListElement;
			m_traitList.onAddCallback = OnAddPresetDropdown;
		}
		
		private void OnDrawTraitListHeader(Rect rect)
		{  
			EditorGUI.LabelField(rect, "Traits");
		}
		
		private void OnDrawTraitListElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			if (!(0 <= index && index < m_traitList.serializedProperty.arraySize)) return;
			rect.width -= 16;
			rect.x += 16;
			rect.y += 2;
			var nameWidth = Mathf.Clamp(rect.width / 4, 80, 200);
			var valueWidth = rect.width - nameWidth - 4;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.TextField(
				new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
				m_traits.factionDatabase.personalityTraitDefinitions[index].name);
			EditorGUI.EndDisabledGroup();
			EditorGUI.Slider(
				new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight),
				m_traitList.serializedProperty.GetArrayElementAtIndex(index), -100, 100, GUIContent.none);
		}

		private void OnAddPresetDropdown(ReorderableList list)
		{
			var menu = new GenericMenu();
			var presets = m_traits.factionDatabase.presets;
			for (int i = 0; i < presets.Length; i++)
			{
				var preset = presets[i];
				menu.AddItem(
					new GUIContent(preset.name),
					false, OnSelectPresetMenuItem, preset);
			}
			menu.ShowAsContext();
		}
		
		private void OnSelectPresetMenuItem(object presetObject)
		{
			var preset = presetObject as Preset;
			if (preset == null) return;
			for (int i = 0; i < m_traitList.serializedProperty.arraySize; i++)
			{
				if (0 <= i && i < preset.traits.Length)
				{
					m_traitList.serializedProperty.GetArrayElementAtIndex(i).floatValue = preset.traits[i];
				}
			}
			serializedObject.ApplyModifiedProperties();
		}
		
	}
	
}
