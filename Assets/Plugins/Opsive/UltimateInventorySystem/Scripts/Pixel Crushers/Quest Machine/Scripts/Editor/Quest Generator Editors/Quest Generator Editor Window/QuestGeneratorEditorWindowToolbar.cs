using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    [Serializable]
    public class QuestGeneratorEditorWindowToolbar
    {

        public int index { get { return m_index; } }

        [SerializeField]
        private int m_index = 0;

        private static GUIContent[] ToolbarContents = new GUIContent[]
        {
            new GUIContent("Entities", "Edit Quest Entity Types."),
            new GUIContent("Factions", "Edit Quest Factions."),
            new GUIContent("Drives", "Edit Quest Drives."),
            new GUIContent("Urgencies", "Edit Urgency Functions."),
            new GUIContent("Actions", "Edit Actions."),
            new GUIContent("Domains", "Edit Domain Types."),
            //new GUIContent("Specifiers", "Edit Specifiers."),
        };

        public enum ToolbarIndex
        {
            Entities,
            Factions,
            Drives,
            Urgencies,
            Actions,
            Domains
            //Specifiers
        }

        public bool Draw()
        {
            var newIndex = GUILayout.SelectionGrid(m_index, ToolbarContents, ToolbarContents.Length / 2);
            var changed = (newIndex != m_index);
            if (changed) m_index = newIndex;
            return changed;
        }
    }

}