using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class AreaEditor : MonoBehaviour
{
    [SerializeField] Transform saveSpace;
    [SerializeField] MonsterIdMapper monsterIdMapper;
    [SerializeField] MusicIdMapper musicIdMapper;

    [SerializeField] MySignal enterSignal;
    [SerializeField] MySignal exitSignal;
    [SerializeField] Song song;
    // [SerializeField] BoxAreaPlacer boxAreaPlacer;
    // [SerializeField] CreateModeHandler createModeHandler;
    private GameObject selectedArea;
    private Vector2 dragMouseOffset;

    void Update()
    {
        Vector2 worldMousePos = CreatorModeUtils.GetMouseWorldPos();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HashSet<GameObject> gameObjects = new HashSet<GameObject>(); // All gameObjects should be unique
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldMousePos, Vector2.zero);
            GameObject areaHit = null;
            foreach (RaycastHit2D hit in hits)
            {
                Collider2D collider = hit.collider;
                if (collider.GetComponent<AreaID>() != null && collider.transform.parent == saveSpace)
                {
                    areaHit = collider.gameObject;
                    break;
                }

            }
            if (areaHit != null)
            {
                // createModeHandler.SetActiveMode(EditModeComponent.EditMode.CreatingArea);
                // boxAreaCreator.Select(areaHit);
                selectedArea = areaHit;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (selectedArea != null)
            {
                dragMouseOffset = selectedArea.transform.position - new Vector3(worldMousePos.x, worldMousePos.y, 0);
            }
        }

        if (Input.GetMouseButton(1))
        {
            selectedArea.transform.position = worldMousePos + dragMouseOffset;
        }
    }

    public void SetMonsterArea(int monsterId)
    {
        MonsterArea monsterArea = selectedArea.GetComponent<MonsterArea>();
        if (monsterArea == null) monsterArea = selectedArea.AddComponent<MonsterArea>();
        if (monsterArea.enemies == null) monsterArea.enemies = new List<EnemiesList>();
        monsterArea.enemies.Add(new EnemiesList(monsterIdMapper.enemies[monsterId], 1));
    }

    public void SetMusicArea(int musicId)
    {
        SongArea musicArea = selectedArea.GetComponent<SongArea>();
        if (musicArea == null) musicArea = selectedArea.AddComponent<SongArea>();
        musicArea.song = song;
        musicArea.enterSignal = enterSignal;
        musicArea.exitSignal = exitSignal;
        musicArea.areaSong = musicIdMapper.GetMusicFromId(musicId);
    }
}