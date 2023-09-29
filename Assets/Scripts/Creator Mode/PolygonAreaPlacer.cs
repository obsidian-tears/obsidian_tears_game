using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PolygonAreaPlacer : MonoBehaviour
{
    [SerializeField] Transform saveSpace;
    // [SerializeField] MonsterIdMapper monsterIdMapper;
    // [SerializeField] MusicIdMapper musicIdMapper;
    private GameObject selectedArea;
    private Vector2[] points;

    public void Select(GameObject area)
    {
        selectedArea = area;
        points = area.GetComponent<PolygonCollider2D>().points;
    }

    public void SetMonsterArea()//int monsterAreaUniqueId)
    {
        MonsterArea monsterArea = selectedArea.AddComponent<MonsterArea>();
        monsterArea.enemies = new List<EnemiesList>();
        monsterArea.enemies.Add(new EnemiesList(Resources.Load<Enemy>("Battle/Enemies/Slughurg"), 1));
        //Load a Texture (Assets/Resources/Textures/texture01.png)
        var texture = Resources.Load<Texture2D>("Textures/texture01");

    }

    public void SetMusicArea(int musicId)
    {

    }

    private void OnEnable()
    {
        selectedArea = null;
        points = null;
    }
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedArea == null)
                selectedArea = CreatorModeUtils.CreateVisiblePolygonArea("Area", Vector2.zero, saveSpace);
            if (points == null)
                points = new Vector2[0];

            var collider = selectedArea.GetComponent<PolygonCollider2D>();
            if (collider == null) Debug.Log("Area Collider Null");
            // collider.points = AddToArray<Vector2>(collider.points, CreatorModeUtils.GetMouseWorldPos());
            points = AddToArray<Vector2>(points, CreatorModeUtils.GetMouseWorldPos());
            collider.points = points;
            selectedArea.GetComponent<DebugPolygonCollider2D>().DrawLines();
            // Debug.Log("Points: ");
            // PrintVector2Array(collider.points);
        }
    }

    private static void PrintVector2Array(Vector2[] vectors)
    {
        foreach (var vector in vectors)
        {
            Debug.Log(vector);
        }
    }

    private static T[] AddToArray<T>(T[] array, T element)
    {
        T[] newArray = new T[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[array.Length] = element;
        return newArray;
    }

}
