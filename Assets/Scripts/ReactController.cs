using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using UnityEngine;
using System.Linq;
using PixelCrushers;
using Core;
using GameManagers;
using PixelCrushers.DialogueSystem;

[Serializable]
public class RewardInfo
{
    public string[] itemIds;
    public int gold;
    public int xp;
}

public class ReactController : MonoSingleton<ReactController>
{
    // TODO probably adding timeout variable would be beneficial
    [Header("Testing/Setup")]
    [Tooltip("For testing purposes - on which save system slot should be game saved locally?")]
    [SerializeField] private int m_localSaveSlotNumber;
    [Tooltip("For testing purposes - how long should be save/load screen shown when testing game in editor?")]
    [SerializeField] private float m_saveLoadScreenShowTime;
    [Space(5)]
    [Header("References")]
    [SerializeField]
    MySignal freezeSignal;
    [SerializeField]
    MySignal unfreezeSignal;

    public event Action<bool> OnLoadGameCheckDone;
    
    // TODO Uncomment this for cooperation with react script!
    // [DllImport("__Internal")]
    // private static extern void CheckForLoadGame();

    [DllImport("__Internal")]
    private static extern void LoadGame(string objectName);

    [DllImport("__Internal")]
    private static extern void SaveGame(string saveGameData, string objectName);

    [DllImport("__Internal")]
    private static extern void OpenChest(int chestId, string objectName);

    [DllImport("__Internal")]
    private static extern void BuyItem(
        int shopId,
        string itemDefId,
        int qty,
        string objectName
    );

    [DllImport("__Internal")]
    private static extern void EquipItems(string[] itemIds, string objectName);

    [DllImport("__Internal")]
    private static extern void DefeatMonster(
        int monsterId,
        string objectName
    );

    protected override void Init()
    {
        Debug.Log("REACT controller INIT!");
    }

    public void SignalCheckForLoadedGame()
    {
        Debug.Log("REACT CHECK FOR LOADED GAME");
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        // call react fx
        // TODO Uncomment this for react script communication
        //CheckForLoadGame();
        //TODO delete this later
        StartCoroutine(ServerCheckLoadGameSimulation());
#else
        // else check saved data locally
        StartCoroutine(LocalCheckLoadGameSimulation());
#endif
    }

    private IEnumerator LocalCheckLoadGameSimulation()
    {
        yield return new WaitForSecondsRealtime(m_saveLoadScreenShowTime);
        OnLoadGameCheckDone?.Invoke(SaveSystem.HasSavedGameInSlot(m_localSaveSlotNumber));
        if (SaveSystem.HasSavedGameInSlot(m_localSaveSlotNumber))
        {
            SignalLoadGame();
        }
    }

    private IEnumerator ServerCheckLoadGameSimulation()
    {
        yield return new WaitForSecondsRealtime(m_saveLoadScreenShowTime);
        ListenCheckLoadGame(true);
    }

    //TODO make react communicate with this
    public void ListenCheckLoadGame(bool fromReact)
    {
        Debug.Log("REACT LOAD GAME CHECKED, HAS BEEN FOUND? " + fromReact);
        OnLoadGameCheckDone(fromReact);
        if (fromReact)
        {
            SignalLoadGame();
        }       
        
    }

    /// <summary>
    /// Reaction to Load game signal, calls react method or local one when running from editor
    /// </summary>
    public void SignalLoadGame()
    {
        Debug.Log("LOADING START!");
        // GameUIManager.Instance.ShowLoadingIndicator(true, true);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        // call react fx
        // TODO why are we sending gameobject name to the server, is it needed?
        // TODO probably adding timeout would be beneficial
        LoadGame(gameObject.name);
#else
        // else save data locally for testing purposes
        StartCoroutine(LocalLoadGameSimulation());
#endif
    }

    private IEnumerator LocalLoadGameSimulation()
    {
        yield return new WaitForSecondsRealtime(m_saveLoadScreenShowTime);
        SavedGameData savedData = SaveSystem.storer.RetrieveSavedGameData(m_localSaveSlotNumber);
        if (savedData == null)
        {
            Debug.LogError("LOADING ERROR, NO DATA FOUND!");
            GameUIManager.Instance.ShowLoadingIndicator(false, true);
            yield break;
        }

        string stringData = SaveSystem.Serialize(savedData);
        ListenLoadGame(stringData);
    }

    /// <summary>
    /// React load game callback, triggering the actual load
    /// </summary>
    /// <param name="fromReact">String with player data</param>
    public void ListenLoadGame(string fromReact)
    {
        Debug.Log("LOADING, DATA TO LOAD: " + fromReact);
        if(fromReact != "{}") {
            SavedGameData gameData = SaveSystem.Deserialize<PixelCrushers.SavedGameData>(fromReact);
            SaveSystem.LoadGame(gameData);
        }
    }

    /// <summary>
    /// Reaction to Save game signal, calls react method or local one when running from editor
    /// </summary>
    public void SignalSaveGame()
    {
        // freeze
        GameUIManager.Instance.ShowLoadingIndicator(true, true);
        Autosave.Instance.RestartAutosaveCoroutine();

        // get saved game data
        SavedGameData gameData = SaveSystem.RecordSavedGameData();
        string stringData = SaveSystem.Serialize(gameData);

        Debug.Log("SAVING, GAME DATA:" + stringData);
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        // call react 
        // TODO timeout would be beneficial
        SaveGame(stringData, gameObject.name);
#else
        // else save data locally for testing purposes
        StartCoroutine(LocalSaveGameSimulation(gameData, stringData));
#endif
    }

    // Callback to react-related save game
    // TODO - probably should receive just simple bool/int to make sure operation was successful
    // TODO - show some error message in case something went wrong
    public void ListenSaveGame(string fromReact)
    {
        // TODO show proper save/load screen
        GameUIManager.Instance.ShowLoadingIndicator(false, true);
        Debug.Log("SAVING SUCCESS, GAME DATA:" + fromReact);
        GameUIManager.Instance.ShowGameSavedSuccesfull(); // this shows message in game that game has been saved so players notice when this is done correctly.
    }

    private IEnumerator LocalSaveGameSimulation(SavedGameData gameData, string stringData)
    {
        SaveSystem.storer.StoreSavedGameData(m_localSaveSlotNumber, gameData);
        yield return new WaitForSecondsRealtime(m_saveLoadScreenShowTime);
        ListenSaveGame(stringData);
    }

    // For treasure chests
    public void SignalOpenChest(string treasureIndex)
    {
        Debug.Log("Deprecated Signal Open Chest was called");
        
        // #if UNITY_WEBGL == true && UNITY_EDITOR == false
        // OpenChest((int)Math.Round(double.Parse(treasureIndex)), gameObject.name);
        // #endif

        // freezeSignal.Raise();
        // GameUIManager.Instance.ShowLoadingIndicator(true, false);
    }

    public void ListenOpenChest(string fromReact)
    {
        Debug.Log("Deprecated Listen Open Chest was called");
        // Debug.Log("REACT OPEN CHEST CALLBACK. Data from react: " + fromReact);
        // handleReward(fromReact);
        // GameUIManager.Instance.ShowLoadingIndicator(false, true);
    }

    public void SignalDefeatMonster(string monsterId)
    {
        Debug.Log("REACT DEFEAT MONSTER was called.");
        // #if UNITY_WEBGL == true && UNITY_EDITOR == false
        // DefeatMonster(int.Parse(monsterId), gameObject.name);
        // freezeSignal.Raise();
        // GameUIManager.Instance.ShowLoadingIndicator(true, false);        
        // #endif
    }

    public void ListenBuyItem(string fromReact)
    {
        // TODO: remove from all callers and then delete event
        Debug.Log("(event deprecated) buy item: " + fromReact);
    }

    public void ListenDefeatMonster(string fromReact)
    {
        // handleReward(fromReact);
        // GameUIManager.Instance.ShowLoadingIndicator(false, true);

        Debug.Log("deprecated defeated monster was reached");
    }

    public void SignalBuyItem(string shopIndex, string itemDefId, int quantity)
    {
        // TODO: remove from all callers and then delete event
        Debug.Log("Buy Item - Event Deprecated");        
    }

    public void EquipItems()
    {
        // TODO: remove from all callers and then delete event
        Debug.Log("EquipItems - Event Deprecated");
    }

    // for generic success
    public void DisplaySuccess(string success)
    {
        // TODO display an error on the screen
        GameUIManager.Instance.ShowLoadingIndicator(false, false);
        unfreezeSignal.Raise();
    }

    // for failure
    public void DisplayError(string error)
    {
        // TODO display an error on the screen
        GameUIManager.Instance.ShowLoadingIndicator(false, false);
        unfreezeSignal.Raise();
    }

    public void handleReward(string fromReact)
    {
        Debug.Log("REACT HANDLING REWARD METHOD, give items should follow.");
        RewardInfo data = JsonUtility.FromJson<RewardInfo>(fromReact);
        uint[] items = Array.ConvertAll(data.itemIds, uint.Parse);
        Dictionary<uint, int> dictionary = items.GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());
        var gold = data.gold;

        GiveItems(dictionary, gold);
    }

    public void GiveItems(Dictionary<uint, int> items, int goldAmount)
    {
        Debug.Log("REACT GIVE ITEMS METHOD, obtained gold: " + goldAmount);
        Debug.Log("REACT GIVE ITEMS METHOD, list of obtained keys should follow");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inv = player.GetComponent<Inventory>();
        foreach (uint key in items.Keys)
        {
            Debug.Log("REACT GIVE ITEMS METHOD, obtaining item key: " + key);
            var itemDefinition =
                InventorySystemManager.GetItemDefinition(key);
            inv.AddItem(itemDefinition, items[key]);
        }
        if (goldAmount != 0)
        {
            CurrencyOwner currencyOwner = inv.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner;
            CurrencyCollection ownerCurrencyCollection = currencyOwner.CurrencyAmount;
            Currency gold = InventorySystemManager.GetCurrency("Gold");
            ownerCurrencyCollection.AddCurrency(gold, goldAmount);

            DialogueManager.ShowAlert("Found " + goldAmount + " gold");
        }
    }
}
