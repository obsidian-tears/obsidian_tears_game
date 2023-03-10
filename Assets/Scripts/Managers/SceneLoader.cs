using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
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
        [SerializeField] private InventorySystemManager m_InventoryManagerPrefab;
        [SerializeField] private GameUIManager m_UIPrefab;
        // Start is called before the first frame update
        private void Awake()
        {
            // Instantiate Opsive Inventory Manager
            if (InventorySystemManager.IsNull)
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
            
            // Instantiate main game canvas
            if (!GameUIManager.Exist)
            {
                Debug.Log("Instantiating GameUI!");
                if (m_UIPrefab != null)
                {
                    Instantiate(m_UIPrefab.gameObject);
                }
                else
                {
                    ThrowError("UI Controller");
                    return;
                }
            }
            else Debug.Log("Canvas already found, doing nothing");

            // Destroy this component after manager deploy is done
            Destroy(this);
        }

        private void ThrowError(string componentName)
        {
            Debug.LogError("ERROR! " + componentName +" prefab is null! Terminating...");
        }
    }
}
