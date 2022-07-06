// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Utility window copies a QuestDatabase's quests to a DialogueDatabase.
    /// </summary>
    public class QuestDatabaseToDialogueDatabaseWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Quest Machine/Third Party/Dialogue System/Quest DB to Dialogue DB")]
        public static void ShowWindow()
        {
            GetWindow<QuestDatabaseToDialogueDatabaseWindow>();
        }

        [SerializeField]
        private int m_questDatabaseInstanceID = 0;

        [SerializeField]
        private int m_dialogueDatabaseInstanceID = 0;

        private QuestDatabase questDatabase { get; set; }
        private DialogueDatabase dialogueDatabase { get; set; }
        private QuestMachineConfiguration qmConfiguration { get; set; }
        private Template template { get; set; }

        private void OnEnable()
        {
            titleContent.text = "QM To DS";
            questDatabase = EditorUtility.InstanceIDToObject(m_questDatabaseInstanceID) as QuestDatabase;
            if (questDatabase == null)
            {
                qmConfiguration = FindObjectOfType<QuestMachineConfiguration>();
                if (qmConfiguration != null && qmConfiguration.questDatabases.Count > 0) questDatabase = qmConfiguration.questDatabases[0];
            }
            dialogueDatabase = EditorUtility.InstanceIDToObject(m_dialogueDatabaseInstanceID) as DialogueDatabase;
            if (dialogueDatabase == null)
            {
                var dialogueManager = FindObjectOfType<DialogueSystemController>();
                if (dialogueManager != null) dialogueDatabase = dialogueManager.initialDatabase;
            }
            template = Template.FromDefault();
        }

        private void OnDisable()
        {
            m_questDatabaseInstanceID = (questDatabase != null) ? questDatabase.GetInstanceID() : 0;
            m_dialogueDatabaseInstanceID = (dialogueDatabase != null) ? dialogueDatabase.GetInstanceID() : 0;
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This tool copies quest IDs from Quest Machine to your dialogue database so you can select them in the Dialogue Editor using the Lua wizard dropdowns.", MessageType.None);
            questDatabase = EditorGUILayout.ObjectField("Quest Database", questDatabase, typeof(QuestDatabase), false) as QuestDatabase;
            dialogueDatabase = EditorGUILayout.ObjectField("Dialogue Database", dialogueDatabase, typeof(DialogueDatabase), false) as DialogueDatabase;
            EditorGUI.BeginDisabledGroup(questDatabase == null || dialogueDatabase == null);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Copy Selected QM Database", "Copy selected quest database's quests to dialogue database."), GUILayout.Width(200)))
            {
                CopyDatabase(questDatabase, true);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(qmConfiguration == null || dialogueDatabase == null);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Copy All QM Databases", "Copy all quest databases' quests to dialogue database."), GUILayout.Width(200)))
            {
                CopyAllDatabases();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private void CopyAllDatabases()
        {
            if (qmConfiguration == null) return;
            int total = 0;
            for (int i = 0; i < qmConfiguration.questDatabases.Count; i++)
            {
                total += CopyDatabase(qmConfiguration.questDatabases[i], false);
            }
            ReportCopyTotal(total);
        }

        private int CopyDatabase(QuestDatabase qdb, bool report)
        {
            if (qdb == null) return 0;
            int total = 0;
            for (int i = 0; i < qdb.questAssets.Count; i++)
            {
                if (qdb.questAssets[i] == null) continue;
                AddQuestIDToDatabase(qdb.questAssets[i]);
                total++;
            }
            if (report) ReportCopyTotal(total);
            return total;
        }

        private void AddQuestIDToDatabase(Quest quest)
        {
            string questID = (quest != null) ? StringField.GetStringValue(quest.id) : null;
            if (string.IsNullOrEmpty(questID)) return;
            if (dialogueDatabase.GetItem(questID) == null)
            {
                var item = template.CreateItem(GetNextItemID(), questID);
                item.IsItem = false;
                dialogueDatabase.items.Add(item);
                EditorUtility.SetDirty(dialogueDatabase);
            }
        }

        private int GetNextItemID()
        {
            int highest = 0;
            for (int i = 0; i < dialogueDatabase.items.Count; i++)
            {
                highest = Mathf.Max(highest, dialogueDatabase.items[i].id);
            }
            return highest + 1;
        }

        private void ReportCopyTotal(int total)
        {
            EditorUtility.DisplayDialog("Copy Quest IDs to Dialogue Database", "Copied " + total + " quests to " + dialogueDatabase.name, "OK");
            //[TODO] Refresh Dialogue Editor
        }

    }
}
