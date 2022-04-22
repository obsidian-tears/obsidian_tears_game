// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine.Demo
{

    /// <summary>
    /// Simple inventory script for the Quest Machine demo. This script is 
    /// very specific to the demo. Don't use it as a basis for a generalized 
    /// inventory system. However, you can examine how it implements Saver
    /// to save the inventory in saved games, and how it uses the Message
    /// System to handle adding and removing items.
    /// </summary>
    public class DemoInventory : Saver, IMessageHandler
    {
        public enum ItemType { Carrot = 0, Wand = 1, Coin = 2 }

        public static int CarrotSlot { get { return (int)ItemType.Carrot; } }
        public static int WandSlot { get { return (int)ItemType.Wand; } }
        public static int CoinSlot { get { return (int)ItemType.Coin; } }

        [Serializable]
        public class Slot
        {
            public UnityEngine.UI.Button itemButton;
            public UnityEngine.UI.Text countText;
            public UnityEngine.UI.Text useText;

            public int count;
            public int maxCount = 1;
            public bool usable;
        }

        public Slot[] slots;

        public GameObject wandBarrel; // Handles barrel containing magic polymorph wand.

        public int usingIndex { get; private set; }

        public override void Awake()
        {
            base.Awake();
            usingIndex = -1;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            ListenForMessages();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            MessageSystem.RemoveListener(this);
        }

        public void AddItem(int index)
        {
            ModifyItemCount(index, 1);
        }

        public void RemoveItem(int index)
        {
            ModifyItemCount(index, -1);
        }

        public void ModifyItemCount(int index, int delta)
        {
            SetItemCount(index, GetItemCount(index) + delta);
        }

        public void SetItemCount(int index, int count)
        {
            var slot = GetSlot(index);
            if (slot == null) return;
            slot.count = Mathf.Clamp(count, 0, slot.maxCount);
            slot.itemButton.GetComponent<UnityEngine.UI.Image>().enabled = slot.count > 0;
            slot.itemButton.interactable = slot.usable && slot.count > 0;
            slot.countText.enabled = slot.count > 1;
            slot.countText.text = slot.count.ToString();
        }

        public int GetItemCount(int index)
        {
            var slot = GetSlot(index);
            return (slot != null) ? slot.count : 0;
        }

        public void UseItem(int index)
        {
            var slot = GetSlot(index);
            if (slot == null) return;
            var turnOn = usingIndex != index;
            slot.useText.enabled = turnOn;
            usingIndex = turnOn ? index : -1;
        }

        private Slot GetSlot(int index)
        {
            return (0 <= index && index < slots.Length) ? slots[index] : null;
        }

        private void ListenForMessages()
        {
            // Listen for messages for getting and dropping items:
            MessageSystem.AddListener(this, "Get", "Carrot");
            MessageSystem.AddListener(this, "Get", "Wand");
            MessageSystem.AddListener(this, "Get", "Coin");
            MessageSystem.AddListener(this, "Drop", "Carrot");
            MessageSystem.AddListener(this, "Drop", "Wand");
            MessageSystem.AddListener(this, "Drop", "Coin");
            MessageSystem.AddListener(this, "Activate Wand Barrel", string.Empty); // and to activate wand barrel.
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            if (messageArgs.message == "Activate Wand Barrel")
            {
                // Make the wand barrel appear:
                wandBarrel.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                // Modify the item count:
                var count = (messageArgs.firstValue != null) && (messageArgs.firstValue.GetType() == typeof(int)) ? (int)messageArgs.firstValue : 1;
                if (messageArgs.message == "Drop") count = -count;
                var slotIndex = GetSlotIndex(messageArgs.parameter);
                ModifyItemCount(slotIndex, count);
            }
        }

        private int GetSlotIndex(string slotType)
        {
            switch (slotType)
            {
                case "Carrot":
                    return CarrotSlot;
                case "Wand":
                    return WandSlot;
                case "Coin":
                    return CoinSlot;
                default:
                    return -1;
            }
        }

        [Serializable]
        public class SaveData
        {
            public int carrots;
            public int wands;
            public int coins;
        }

        // Called by the Save System when saving a game. Returns a string 
        // containing data we want to include in saved games.
        public override string RecordData()
        {
            var data = new SaveData();
            data.carrots = GetItemCount(CarrotSlot);
            data.wands = GetItemCount(WandSlot);
            data.coins = GetItemCount(CoinSlot);
            return SaveSystem.Serialize(data);
        }

        // Called by the Save System when loading a game. Applies the
        // saved game data to the current state of this script.
        public override void ApplyData(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            var saveData = SaveSystem.Deserialize<SaveData>(data);
            if (saveData == null) return;
            SetItemCount(CarrotSlot, saveData.carrots);
            SetItemCount(WandSlot, saveData.wands);
            SetItemCount(CoinSlot, saveData.coins);
        }
    }
}