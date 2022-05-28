using UnityEngine;
using PixelCrushers.DialogueSystem;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Exchange;

namespace PixelCrushers.UISSupport
{
    /// <summary>
    /// Adds Lua functions to work with Opsive Ultimate Inventory System. Add this
    /// to the Dialogue Manager.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/Opsive/UIS Lua")]
    public class UISLua : MonoBehaviour
    {
        #region Initialization

        public bool unregisterOnDisable = false;

        protected CurrencyCollection m_TemporaryCurrencyCollection;

        protected virtual void Awake()
        {
            m_TemporaryCurrencyCollection = new CurrencyCollection();
        }

        protected virtual void OnEnable()
        {
            Lua.RegisterFunction("uisGetItemAmount", this, SymbolExtensions.GetMethodInfo(() => uisGetItemAmount(string.Empty, string.Empty)));
            Lua.RegisterFunction("uisAddItem", this, SymbolExtensions.GetMethodInfo(() => uisAddItem(string.Empty, (double)0, string.Empty, string.Empty)));
            Lua.RegisterFunction("uisRemoveItem", this, SymbolExtensions.GetMethodInfo(() => uisRemoveItem(string.Empty, (double)0, string.Empty, string.Empty)));
            Lua.RegisterFunction("uisGetCurrencyAmount", this, SymbolExtensions.GetMethodInfo(() => uisGetCurrencyAmount(string.Empty, string.Empty)));
            Lua.RegisterFunction("uisAddCurrency", this, SymbolExtensions.GetMethodInfo(() => uisAddCurrency(string.Empty, (double)0, string.Empty)));
            Lua.RegisterFunction("uisRemoveCurrency", this, SymbolExtensions.GetMethodInfo(() => uisRemoveCurrency(string.Empty, (double)0, string.Empty)));
        }

        protected virtual void OnDisable()
        {
            if (unregisterOnDisable)
            {
                Lua.UnregisterFunction("uisGetItemAmount");
                Lua.UnregisterFunction("uisAddItem");
                Lua.UnregisterFunction("uisRemoveItem");
                Lua.UnregisterFunction("uisGetCurrencyAmount");
                Lua.UnregisterFunction("uisAddCurrency");
                Lua.UnregisterFunction("uisRemoveCurrency");
            }
        }

        #endregion

        #region Inventory Methods

        /// <summary>
        /// Get the amount of an item in a specified inventory.
        /// </summary>
        /// <param name="itemName">Item definition name.</param>
        /// <param name="inventoryName">Name of GameObject that has Inventory component, or blank to use GameObject tagged Player.</param>
        /// <returns>Amount in inventory.</returns>
        public virtual double uisGetItemAmount(string itemName, string inventoryName)
        {
            var itemDefinition = GetItemDefinition(itemName);
            if (itemDefinition == null) return 0;
            var inventory = GetInventory(inventoryName);
            if (inventory == null) return 0;
            return (double)inventory.GetItemAmount(itemDefinition);
        }

        /// <summary>
        /// Add an amount of an item to a specified inventory.
        /// </summary>
        /// <param name="itemName">Item definition name.</param>
        /// <param name="amount">Amount of item to add.</param>
        /// <param name="inventoryName">Name of GameObject that has Inventory component, or blank to use GameObject tagged Player.</param>
        /// <param name="itemCollectionName">Name of item collection in Inventory, or blank to not specify an item collection.</param>
        public virtual void uisAddItem(string itemName, double amount, string inventoryName, string itemCollectionName)
        {
            var item = GetItem(itemName);
            if (item == null) return;
            var inventory = GetInventory(inventoryName);
            if (inventory == null) return;
            var itemCollection = GetItemCollection(inventory, itemCollectionName);
            var itemInfo = new ItemInfo(new ItemAmount(item, (int)amount), itemCollection);
            if (itemCollection != null)
            {
                itemCollection.AddItem(itemInfo);
            }
            else
            {
                inventory.AddItem(itemInfo);
            }
        }

        /// <summary>
        /// Remove an amount of an item from a specified inventory.
        /// </summary>
        /// <param name="itemName">Item definition name.</param>
        /// <param name="amount">Amount of item to remove.</param>
        /// <param name="inventoryName">Name of GameObject that has Inventory component, or blank to use GameObject tagged Player.</param>
        /// <param name="itemCollectionName">Name of item collection in Inventory, or blank to not specify an item collection.</param>
        public virtual void uisRemoveItem(string itemName, double amount, string inventoryName, string itemCollectionName)
        {
            var itemDefinition = GetItemDefinition(itemName);
            if (itemDefinition == null) return;
            var inventory = GetInventory(inventoryName);
            if (inventory == null) return;
            var itemCollection = GetItemCollection(inventory, itemCollectionName);
            var itemInfo = itemCollection != null
                ? itemCollection.GetItemInfo(itemDefinition, false)
                : inventory.GetItemInfo(itemDefinition, false);
            if (!itemInfo.HasValue) return;
            var itemInfoToRemove = ((int)amount, itemInfo.Value);
            if (itemCollection != null)
            {
                itemCollection.RemoveItem(itemInfoToRemove);
            }
            else
            {
                inventory.RemoveItem(itemInfoToRemove);
            }
        }

        protected virtual ItemDefinition GetItemDefinition(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: No item name specified.");
                return null;
            }
            var itemDefinition = InventorySystemManager.GetItemDefinition(itemName);
            if (itemDefinition == null && DialogueDebug.logWarnings)
            {
                Debug.LogWarning("Dialogue System: Can't find UIS item definition named '" + itemName + "'.");
            }
            return itemDefinition;
        }

        protected virtual Opsive.UltimateInventorySystem.Core.Item GetItem(string itemName)
        {
            var item = InventorySystemManager.CreateItem(itemName);
            if (item == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find UIS item named '" + itemName + "'.");
            }
            return item;
        }

        protected virtual Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory GetInventory(string inventoryName)
        {
            // If inventoryName is blank, use GO tagged Player.
            GameObject subject = null;
            if (string.IsNullOrEmpty(inventoryName))
            {
                subject = GameObject.FindGameObjectWithTag("Player");
                if (subject == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find GameObject tagged 'Player' to access Inventory.");
                    return null;
                }
            }
            else
            {
                subject = PixelCrushers.GameObjectUtility.GameObjectHardFind(inventoryName);
                if (subject == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find GameObject named '" + inventoryName + "' to access Inventory.");
                    return null;
                }
            }
            var inventory = subject.GetComponent<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>() ?? subject.GetComponentInChildren<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>();
            if (inventory == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find Inventory on '" + subject.name + "'.", subject);
                return null;
            }
            return inventory;
        }

        protected virtual ItemCollection GetItemCollection(Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory inventory, string itemCollectionName)
        {
            if (inventory == null || string.IsNullOrEmpty(itemCollectionName)) return null;
            foreach (var itemCollection in inventory.ItemCollectionsReadOnly)
            {
                if (itemCollection.Name == itemCollectionName) return itemCollection;
            }
            return null;
        }

        #endregion

        #region Currency Methods

        /// <summary>
        /// Get the amount of a specified currency owned by a currency owner.
        /// </summary>
        /// <param name="currencyName">Currency name.</param>
        /// <param name="currencyOwnerName">Name of GameObject that has Currency Owner component, or blank to use GameObject tagged Player.</param>
        /// <returns>Amount of currency owned by currency owner.</returns>
        public virtual double uisGetCurrencyAmount(string currencyName, string currencyOwnerName)
        {
            var currency = GetCurrency(currencyName);
            if (currency == null) return 0;
            var currencyOwner = GetCurrencyOwner(currencyOwnerName);
            if (currencyOwner == null) return 0;
            return currencyOwner.CurrencyAmount.GetAmountOf(currency);
        }

        /// <summary>
        /// Add an amount of a currency to a currency owner.
        /// </summary>
        /// <param name="currencyName">Currency name.</param>
        /// <param name="amount">Amount to add.</param>
        /// <param name="currencyOwnerName">Name of GameObject that has Currency Owner component, or blank to use GameObject tagged Player.</param>
        public virtual void uisAddCurrency(string currencyName, double amount, string currencyOwnerName)
        {
            var currency = GetCurrency(currencyName);
            if (currency == null) return;
            var currencyOwner = GetCurrencyOwner(currencyOwnerName);
            if (currencyOwner == null) return;
            currencyOwner.CurrencyAmount.AddCurrency(currency, amount);
        }

        /// <summary>
        /// Remove an amount of a currency from a currency owner.
        /// </summary>
        /// <param name="currencyName">Currency name.</param>
        /// <param name="amount">Amount to remove.</param>
        /// <param name="currencyOwnerName">Name of GameObject that has Currency Owner component, or blank to use GameObject tagged Player.</param>
        public virtual void uisRemoveCurrency(string currencyName, double amount, string currencyOwnerName)
        {
            var currency = GetCurrency(currencyName);
            if (currency == null) return;
            var currencyOwner = GetCurrencyOwner(currencyOwnerName);
            if (currencyOwner == null) return;
            currencyOwner.CurrencyAmount.RemoveCurrency(currency, amount);
        }

        protected virtual Currency GetCurrency(string currencyName)
        {
            if (string.IsNullOrEmpty(currencyName))
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: No currency name specified.");
                return null;
            }
            var currency = InventorySystemManager.GetCurrency(currencyName);
            if (currency == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find currency named '" + currencyName + "'.");
                return null;
            }
            return currency;
        }

        protected virtual CurrencyOwner GetCurrencyOwner(string currencyOwnerName)
        {
            // If currencyOwnerName is blank, use GO tagged Player.
            GameObject subject = null;
            if (string.IsNullOrEmpty(currencyOwnerName))
            {
                subject = GameObject.FindGameObjectWithTag("Player");
                if (subject == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find GameObject tagged 'Player' to access Currency Owner.");
                    return null;
                }
            }
            else
            {
                subject = PixelCrushers.GameObjectUtility.GameObjectHardFind(currencyOwnerName);
                if (subject == null)
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find GameObject named '" + currencyOwnerName + "' to access Currency Owner.");
                    return null;
                }
            }
            var currencyOwner = subject.GetComponent<CurrencyOwner>() ?? subject.GetComponentInChildren<CurrencyOwner>();
            if (currencyOwner == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't find Currency Owner on '" + subject.name + "'.", subject);
                return null;
            }
            return currencyOwner;
        }

        #endregion

    }
}