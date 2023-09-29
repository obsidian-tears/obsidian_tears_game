using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeHandler : MonoBehaviour
{
    private bool gameMode;
    [SerializeField] CreateModeHandler createModeHandler;
    [SerializeField] GameObject phendrin;
    [SerializeField] GameObject createModeUI;
    [SerializeField] GameObject playModeUI;
    [SerializeField] GameObject freeCam;

    public void StartGame()
    {
        gameMode = true;
        TogglePhendrin(gameMode);
        ToggleUI(gameMode);
        ToggleFreeCam(gameMode);
        createModeHandler.SetActiveMode(EditModeComponent.EditMode.None);
    }

    public void StopGame()
    {
        gameMode = false;
        TogglePhendrin(gameMode);
        ToggleUI(gameMode);
        ToggleFreeCam(gameMode);
        createModeHandler.SetActiveMode(EditModeComponent.EditMode.Default);
    }

    private void ToggleFreeCam(bool gameMode)
    {
        freeCam.SetActive(!gameMode);
    }

    private void ToggleUI(bool gameMode)
    {
        createModeUI.SetActive(!gameMode);
        playModeUI.SetActive(gameMode);
        DontDestroyOnLoadAccessor.Instance.GetDdolGameObjectByName("GameUI(Clone)").SetActive(gameMode);
    }

    private void TogglePhendrin(bool activated)
    {
        phendrin.SetActive(activated);
        // Maybe set color to show ghost of where Phendrin is?
    }

}
