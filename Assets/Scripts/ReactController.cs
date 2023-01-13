using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ReactController : MonoBehaviour
{
    [SerializeField]
    MySignal freezeSignal;

    [SerializeField]
    MySignal unfreezeSignal;

    [SerializeField]
    GameObject loadingIndicator;

    [SerializeField]
    GameObject blocker;

    [DllImport("__Internal")]
    private static extern void LoadGame(string objectName);

    [DllImport("__Internal")]
    private static extern void SaveGame(string saveGameData, string objectName);

    [DllImport("__Internal")]
    private static extern void OpenChest(string chestId, string objectName);

    [DllImport("__Internal")]
    private static extern void BuyItem(string shopId, string itemId, string objectName);

    [DllImport("__Internal")]
    private static extern void EquipItems(string[] itemIds, string objectName);

    [DllImport("__Internal")]
    private static extern void DefeatMonster(string monsterId, string objectName);

    // calls the react function to load the game, start loading
    public void SignalLoadGame()
    {
        loadingIndicator.SetActive(true);
        blocker.SetActive(true);

        // call react fx
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        LoadGame(gameObject.name);
#endif

#if UNITY_WEBGL == false || UNITY_EDITOR == true
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
#endif

        Debug.Log("sent load game message");
    }

    // react calls this function, triggering the actual load, end loading
    public void ListenLoadGame(string fromReact)
    {
        Debug.Log (fromReact);
        PixelCrushers.SavedGameData gameData =
            PixelCrushers
                .SaveSystem
                .Deserialize<PixelCrushers.SavedGameData>(fromReact);
        PixelCrushers.SaveSystem.LoadGame (gameData);
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("load the game: " + fromReact);
    }

    // calls the react function to save the game, start loading
    public void SignalSaveGame()
    {
        loadingIndicator.SetActive(true);
        blocker.SetActive(true);

        // get saved game data
        PixelCrushers.SavedGameData gameData =
            PixelCrushers.SaveSystem.RecordSavedGameData();
        string stringData = PixelCrushers.SaveSystem.Serialize(gameData);

        // call react fx
        Debug.Log (stringData);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        SaveGame(stringData, gameObject.name);
#endif

#if UNITY_WEBGL == false || UNITY_EDITOR == true
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
#endif

        Debug.Log("sent save signal: " + stringData);
    }

    // applies the data from react (to refresh inventory) and stops loading
    public void ListenSaveGame(string fromReact)
    {
        // apply saved game data
        PixelCrushers.SavedGameData gameData =
            PixelCrushers
                .SaveSystem
                .Deserialize<PixelCrushers.SavedGameData>(fromReact);
        PixelCrushers.SaveSystem.ApplySavedGameData (gameData);
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("apply save data: " + fromReact);
    }

    // For treasure chests
    public void SignalOpenChest()
    {
        // TODO use proper id for treasure chest
        const string treasureIndex = "id";

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        OpenChest(treasureIndex, gameObject.name);
#endif

        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void ListenOpenChest(string fromReact)
    {
        // TODO set the chest data
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("open chest: " + fromReact);
    }

    public void DefeatMonster()
    {
        // TODO use proper id for treasure chest
        const string monsterId = "id";

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        DefeatMonster(monsterId, gameObject.name);
#endif

        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void ListenDefeatMonster(string fromReact)
    {
        // TODO apply monster rewards
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("defeat monster: " + fromReact);
    }

    public void BuyItem()
    {
        // TODO use proper id for item
        const string itemIndex = "id";

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        BuyItem (itemIndex, gameObject.name);
#endif

        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void EquipItems()
    {
        string[] itemIds = new string[] { };
        const string gameObjectId = "id";

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        EquipItems (itemIds, gameObjectId);
#endif

        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }


    // for generic success
    public void DisplaySuccess(string success)
    {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }

    // for failure
    public void DisplayError(string error)
    {
        // TODO display an error on the screen
        loadingIndicator.SetActive(false);
        unfreezeSignal.Raise();
    }
}
