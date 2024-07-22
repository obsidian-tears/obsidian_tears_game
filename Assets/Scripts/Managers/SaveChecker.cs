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
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ChangeLoadState()
    {
        HasToLoad = false;
    }
}
