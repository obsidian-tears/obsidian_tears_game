using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button m_LoadButton;
    [Space(5)]
    [SerializeField] private GameObject m_InitScreen;
    [SerializeField] private GameObject m_LoadingScreen;

    private void Awake()
    {
        Debug.Log("AWAKE!");
        // ReactController.Instance.OnLoadGameCheckDone += OnLoadGameCheckDone;
        // ReactController.Instance.SignalCheckForLoadedGame();
        // SetInitScreen(true);
    }

    private void OnDestroy()
    {
        // ReactController.Instance.OnLoadGameCheckDone -= OnLoadGameCheckDone;
    }

    public void OnLoadGameCheckDone(bool loadGameFound)
    {
        m_LoadButton.interactable = loadGameFound;
        SetInitScreen(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetInitScreen(bool isShown)
    {
        if (m_InitScreen != null)
        {
            m_InitScreen.SetActive(isShown);
        }
    }

    public void SetLoadingScreen(bool isShown)
    {
        if (m_LoadingScreen != null)
        {
            m_LoadingScreen.SetActive(isShown);
        }
    }
}
