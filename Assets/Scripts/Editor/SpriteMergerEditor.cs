using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteMerger))]
public class SpriteMergerEditor : Editor
{
    bool isOriginalObjectsVisible = true;

    public override void OnInspectorGUI()
    {
        var spriteMerger = (SpriteMerger)target;

        base.OnInspectorGUI();
        GUILayout.Space(10);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(8, 8, 8, 8),
            margin = new RectOffset(5, 5, 5, 5),
        };

        if (GUILayout.Button("Add Selected Objects", buttonStyle))
        {
            spriteMerger.mergeObjects.AddRange(Selection.transforms);
        }

        if (GUILayout.Button("Make Sprites Readable", buttonStyle))
            spriteMerger.MakeSpritesReadable();

        if (GUILayout.Button("Merge Sprites", buttonStyle))
            spriteMerger.Merge();

        if (GUILayout.Button((isOriginalObjectsVisible ? "Hide" : "Show") + " Original Objects", buttonStyle))
        {
            isOriginalObjectsVisible = !isOriginalObjectsVisible;
            spriteMerger.ToggleMergeObjects(isOriginalObjectsVisible);
        }

        if (GUILayout.Button("Save to File", buttonStyle))
            spriteMerger.SaveSpriteToFile();

        if (GUILayout.Button("Make Sprites Unreadable", buttonStyle))
            spriteMerger.MakeSpritesUnreadable();
    }
}
