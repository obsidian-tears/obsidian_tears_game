// Copyright (c) Pixel Crushers. All rights reserved.

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Exports & imports FactionDatabase contents to CSV.
    /// </summary>
    public class FactionDatabaseImportExportCSVWindow : EditorWindow
    {

        #region Variables

        [System.Serializable]
        public class Prefs
        {
            public int toolbarIndex = 0;
            public int dbInstanceID = 0;
            public string folder = null;
            public EncodingType encoding = EncodingType.Default;

            // Import settings:
            public bool autoID = false;
            public int idColumn = 1; // Columns start from 1.
            public int nameColumn = 2;
            public int descriptionColumn = 3;
            public bool hasPresets = true;
            public int presetColumn = 4;
            public int colorColumn = 5;
            public int percentJudgeParentsColumn = 6;
            public int firstPersonalityTraitColumn = 7;
            public bool parentsAfterPersonalityTraits = true;
            public int firstParentsColumn = 8;
            public int lastParentsColumn = 8;
        }

        private const string EditorPrefsKey = "PixelCrushers.LoveHate.CSVImportExportPrefs";
        private static string[] Toolbar = new string[] { "Export", "Import" };

        private Prefs prefs = null;
        private FactionDatabase db = null;
        private Vector2 scrollPosition = Vector2.zero;

        #endregion

        #region Initialization

        [MenuItem("Tools/Pixel Crushers/Love\u2215Hate/CSV Export\u2215Import...", false, 1)]
        public static FactionDatabaseImportExportCSVWindow Open()
        {
            var window = GetWindow(typeof(FactionDatabaseImportExportCSVWindow), false, "Love/Hate CSV") as FactionDatabaseImportExportCSVWindow;
            window.minSize = new Vector2(320, 240);
            return window;
        }

        private void OnEnable()
        {
            prefs = EditorPrefs.HasKey(EditorPrefsKey) ? JsonUtility.FromJson<Prefs>(EditorPrefs.GetString(EditorPrefsKey)) : null;
            if (prefs == null) prefs = new Prefs();
            if (prefs.dbInstanceID != 0) db = EditorUtility.InstanceIDToObject(prefs.dbInstanceID) as FactionDatabase;
        }

        private void OnDisable()
        {
            prefs.dbInstanceID = (db != null) ? db.GetInstanceID() : 0;
            EditorPrefs.SetString(EditorPrefsKey, JsonUtility.ToJson(prefs));
        }

        public void SetDatabase(FactionDatabase db)
        {
            this.db = db;
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            prefs.toolbarIndex = GUILayout.Toolbar(prefs.toolbarIndex, Toolbar);
            try
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                switch (prefs.toolbarIndex)
                {
                    case 0:
                        DrawExport();
                        break;
                    case 1:
                        DrawImport();
                        break;
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawExport()
        {
            db = EditorGUILayout.ObjectField("Faction Database", db, typeof(FactionDatabase), true) as FactionDatabase;
            EditorGUILayout.BeginHorizontal();
            prefs.folder = EditorGUILayout.TextField("Export To", prefs.folder);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                prefs.folder = EditorUtility.OpenFolderPanel("Destination for CSV files", prefs.folder, "");
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
            prefs.encoding = (EncodingType)EditorGUILayout.EnumPopup("Encoding", prefs.encoding);

            var isFolderValid = !string.IsNullOrEmpty(prefs.folder) && Directory.Exists(prefs.folder);
            if (isFolderValid)
            {
                EditorGUILayout.HelpBox("Destination CSV files:\n" +
                    prefs.folder + "/PersonalityTraits.csv\n" +
                    prefs.folder + "/RelationshipTraits.csv\n" +
                    prefs.folder + "/Factions.csv\n" +
                    prefs.folder + "/Relationships_*.csv", MessageType.None);
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a folder to export the CSV files.", MessageType.Info);
            }
            EditorGUI.BeginDisabledGroup(db == null || !isFolderValid);
            if (GUILayout.Button("Export")) Export();
            EditorGUI.EndDisabledGroup();

        }

        private void DrawImport()
        {
            db = EditorGUILayout.ObjectField("Faction Database", db, typeof(FactionDatabase), false) as FactionDatabase;
            EditorGUILayout.BeginHorizontal();
            prefs.folder = EditorGUILayout.TextField("Import From", prefs.folder);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                prefs.folder = EditorUtility.OpenFolderPanel("Location of CSV files", prefs.folder, "");
                GUIUtility.keyboardControl = 0;
            }
            EditorGUILayout.EndHorizontal();
            prefs.encoding = (EncodingType)EditorGUILayout.EnumPopup("Encoding", prefs.encoding);

            var isFolderValid = !string.IsNullOrEmpty(prefs.folder) && Directory.Exists(prefs.folder);
            if (isFolderValid)
            {
                EditorGUILayout.HelpBox("Attempt to import these CSV files:\n" +
                    prefs.folder + "/PersonalityTraits.csv\n" +
                    prefs.folder + "/RelationshipTraits.csv\n" +
                    prefs.folder + "/Factions.csv\n" +
                    prefs.folder + "/Relationships_*.csv", MessageType.None);
                EditorGUILayout.LabelField("Faction Spreadsheet Configuration", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("If you're importing from CSV files that were exported from this window, you can use the default import settings. " +
                    "If you're importing from a different spreadsheet format, specify the format of the Factions spreadsheet below.", MessageType.None);
                prefs.autoID = EditorGUILayout.Toggle(new GUIContent("Auto-assign IDs", "If ticked, auto-assign IDs. Otherwise use the ID Column."), prefs.autoID);
                if (!prefs.autoID) prefs.idColumn = EditorGUILayout.IntField("ID (" + GetColumnAlpha(prefs.idColumn) + ")", prefs.idColumn);
                prefs.nameColumn = EditorGUILayout.IntField("Name Column (" + GetColumnAlpha(prefs.nameColumn) + ")", prefs.nameColumn);
                prefs.descriptionColumn = EditorGUILayout.IntField("Description (" + GetColumnAlpha(prefs.descriptionColumn) + ")", prefs.descriptionColumn);
                prefs.hasPresets = EditorGUILayout.Toggle(new GUIContent("Includes Presets", "If ticked, spreadsheet also includes rows for presets."), prefs.hasPresets);
                if (prefs.hasPresets) prefs.presetColumn = EditorGUILayout.IntField("Preset (" + GetColumnAlpha(prefs.presetColumn) + ")", prefs.presetColumn);
                prefs.colorColumn = EditorGUILayout.IntField("Color (" + GetColumnAlpha(prefs.colorColumn) + ")", prefs.colorColumn);
                prefs.percentJudgeParentsColumn = EditorGUILayout.IntField("% Judge Parents (" + GetColumnAlpha(prefs.percentJudgeParentsColumn) + ")", prefs.percentJudgeParentsColumn);
                prefs.firstPersonalityTraitColumn = EditorGUILayout.IntField("First Personality Trait (" + GetColumnAlpha(prefs.firstPersonalityTraitColumn) + ")", prefs.firstPersonalityTraitColumn);
                prefs.parentsAfterPersonalityTraits = EditorGUILayout.Toggle(new GUIContent("Parents After Personality", "Parents immediately follow personality trait values."), prefs.parentsAfterPersonalityTraits);
                if (!prefs.parentsAfterPersonalityTraits)
                {
                    prefs.firstParentsColumn = EditorGUILayout.IntField("First Parent (" + GetColumnAlpha(prefs.firstParentsColumn) + ")", prefs.firstParentsColumn);
                    prefs.lastParentsColumn = EditorGUILayout.IntField("Last Parent (" + GetColumnAlpha(prefs.lastParentsColumn) + ")", prefs.lastParentsColumn);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Specify a folder containing the CSV files.", MessageType.Info);
            }
            EditorGUILayout.HelpBox("Importing will overwrite the contents of your faction database.", MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset", GUILayout.Width(64))) ResetSettings();
            EditorGUI.BeginDisabledGroup(db == null || !isFolderValid);
            var importNow = GUILayout.Button("Import");
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            if (importNow) Import();
        }

        private string GetColumnAlpha(int columnFromOne)
        {
            return (columnFromOne <= 26) ? ((char)((int)'A' + columnFromOne - 1)).ToString()
                : ((char)((int)'A' + (columnFromOne / 26) - 1)).ToString() + ((char)((int)'A' + columnFromOne - 27)).ToString();
        }

        private void ResetSettings()
        {
            if (EditorUtility.DisplayDialog("Reset Settings", "Reset the Love/Hate CSV export/import window settings?", "OK", "Cancel"))
            {
                prefs = new Prefs();
                db = null;
            }
        }

        #endregion

        #region Export

        private void Export()
        {
            try
            {
                // PersonalityTraits.csv:
                EditorUtility.DisplayProgressBar("Exporting Faction Database To CSV", "Exporting personality trait definitions...", 0);
                var csv = new List<List<string>>();
                csv.Add(new List<string>(new string[] { "Name", "Description" }));
                for (int i = 0; i < db.personalityTraitDefinitions.Length; i++)
                {
                    var def = db.personalityTraitDefinitions[i];
                    csv.Add(new List<string>(new string[] { def.name, def.description }));
                }
                CSVUtility.WriteCSVFile(csv, prefs.folder + "/PersonalityTraits.csv", prefs.encoding);

                // RelationshipTraits.csv:
                EditorUtility.DisplayProgressBar("Exporting Faction Database To CSV", "Exporting relationship trait definitions...", 40);
                csv = new List<List<string>>();
                csv.Add(new List<string>(new string[] { "Name", "Description" }));
                for (int i = 0; i < db.relationshipTraitDefinitions.Length; i++)
                {
                    var def = db.relationshipTraitDefinitions[i];
                    csv.Add(new List<string>(new string[] { def.name, def.description }));
                }
                CSVUtility.WriteCSVFile(csv, prefs.folder + "/RelationshipTraits.csv", prefs.encoding);

                // Factions.csv:
                EditorUtility.DisplayProgressBar("Exporting Faction Database To CSV", "Exporting factions...", 60);
                csv = new List<List<string>>();
                var row = new List<string>(new string[] { "ID", "Name", "Description", "Preset", "Color", "%Judge Parents" });
                for (int i = 0; i < db.personalityTraitDefinitions.Length; i++)
                {
                    row.Add(db.personalityTraitDefinitions[i].name);
                }
                row.Add("Parents");
                csv.Add(row);
                for (int i = 0; i < db.presets.Length; i++)
                {
                    var preset = db.presets[i];
                    row = new List<string>(new string[] { "-1", preset.name, preset.description, "1", "0", "0" });
                    for (int j = 0; j < db.personalityTraitDefinitions.Length; j++)
                    {
                        row.Add(preset.traits[j].ToString());
                    }
                    csv.Add(row);
                }
                for (int i = 0; i < db.factions.Length; i++)
                {
                    var faction = db.factions[i];
                    row = new List<string>(new string[] { faction.id.ToString(), faction.name, faction.description, "0", faction.color.ToString(), faction.percentJudgeParents.ToString() });
                    for (int j = 0; j < db.personalityTraitDefinitions.Length; j++)
                    {
                        row.Add(faction.traits[j].ToString());
                    }
                    for (int j = 0; j < faction.parents.Length; j++)
                    {
                        var parentID = faction.parents[j];
                        var parent = db.GetFaction(parentID);
                        if (parent != null)
                        {
                            row.Add(parent.name);
                        }
                        else
                        {
                            row.Add("-1");
                            Debug.LogWarning("Faction [" + faction.id + "] '" + faction.name + " has parent with ID " + parentID + " but database doesn't have a faction with ID " + parentID);
                        }
                    }
                    csv.Add(row);
                }
                CSVUtility.WriteCSVFile(csv, prefs.folder + "/Factions.csv", prefs.encoding);

                // Relationships.csv:
                EditorUtility.DisplayProgressBar("Exporting Faction Database To CSV", "Exporting relationships...", 80);
                for (int i = 0; i < db.relationshipTraitDefinitions.Length; i++)
                {
                    csv = new List<List<string>>();
                    row = new List<string>(new string[] { db.relationshipTraitDefinitions[i].name });
                    for (int j = 0; j < db.factions.Length; j++)
                    {
                        row.Add(db.factions[j].name);
                    }
                    csv.Add(row);
                    for (int j = 0; j < db.factions.Length; j++)
                    {
                        var judge = db.factions[j];
                        row = new List<string>(new string[] { judge.name });
                        for (int k = 0; k < db.factions.Length; k++)
                        {
                            var subject = db.factions[k];
                            Relationship relationship;
                            if (db.FindPersonalRelationship(judge.id, subject.id, out relationship))
                            {
                                row.Add(relationship.traits[i].ToString());
                            }
                            else
                            {
                                row.Add("none");
                            }
                        }
                        csv.Add(row);
                    }
                    CSVUtility.WriteCSVFile(csv, prefs.folder + "/Relationships_" + GetValidFilespec(db.relationshipTraitDefinitions[i].name) + ".csv", prefs.encoding);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            Debug.Log("Exported " + db.name + " to CSV files in " + prefs.folder + ".", db);
        }

        private static string invalidRegex = string.Format(@"([{0}]*\.+$)|([{0}]+)", Regex.Escape(new string(Path.GetInvalidFileNameChars())));

        private string GetValidFilespec(string s)
        {
            return Regex.Replace(s, invalidRegex, "_");
        }

        #endregion

        #region Import

        private void Import()
        {
            try
            {
                // PersonalityTraits.csv:
                EditorUtility.DisplayProgressBar("Importing Faction Database From CSV", "Importing personality trait definitions...", 0);
                var csv = ReadCSVMinusBlankLastLine(prefs.folder + "/PersonalityTraits.csv");
                if (csv.Count > 1)
                {
                    csv.RemoveAt(0); // Remove heading.
                    db.personalityTraitDefinitions = new TraitDefinition[csv.Count];
                    for (int i = 0; i < csv.Count; i++)
                    {
                        var row = csv[i];
                        db.personalityTraitDefinitions[i] = new TraitDefinition(row[0], row[1]);
                    }
                }

                // RelationshipTraits.csv:
                EditorUtility.DisplayProgressBar("Importing Faction Database From CSV", "Importing relationship trait definitions...", 40);
                csv = ReadCSVMinusBlankLastLine(prefs.folder + "/RelationshipTraits.csv");
                if (csv.Count > 1)
                {
                    csv.RemoveAt(0); // Remove heading.
                    db.relationshipTraitDefinitions = new TraitDefinition[csv.Count];
                    for (int i = 0; i < csv.Count; i++)
                    {
                        var row = csv[i];
                        db.relationshipTraitDefinitions[i] = new TraitDefinition(row[0], row[1]);
                    }
                }

                // Factions.csv:
                EditorUtility.DisplayProgressBar("Importing Faction Database From CSV", "Importing factions...", 60);
                csv = ReadCSVMinusBlankLastLine(prefs.folder + "/Factions.csv");
                if (csv.Count > 1)
                {
                    csv.RemoveAt(0); // Remove heading row.
                    var presetList = new List<Preset>();
                    var factionList = new List<Faction>();
                    // Get all except parents:
                    for (int i = 0; i < csv.Count; i++)
                    {
                        var row = csv[i];
                        if (row[0].StartsWith("GDE_")) continue; // Skip special Game Data Editor (GDE) rows.
                        if (prefs.hasPresets && GetColumn(row, prefs.presetColumn) == "1")
                        {
                            var preset = new Preset();
                            preset.name = GetColumn(row, prefs.nameColumn);
                            preset.description = GetColumn(row, prefs.descriptionColumn);
                            preset.traits = GetPersonalityTraits(row);
                            presetList.Add(preset);
                        }
                        else
                        {
                            var faction = new Faction();
                            faction.id = prefs.autoID ? factionList.Count : SafeConvert.ToInt(GetColumn(row, prefs.idColumn));
                            faction.name = GetColumn(row, prefs.nameColumn);
                            faction.description = GetColumn(row, prefs.descriptionColumn);
                            faction.color = SafeConvert.ToInt(GetColumn(row, prefs.colorColumn));
                            faction.percentJudgeParents = SafeConvert.ToFloat(GetColumn(row, prefs.percentJudgeParentsColumn));
                            faction.traits = GetPersonalityTraits(row);
                            factionList.Add(faction);
                        }
                    }
                    // Then do parents:
                    int factionIndex = 0;
                    for (int i = 0; i < csv.Count; i++)
                    {
                        var row = csv[i];
                        if (row[0].StartsWith("GDE_")) continue; // Skip special Game Data Editor (GDE) rows.
                        if (!(prefs.hasPresets && GetColumn(row, prefs.presetColumn) == "1"))
                        {
                            var faction = factionList[factionIndex++];
                            faction.parents = GetParents(row, factionList);
                        }
                    }
                    db.presets = presetList.ToArray();
                    db.factions = factionList.ToArray();
                }

                // Relationships.csv:
                EditorUtility.DisplayProgressBar("Importing Faction Database From CSV", "Importing relationships...", 60);
                for (int i = 0; i < db.relationshipTraitDefinitions.Length; i++)
                {
                    csv = ReadCSVMinusBlankLastLine(prefs.folder + "/Relationships_" + GetValidFilespec(db.relationshipTraitDefinitions[i].name) + ".csv");
                    if (csv.Count > 1)
                    {
                        for (int j = 1; j < csv.Count; j++) // Skip heading row.
                        {
                            var row = csv[j];
                            if (row.Count == 0) continue;
                            var judgeName = row[0];// db.GetFactionID(row[0]);
                            for (int k = 1; k < Mathf.Min(db.factions.Length, row.Count); k++)
                            {
                                var subjectName = db.factions[k - 1].name;
                                float value;
                                if (float.TryParse(row[k], out value))
                                {
                                    db.SetPersonalRelationshipTrait(judgeName, subjectName, i, value);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.SetDirty(db);
            }
            Debug.Log("Exported " + db.name + " to CSV files in " + prefs.folder + ".", db);
        }

        private List<List<string>> ReadCSVMinusBlankLastLine(string filename)
        {
            if (!File.Exists(filename))
            {
                Debug.LogError("Love/Hate: File not found: " + filename);
                return new List<List<string>>();
            }
            var csv = CSVUtility.ReadCSVFile(filename, prefs.encoding);
            if (string.IsNullOrEmpty(csv[csv.Count - 1][0])) csv.RemoveAt(csv.Count - 1);
            return csv;
        }

        private string GetColumn(List<string> row, int column)
        {
            return (column - 1 < row.Count) ? row[column - 1] : string.Empty;
        }

        private float[] GetPersonalityTraits(List<string> row)
        {
            var traits = new float[db.personalityTraitDefinitions.Length];
            for (int i = 0; i < db.personalityTraitDefinitions.Length; i++)
            {
                traits[i] = SafeConvert.ToFloat(GetColumn(row, prefs.firstPersonalityTraitColumn + i));
            }
            return traits;
        }

        private int[] GetParents(List<string> row, List<Faction> factionList)
        {
            int firstIndex = prefs.parentsAfterPersonalityTraits ? (prefs.firstPersonalityTraitColumn + db.personalityTraitDefinitions.Length - 1)
                : prefs.firstParentsColumn - 1;
            firstIndex = Mathf.Min(firstIndex, row.Count - 1);
            int lastIndex = prefs.parentsAfterPersonalityTraits ? row.Count : prefs.lastParentsColumn - 1;
            lastIndex = Mathf.Min(lastIndex, row.Count - 1);
            var parentList = new List<int>();
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                var parent = factionList.Find(x => x.name == row[i]);
                if (parent != null) parentList.Add(parent.id);
            }
            return parentList.ToArray();
        }

        #endregion

    }
}
