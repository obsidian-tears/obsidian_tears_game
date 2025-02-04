using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
using PixelCrushers.Wrappers;
using UnityEngine;

namespace GameManagers
{
    /// <summary>
    /// Main game managers loading is happening here. It is called before anything else 
    /// and must be part of every scene!
    /// All components instantiated here are set to Dont destroy on load, so only one instance of it persists within the scene changes
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool m_loadInventoryManager;
        [SerializeField] private bool m_loadReactController;
        [SerializeField] private bool m_loadSaveSystem;
        [SerializeField] private bool m_loadInGameUI;
        [SerializeField] private InventorySystemManager m_InventoryManagerPrefab;
        [SerializeField] private ReactController m_ReactControllerPrefab;
        [SerializeField] private SaveSystem m_SaveSystemPrefab;
        [SerializeField] private GameUIManager m_UIPrefab;
        // Start is called before the first frame update
        private void Awake()
        {
            // Instantiate Opsive Inventory Manager
            if (m_loadInventoryManager && InventorySystemManager.IsNull)
            {
                Debug.Log("Instantiating Inventory system!");
                if (m_InventoryManagerPrefab != null)
                {
                    Instantiate(m_InventoryManagerPrefab.gameObject);
                }
                else
                {
                    ThrowError("Inventory System Manager");
                    return;
                }
            }
            else Debug.Log("Inventory already found, doing nothing");

            if (m_loadSaveSystem && !SaveSystem.hasInstance)
            {
                Debug.Log("Instantiating Save system!");
                if (m_SaveSystemPrefab != null)
                {
                    Instantiate(m_SaveSystemPrefab.gameObject);
                }
                else
                {
                    ThrowError("SaveSystem");
                    return;
                }
            }
            else Debug.Log("Save system already found, doing nothing");
            
            // Instantiate main game canvas
            if (m_loadInGameUI && !GameUIManager.Exist)
            {
                Debug.Log("Instantiating GameUI!");
                if (m_UIPrefab != null)
                {
                    Instantiate(m_UIPrefab.gameObject);
                    // var beginningStats = FindObjectOfType<BeginningStats>();
                    // beginningStats.slotCollectionView = ui.GetComponentInChildren<ItemSlotCollectionView>();
                }
                else
                {
                    ThrowError("UI Controller");
                    return;
                }
            }
            else Debug.Log("Canvas already found, doing nothing");

            // Instantiate ReactController
            if (m_loadReactController && !ReactController.Exist)
            {
                Debug.Log("Instantiating React controller!");
                if (m_ReactControllerPrefab != null)
                {
                    Instantiate(m_ReactControllerPrefab.gameObject);
                }
                else
                {
                    ThrowError("React Controller");
                    return;
                }
            }
            else Debug.Log("React controller already found, doing nothing");
        }

        private void Start()
        {
            //If we need some special stuff to do on scene load, ad it to this OnSceneLoaded scene
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void ThrowError(string componentName)
        {
            Debug.LogError("ERROR! " + componentName +" prefab is null! Terminating...");
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            Time.timeScale = 1;
            GameUIManager.Instance.ShowLoadingIndicator(false, true);
        }
    }
}
