using PixelCrushers.QuestMachine.Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveChecker : MonoBehaviour
{
    public static SaveChecker Instance;
    public bool HasToLoad = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("soy el save checker");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("soy el save checker 2");

    }

    public void ChangeLoadState()
    {
        HasToLoad = false;
    }
}
