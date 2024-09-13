using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameObjectToTilemap : EditorWindow
{
    private Transform rootTransform;
    private Tilemap targetTilemap;
    private TileBase[] matchingTiles;
    private bool deleteOriginals = false;

    [MenuItem("Tools/Temo_Koki/GameObject to Tilemap Converter")]
    public static void ShowWindow() => GetWindow<GameObjectToTilemap>("GameObject to Tilemap Converter");

    private void OnGUI()
    {
        GUILayout.Label("Tilemap Conversion Tool", EditorStyles.boldLabel);

        rootTransform = (Transform)EditorGUILayout.ObjectField("Objects Root Transform", rootTransform, typeof(Transform), true);
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap", targetTilemap, typeof(Tilemap), true);

        EditorGUILayout.LabelField("Matching Tiles", EditorStyles.boldLabel);

        if (matchingTiles == null) matchingTiles = new TileBase[0];
        var newArraySize = EditorGUILayout.IntField("Size", matchingTiles.Length);

        if (newArraySize != matchingTiles.Length)
            System.Array.Resize(ref matchingTiles, newArraySize);

        for (int i = 0; i < matchingTiles.Length; i++)
            matchingTiles[i] = (TileBase)EditorGUILayout.ObjectField($"Tile {i + 1}", matchingTiles[i], typeof(TileBase), false);

        if (!targetTilemap || !rootTransform || matchingTiles == null || matchingTiles.Length <= 0)
        {
            EditorGUILayout.HelpBox("Assign all fields to continue", MessageType.Warning, true);
            return;
        }

        deleteOriginals = EditorGUILayout.Toggle("Delete Original Objects", deleteOriginals);

        if (GUILayout.Button("Convert GameObjects to Tilemap"))
            ConvertGameObjectsToTilemap();
    }

    private void ConvertGameObjectsToTilemap()
    {
        Undo.RegisterCompleteObjectUndo(targetTilemap, "Tilemap Conversion");

        var spriteRenderers = rootTransform.GetComponentsInChildren<SpriteRenderer>();

        for (int i = spriteRenderers.Length - 1; i >= 0; i--)
        {
            var SR = spriteRenderers[i];
            var matchingTile = FindMatchingTile(SR.sprite);

            if (matchingTile)
            {
                var gridPos = targetTilemap.WorldToCell(SR.transform.position);
                targetTilemap.SetTile(gridPos, matchingTile);

                if (deleteOriginals)
                    Undo.DestroyObjectImmediate(SR.gameObject);
            }
        }

        targetTilemap.RefreshAllTiles();

        // EditorUtility.DisplayDialog("Success", "Conversion complete!", "OK");
    }

    private TileBase FindMatchingTile(Sprite sprite)
    {
        foreach (var tile in matchingTiles)
        {
            if (tile is Tile)
            {
                var standardTile = tile as Tile;

                if (standardTile.sprite == sprite)
                    return standardTile;
            }
        }

        return null;
    }
}
