using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers;
using GameManagers;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button m_LoadButton;
    [Space(5)]
    [SerializeField] private GameObject m_InitScreen;
    [SerializeField] private GameObject m_LoadingScreen;
    //private GameObject m_GameUi;
    [SerializeField] private string url = "https://toniq.io/marketplace/obsidian-tears-items";

    private void Awake()
    {
        Debug.Log("AWAKE!");
        // ReactController.Instance.OnLoadGameCheckDone += OnLoadGameCheckDone;
        // ReactController.Instance.SignalCheckForLoadedGame();
        // SetInitScreen(true);
    }


    private void Start()
    {
        GameObject m_GameUi = GameObject.Find("GameUI(Clone)");
        Debug.Log("gameui" + m_GameUi); 
        if (m_GameUi != null)
        {

            m_GameUi.SetActive(false);
        }
        else
        {
            return;
        }
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

    public void OpenItemShop()
    {

        Application.OpenURL(url);

    }


    public void ClickLoadGame()
    {
        GameObject m_GameUi = GameObject.Find("GameUI(Clone)");
        m_GameUi.SetActive(true);


    }



}
