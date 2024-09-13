using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FilterSelectedObjects : EditorWindow
{
    private string filterKeyword = "tree";

    [MenuItem("Tools/Temo_Koki/Filter Selection")]
    public static void ShowWindow() => GetWindow<FilterSelectedObjects>("Filter Selection");

    private void OnGUI()
    {
        GUILayout.Label("Filter Selected Game Objects by Name", EditorStyles.boldLabel);

        filterKeyword = EditorGUILayout.TextField("Filter Keyword:", filterKeyword);

        if (GUILayout.Button("Filter Objects"))
            Filter();
    }

    private void Filter()
    {
        var selectedObjects = Selection.gameObjects;
        var filteredObjects = new List<GameObject>();

        foreach (var obj in selectedObjects)
        {
            if (obj.name.ToLower().Contains(filterKeyword.ToLower()))
                filteredObjects.Add(obj);
        }

        Selection.objects = filteredObjects.ToArray();
    }
}
