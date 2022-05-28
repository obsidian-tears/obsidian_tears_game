// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Utility window that lets the user copy tags and messages to the clipboard
    /// to make it easier to enter them in text fields in the Quest Editor.
    /// </summary>
    public class QuestReferenceEditorWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Quest Machine/Quest Reference", false, 3)]
        public static void ShowWindow()
        {
            GetWindow<QuestReferenceEditorWindow>();
        }

        private static string[] ToolbarNames = new string[] { "About", "Tags", "Messages", "Counters" };
        [SerializeField]
        private int m_selectedToolbar = 0;

        private static string[] CounterToolbarNames = new string[] { "Current", "Min", "Max", "As Time" };
        [SerializeField]
        private int m_selectedCounterToolbar = 0;

        [SerializeField]
        private Vector2 m_tagsScrollPosition = Vector2.zero;

        [SerializeField]
        private Vector2 m_messagesScrollPosition = Vector2.zero;

        [SerializeField]
        private Vector2 m_countersScrollPosition = Vector2.zero;

        private List<Info> m_tags;
        private const float MaxTagWidth = 150;

        private List<Info> m_messages;
        private const float MaxMessageWidth = 150;

        private const float MaxCounterWidth = 140;

        private GUIStyle m_label = null;
        private GUIStyle wordWrappedLabel
        {
            get
            {
                if (m_label == null)
                {
                    m_label = new GUIStyle(EditorStyles.label);
                    m_label.wordWrap = true;
                }
                return m_label;
            }
        }

        private void OnEnable()
        {
            titleContent.text = "Quest Reference";
            m_tags = LoadInfo(TagsText);
            m_messages = LoadInfo(MessagesText);
        }

        private void OnGUI()
        {
            m_selectedToolbar = GUILayout.Toolbar(m_selectedToolbar, ToolbarNames);
            switch (m_selectedToolbar)
            {
                case 0:
                    DrawAbout();
                    break;
                case 1:
                    m_tagsScrollPosition = DrawInfoList(m_tagsScrollPosition, m_tags, MaxTagWidth);
                    break;
                case 2:
                    m_messagesScrollPosition = DrawInfoList(m_messagesScrollPosition, m_messages, MaxMessageWidth);
                    break;
                case 3:
                    m_countersScrollPosition = DrawCounterList(m_countersScrollPosition);
                    break;
            }
        }

        private void DrawAbout()
        {
            EditorGUILayout.HelpBox("Use this window to copy commonly-used tags and messages " +
                "to the clipboard so you can paste them into your quests. A future update will " +
                "let you add custom tags and messages to the lists in this window.", MessageType.Info);
        }

        private Vector2 DrawInfoList(Vector2 scrollPosition, List<Info> list, float maxValueWidth)
        {
            if (list == null) return scrollPosition;
            try
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.HelpBox("Click a button to copy it to the clipboard.", MessageType.None);
                for (int i = 0; i < list.Count; i++)
                {
                    var value = list[i].value;
                    var description = list[i].description;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(value, GUILayout.Width(maxValueWidth)))
                    {
                        EditorGUIUtility.systemCopyBuffer = value;
                        Debug.Log("Copied '" + value + "' to clipboard.");
                    }
                    EditorGUILayout.LabelField(description, wordWrappedLabel);
                    EditorGUILayout.EndHorizontal();
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
            return scrollPosition;
        }

        private Vector2 DrawCounterList(Vector2 scrollPosition)
        {
            if (QuestEditorWindow.selectedQuest == null)
            {
                EditorGUILayout.HelpBox("Open a quest in the Quest Editor window to see its counters.", MessageType.Info);
                return scrollPosition;
            }
            try
            {
                var quest = QuestEditorWindow.selectedQuest;
                m_selectedCounterToolbar = GUILayout.Toolbar(m_selectedCounterToolbar, CounterToolbarNames);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.HelpBox("Click a button to copy it to the clipboard.", MessageType.None);
                for (int i = 0; i < quest.counterList.Count; i++)
                {
                    var value = GetCounterButtonValue(quest.counterList[i]);
                    var description = GetCounterButtonDescription();
                    if (string.IsNullOrEmpty(value)) continue; ;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(value, GUILayout.Width(MaxCounterWidth)))
                    {
                        EditorGUIUtility.systemCopyBuffer = value;
                        Debug.Log("Copied '" + value + "' to clipboard.");
                    }
                    EditorGUILayout.LabelField(description, wordWrappedLabel);
                    EditorGUILayout.EndHorizontal();
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
            return scrollPosition;
        }

        private string GetCounterButtonValue(QuestCounter counter)
        {
            if (counter == null) return null;
            switch (m_selectedCounterToolbar)
            {
                default:
                case 0:
                    return "{#" + counter.name + "}";
                case 1:
                    return "{<#" + counter.name + "}";
                case 2:
                    return "{>#" + counter.name + "}";
                case 3:
                    return "{:" + counter.name + "}";
            }
        }

        private string GetCounterButtonDescription()
        {
            switch (m_selectedCounterToolbar)
            {
                default:
                case 0:
                    return "Current value.";
                case 1:
                    return "Min value.";
                case 2:
                    return "Max value.";
                case 3:
                    return "Current value as time.";
            }
        }

        #region Info List Element

        private class Info
        {
            public string value;
            public string description;

            public Info(string value, string description)
            {
                this.value = value;
                this.description = description;
            }
        }

        private List<Info> LoadInfo(string text)
        {
            var list = new List<Info>();
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var barPosition = line.IndexOf('|');
                if (barPosition <= 0) continue;
                var value = line.Substring(0, barPosition);
                var description = line.Substring(barPosition + 1).Replace("\\n", "\n");
                list.Add(new Info(value, description));
            }
            return list;
        }

        private const string TagsText =
            "{QUEST}|Quest's title.\n" +
            "{QUESTID}|Quest's ID.\n" +
            "{QUESTGIVER}|Quest giver's display name.\n" +
            "{QUESTGIVERID}|Quest giver's ID.\n" +
            "{QUESTER}|Quester's (player's) display name.\n" +
            "{QUESTERID}|Quester's (player's) ID.\n" +
            "{GREETER}|Display name of quester (player) greeting quest giver. Use if quester hasn't accepted quest yet.\n" +
            "{GREETERID}|ID of quester (player) greeting quest giver.\n" +
            "{#COUNTERNAME}|(Generator) Current value of action's counter.\n" +
            "{#COUNTERGOAL}|(Generator) Required value of action's counter.\n" +
            "{TARGETENTITY}|(Generator) Entity name of quest target.\n" +
            "{TARGETDESCRIPTOR}|(Generator) Display name & count of quest target.\n" +
            "{DOMAIN}|(Generator) Location of quest target.\n";

        private const string MessagesText =
            "Greet|Sent prior to starting dialogue. Parameter is quest giver's ID.\n" +
            "Greeted|Sent after starting dialogue. Parameter is quest giver's ID.\n" +
            "Discuss Quest|Sent prior to starting discussion of a quest. Parameter is quest ID. Value is quest giver's ID.\n" +
            "Discussed Quest|Sent after starting discussion of a quest. Parameter is quest ID. Value is quest giver's ID.\n" +
            "Quest State Changed|Sent when a quest state changes. Parameter is quest ID. Value is quest node ID or blank. Value 2 is state.\n" +
            "Check Offer Conditions|Send to recheck offer conditions in case quest is no longer offerable. Parameter is quest ID.\n" +
            "Start Spawner|Send to start a spawner. Parameter is spawner name.\n" +
            "Stop Spawner|Send to stop a spawner. Parameter is spawner name.\n" +
            "Despawn Spawner|Send to stop a spawner and despawn all spawned objects. Parameter is spawner name.\n";
        //--- Haven't decided if these are worth including:
        //"Quest Alert|\n" +
        //"Refresh UIs|\n" +
        //"Refresh Indicator|\n" +
        //"Set Indicator State|\n" +
        //"Quest Track Toggle Changed|\n" +
        //"Quest Abandoned|\n" +
        //"Quest Counter Changed|\n" +
        //"Set Quest Counter|\n" +
        //"Increment Quest Counter|\n";

        #endregion

    }
}
