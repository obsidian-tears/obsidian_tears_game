
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ReactController : MonoBehaviour
{
    [SerializeField] MySignal freezeSignal;
    [SerializeField] MySignal unfreezeSignal;
    [SerializeField] GameObject loadingIndicator;
    [DllImport("__Internal")] private static extern void LoadGame(string objectName);
    [DllImport("__Internal")] private static extern void SaveGame(string saveGameData, string objectName);
    [DllImport("__Internal")] private static extern void OpenChest(string chestId, string objectName);
    [DllImport("__Internal")] private static extern void BuyItem(string chestId, string objectName);
    [DllImport("__Internal")] private static extern void EquipItems(string[] itemIds, string objectName);
    [DllImport("__Internal")] private static extern void DefeatMonster(string monsterId, string objectName);

    // ---------------------------
    // Loading/Saving Functions
    // ---------------------------

    // calls the react function to load the game, start loading
    public void SignalLoadGame() {
        loadingIndicator.SetActive(true);
        // call react fx
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        LoadGame(gameObject.name);
#endif
#if UNITY_WEBGL == false || UNITY_EDITOR == true
        loadingIndicator.SetActive(false);
#endif
        // TODO start loading
        Debug.Log("sent load game message");
    }

    // react calls this function, triggering the actual load, end loading
    public void ListenLoadGame(string fromReact) {
        PixelCrushers.SavedGameData gameData = PixelCrushers.SaveSystem.Deserialize<PixelCrushers.SavedGameData>(fromReact);
        PixelCrushers.SaveSystem.LoadGame(gameData);
        loadingIndicator.SetActive(false);
        Debug.Log("load the game: " + fromReact);

    }

    // calls the react function to save the game, start loading
    public void SignalSaveGame() {
        loadingIndicator.SetActive(true);
        // get saved game data
        PixelCrushers.SavedGameData gameData = PixelCrushers.SaveSystem.RecordSavedGameData();
        string stringData = PixelCrushers.SaveSystem.Serialize(gameData);
        // call react fx
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        SaveGame(stringData, gameObject.name);
#endif
#if UNITY_WEBGL == false || UNITY_EDITOR == true
        loadingIndicator.SetActive(false);
#endif
        Debug.Log("sent save signal: " + stringData);

    }

    // applies the data from react (to refresh inventory) and stops loading
    public void ListenSaveGame(string fromReact) {
        // apply saved game data
        PixelCrushers.SavedGameData gameData = PixelCrushers.SaveSystem.Deserialize<PixelCrushers.SavedGameData>(fromReact);
        PixelCrushers.SaveSystem.ApplySavedGameData(gameData);
        loadingIndicator.SetActive(false);
        Debug.Log("apply save data: " + fromReact);
    }



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
        string[] itemIds = new string[] {};
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
