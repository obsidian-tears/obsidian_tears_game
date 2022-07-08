
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ReactController : MonoBehaviour
{
    [SerializeField] MySignal freezeSignal;
    [SerializeField] MySignal unfreezeSignal;
    [SerializeField] GameObject loadingIndicator;
    [SerializeField] GameObject gameObject;
    [DllImport("__Internal")] private static extern void OpenChest(string chestId, string objectName);
    [DllImport("__Internal")] private static extern void BuyItem(string chestId, string objectName);
    [DllImport("__Internal")] private static extern void EquipItems(string[] itemIds, string objectName);
    [DllImport("__Internal")] private static extern void DefeatMonster(string monsterId, string objectName);


    // ---------------------------
    // Functions Unity will call through events
    // ---------------------------

    // For treasure chests
    public void GetTreasure()
    {
        // TODO use proper id for treasure chest
        const string treasureId = "id";
        const string gameObjectName = "name";
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        OpenChest(treasureId, gameObjectName);
#endif
        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void DefeatMonster()
    {
        // TODO use proper id for treasure chest
        const string monsterId = "id";
        const string gameObjectName = "name";
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        DefeatMonster(monsterId, gameObjectName);
#endif
        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void BuyItem() {
        // TODO use proper id for item
        const string itemId = "id";
        const string gameObjectName = "name";
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        BuyItem(itemId, gameObjectName);
#endif
        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void EquipItems() {
        const string[] itemIds = new string[] {};
        const string gameObjectId = "id";
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        EquipItems(itemIds, gameObjectId);
#endif
        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }


    // ---------------------------
    // Functions React will call upon server response
    // ---------------------------

    // For treasure chests
    public void SetTreasure(string treasureIdentifier, int numGold)
    {
        // TODO set the treasure that is inside the treasure chest
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }

    public void SetMonsterRewards(string[] itemIds, int numGold, int xp) {
        // TODO set monster rewards (including items if any)
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }

    // for generic success
    public void DisplaySuccess(string success) {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }

    // for failure
    public void DisplayError(string error) {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }
}
