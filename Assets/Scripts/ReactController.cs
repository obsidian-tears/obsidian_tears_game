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

[Serializable]
public class RewardInfo
{
    public string[] itemIds;
    public int gold;
    public int xp;
}

public class ReactController : MonoBehaviour
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

    [SerializeField]
    GameObject loadingIndicator;

    [SerializeField]
    GameObject blocker;

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

    [DllImport("__Internal")]
    private static extern void NewGame(string objectName);

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    /// <summary>
    /// Reaction to Load game signal, calls react method or local one when running from editor
    /// </summary>
    public void SignalLoadGame()
    {
        Debug.Log("LOADING START!");
        ShowLoadingIndicator(true, true);

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
            ShowLoadingIndicator(false, true);
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
        SavedGameData gameData = SaveSystem.Deserialize<PixelCrushers.SavedGameData>(fromReact);
        SaveSystem.LoadGame(gameData);
    }

    /// <summary>
    /// Reaction to Save game signal, calls react method or local one when running from editor
    /// </summary>
    public void SignalSaveGame()
    {
        // freeze
        ShowLoadingIndicator(true, true);
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
        //TODO show proper save/load screen
        ShowLoadingIndicator(false, true);
        Debug.Log("SAVING SUCCESS, GAME DATA:" + fromReact);
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
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        OpenChest((int)Math.Round(double.Parse(treasureIndex)), gameObject.name);
#endif

        freezeSignal.Raise();
        ShowLoadingIndicator(true, false);
    }

    public void ListenOpenChest(string fromReact)
    {
        handleReward(fromReact);
        ShowLoadingIndicator(false, true);
        Debug.Log("open chest: " + fromReact);
    }

    public void SignalDefeatMonster(string monsterId)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        DefeatMonster(int.Parse(monsterId), gameObject.name);
#endif

        freezeSignal.Raise();
        ShowLoadingIndicator(true, false);
    }

    public void SignalNewGame()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        NewGame(gameObject.name);
#endif

        //freezeSignal.Raise();
        //loadingIndicator.SetActive(true);
    }

    public void ListenBuyItem(string fromReact)
    {
        // TODO WHAT ABOUT SOME HANDLE REWARD HERE?? OR IS IT HANDLED ELSEWHERE?
        ShowLoadingIndicator(false, true);
        Debug.Log("bought item: " + fromReact);
    }

    public void ListenDefeatMonster(string fromReact)
    {
        handleReward(fromReact);
        ShowLoadingIndicator(false, true);

        Debug.Log("defeated monster: " + fromReact);
    }

    public void SignalBuyItem(string shopIndex, string itemDefId, int quantity)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        BuyItem(int.Parse(shopIndex), itemDefId, quantity, gameObject.name);
#endif

        freezeSignal.Raise();
        ShowLoadingIndicator(true, false);
    }

    public void EquipItems()
    {
        string[] itemIds = new string[] { };

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        EquipItems(itemIds, gameObject.name);
#endif

        freezeSignal.Raise();
        ShowLoadingIndicator(true, false);
    }

    // for generic success
    public void DisplaySuccess(string success)
    {
        // TODO display an error on the screen
        ShowLoadingIndicator(false, false);
        unfreezeSignal.Raise();
    }

    // for failure
    public void DisplayError(string error)
    {
        // TODO display an error on the screen
        ShowLoadingIndicator(false, false);
        unfreezeSignal.Raise();
    }

    public void handleReward(string fromReact)
    {
        RewardInfo data = JsonUtility.FromJson<RewardInfo>(fromReact);
        uint[] items = Array.ConvertAll(data.itemIds, uint.Parse);
        Dictionary<uint, int> dictionary = items.GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());
        var gold = data.gold;

        GiveItems(dictionary, gold);
    }

    public void GiveItems(Dictionary<uint, int> items, int goldAmount)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Inventory inv = player.GetComponent<Inventory>();
        foreach (uint key in items.Keys)
        {
            var itemDefinition =
                InventorySystemManager.GetItemDefinition(key);
            inv.AddItem(itemDefinition, items[key]);
        }
        if (goldAmount != 0)
        {
            var currencyOwner =
                inv.GetCurrencyComponent<CurrencyCollection>() as CurrencyOwner;
            var ownerCurrencyCollection = currencyOwner.CurrencyAmount;
            var gold = InventorySystemManager.GetCurrency("Gold");
            ownerCurrencyCollection.AddCurrency(gold, goldAmount);
        }
    }

    // TODO this should be ideally move to the dedicated UI script
    private void ShowLoadingIndicator(bool show, bool involveBlocker)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(show);
        }

        if (involveBlocker && blocker != null)
        {
            blocker.SetActive(show);
        }
    }
}
