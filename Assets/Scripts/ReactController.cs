using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using UnityEngine;
using System.Linq; 

[Serializable]
public class RewardInfo
{
    public string[] itemIds;
    public int gold;
    public int xp;
}

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
        string monsterId,
        string objectName
    );

    [DllImport("__Internal")]
    private static extern void NewGame(string objectName);

    // calls the react function to load the game, start loading
    public void SignalLoadGame()
    {
        loadingIndicator.SetActive(true);
        blocker.SetActive(true);

        // call react fx
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        LoadGame(gameObject.name);
#endif
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
        // freeze
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
    public void SignalOpenChest(string treasureIndex)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        OpenChest((int)Math.Round(double.Parse(treasureIndex)), gameObject.name);
#endif


        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void ListenOpenChest(string fromReact)
    {
        handleReward(fromReact);
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("open chest: " + fromReact);
    }

    public void SignalDefeatMonster(string monsterId)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        DefeatMonster(int.parse(monsterId), gameObject.name);
#endif


        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
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
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);
        Debug.Log("bought item: " + fromReact);
    }

    public void ListenDefeatMonster(string fromReact)
    {
        handleReward(fromReact);
        loadingIndicator.SetActive(false);
        blocker.SetActive(false);

        // TODO give player items parsed from fromReact
        Debug.Log("defeated monster: " + fromReact);
    }

    public void SignalBuyItem(string shopIndex, string itemDefId, int quantity)
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        BuyItem(int.Parse(shopIndex), itemDefId, quantity, gameObject.name);
#endif


        freezeSignal.Raise();
        loadingIndicator.SetActive(true);
    }

    public void EquipItems()
    {
        string[] itemIds = new string[] { };


#if UNITY_WEBGL == true && UNITY_EDITOR == false
        EquipItems(itemIds, gameObject.name);
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

    public void handleReward(string fromReact) {
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
            ownerCurrencyCollection.AddCurrency (gold, goldAmount);
        }
    }
}
