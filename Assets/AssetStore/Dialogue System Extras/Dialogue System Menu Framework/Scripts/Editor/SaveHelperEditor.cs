// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// Adds a 'Delete all saved games' button to the Save Helper editor.
    /// </summary>
    [CustomEditor(typeof(SaveHelper))]
    public class SaveHelperEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button(new GUIContent("Delete Saved Games", "Delete all saved games.")))
            {
                if (EditorUtility.DisplayDialog("Delete Saved Games", "Delete all saved games?", "OK", "Cancel"))
                {
                    DeleteSavedGames();
                }
            }
        }

        private void DeleteSavedGames()
        {
            var saveHelper = target as SaveHelper;
            if (saveHelper == null) return;
            for (int i = 0; i < 100; i++)
            {
                PlayerPrefs.DeleteKey("savedgame_lastSlotNum");
                PlayerPrefs.DeleteKey("savedgame_" + i + "_summary");
                PlayerPrefs.DeleteKey("savedgame_" + i + "_details");
                var storer = FindObjectOfType<SavedGameDataStorer>();
                if (storer == null) storer = FindObjectOfType<SaveSystem>().gameObject.AddComponent<PlayerPrefsSavedGameDataStorer>();
                storer.DeleteSavedGameData(i);
            }
        }
    }
}