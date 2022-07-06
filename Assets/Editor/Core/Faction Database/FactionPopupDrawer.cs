// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    [CustomPropertyDrawer(typeof(FactionPopupAttribute))]
    public class FactionPopupDrawer : PropertyDrawer
    {

        private bool showReferenceDatabase = false;
        private string[] names = null;
        private bool usePicker = true;

        private static FactionDatabase database = null;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (showReferenceDatabase ? EditorGUIUtility.singleLineHeight : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // Set up property drawer:
            EditorGUI.BeginProperty(position, GUIContent.none, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Show database field if specified:
            showReferenceDatabase = (attribute as FactionPopupAttribute).showReferenceDatabase;
            if (database == null)
            {
                var factionManager = GameObject.FindObjectOfType<FactionManager>();
                if (factionManager != null)
                {
                    database = factionManager.factionDatabase;
                }
            }
            if (showReferenceDatabase)
            {
                var dbPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                var newDatabase = EditorGUI.ObjectField(dbPosition, database, typeof(FactionDatabase), true) as FactionDatabase;
                if (newDatabase != database)
                {
                    database = newDatabase;
                    names = null;
                }
            }
            if (database == null) usePicker = false;

            // Set up titles array:
            if (names == null) UpdateNames();

            // Update current index:
            var currentIndex = IDToIndex(prop.intValue);

            // Draw popup or plain int field:
            var rect = new Rect(position.x, position.y, position.width - 48, position.height);
            if (usePicker)
            {
                var newIndex = EditorGUI.Popup(rect, currentIndex, names);
                if ((newIndex != currentIndex) && (0 <= newIndex && newIndex < names.Length))
                {
                    currentIndex = newIndex;
                    prop.intValue = IndexToID(currentIndex);
                    GUI.changed = true;
                }
                if (GUI.Button(new Rect(position.x + position.width - 45, position.y, 18, 14), "x"))
                {
                    prop.intValue = -1;
                    currentIndex = -1;
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                int value = EditorGUI.IntField(rect, prop.intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    prop.intValue = value;
                }
            }

            // Radio button toggle between popup and plain int field:
            rect = new Rect(position.x + position.width - 22, position.y, 22, position.height);
            var newToggleValue = EditorGUI.Toggle(rect, usePicker, EditorStyles.radioButton);
            if (newToggleValue != usePicker)
            {
                usePicker = newToggleValue;
                names = null;
            }

            EditorGUI.EndProperty();
        }

        public void UpdateNames()
        {
            var list = new List<string>();
            if (database != null)
            {
                for (int i = 0; i < database.factions.Length; i++)
                {
                    var faction = database.factions[i];
                    var factionName = faction.name;
                    list.Add(factionName);
                }
            }
            names = list.ToArray();
        }

        public int IDToIndex(int currentID)
        {
            if (database == null) return -1;
            for (int i = 0; i < database.factions.Length; i++)
            {
                if (database.factions[i].id == currentID) return i;
            }
            return -1;
        }

        public int IndexToID(int index)
        {
            if (database == null || !(0 <= index && index < database.factions.Length)) return -1;
            return database.factions[index].id;
        }

    }
}
