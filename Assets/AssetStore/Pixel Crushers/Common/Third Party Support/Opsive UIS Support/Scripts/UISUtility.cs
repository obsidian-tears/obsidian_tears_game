using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;
using UnityEngine;

namespace PixelCrushers.UISSupport
{
    /// <summary>
    /// Utility functions for working with Opsive Ultimate Inventory System.
    /// </summary>
    public static class UISUtility
    {
        /// <summary>
        /// Set true to log warnings.
        /// </summary>
        public static bool debug = false;

        #region Inventory

        public static ItemDefinition GetItemDefinition(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                if (debug) Debug.LogWarning("UISUtility.GetItemDefinition: No item name specified.");
                return null;
            }
            var itemDefinition = InventorySystemManager.GetItemDefinition(itemName);
            if (itemDefinition == null && debug)
            {
                Debug.LogWarning("UISUtility.GetItemDefinition: Can't find UIS item definition named '" + itemName + "'.");
            }
            return itemDefinition;
        }

        public static Item GetItem(string itemName)
        {
            var item = InventorySystemManager.CreateItem(itemName);
            if (item == null)
            {
                if (debug) Debug.LogWarning("UISUtility.GetItem: Can't find UIS item named '" + itemName + "'.");
            }
            return item;
        }

        public static Inventory GetInventory(string inventoryName)
        {
            // If inventoryName is blank, use GO tagged Player.
            if (string.IsNullOrEmpty(inventoryName))
            {
                var playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
                if (playerGameObjects.Length == 0)
                {
                    if (debug) Debug.LogWarning("UISUtility.GetInventory: Can't find GameObject tagged 'Player' to access Inventory.");
                    return null;
                }
                else
                {
                    foreach (GameObject go in playerGameObjects)
                    {
                        var inventory = go.GetComponent<Inventory>() ?? go.GetComponentInChildren<Inventory>();
                        if (inventory != null) return inventory;
                    }
                    return null;
                }
            }
            // Otherwise search for inventoryName GameObject:
            else
            {
                var subject = PixelCrushers.GameObjectUtility.GameObjectHardFind(inventoryName);
                if (subject == null)
                {
                    if (debug) Debug.LogWarning("UISUtility.GetInventory: Can't find GameObject named '" + inventoryName + "' to access Inventory.");
                    return null;
                }
                else
                {
                    var inventory = subject.GetComponent<Inventory>() ?? subject.GetComponentInChildren<Inventory>();
                    if (inventory == null) 
                    {
                        if (debug) Debug.LogWarning("UISUtility.GetInventory: Can't find Inventory on '" + subject.name + "'.", subject);
                        return null;
                    }
                    return inventory;
                }
            }
        }

        public static ItemCollection GetItemCollection(Inventory inventory, string itemCollectionName)
        {
            if (inventory == null || string.IsNullOrEmpty(itemCollectionName)) return null;
            foreach (var itemCollection in inventory.ItemCollectionsReadOnly)
            {
                if (itemCollection.Name == itemCollectionName) return itemCollection;
            }
            return null;
        }

        #endregion

        #region Currency

        public static Currency GetCurrency(string currencyName)
        {
            if (string.IsNullOrEmpty(currencyName))
            {
                if (debug) Debug.LogWarning("UISUtility.GetCurrency: No currency name specified.");
                return null;
            }
            var currency = InventorySystemManager.GetCurrency(currencyName);
            if (currency == null)
            {
                if (debug) Debug.LogWarning("UISUtility.GetCurrency: Can't find currency named '" + currencyName + "'.");
                return null;
            }
            return currency;
        }

        public static CurrencyOwner GetCurrencyOwner(string currencyOwnerName)
        {
            // If currencyOwnerName is blank, use GO tagged Player.
            GameObject subject = null;
            if (string.IsNullOrEmpty(currencyOwnerName))
            {
                subject = GameObject.FindGameObjectWithTag("Player");
                if (subject == null)
                {
                    if (debug) Debug.LogWarning("UISUtility.GetCurrencyOwner: Can't find GameObject tagged 'Player' to access Currency Owner.");
                    return null;
                }
            }
            else
            {
                subject = PixelCrushers.GameObjectUtility.GameObjectHardFind(currencyOwnerName);
                if (subject == null)
                {
                    if (debug) Debug.LogWarning("UISUtility.GetCurrencyOwner: Can't find GameObject named '" + currencyOwnerName + "' to access Currency Owner.");
                    return null;
                }
            }
            var currencyOwner = subject.GetComponent<CurrencyOwner>() ?? subject.GetComponentInChildren<CurrencyOwner>();
            if (currencyOwner == null)
            {
                if (debug) Debug.LogWarning("UISUtility.GetCurrencyOwner: Can't find Currency Owner on '" + subject.name + "'.", subject);
                return null;
            }
            return currencyOwner;
        }

        #endregion

    }
}
