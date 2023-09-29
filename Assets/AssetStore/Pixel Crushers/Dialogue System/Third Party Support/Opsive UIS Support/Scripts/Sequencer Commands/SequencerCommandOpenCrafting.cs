using UnityEngine;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.UI.Panels;
using Opsive.UltimateInventorySystem.UI.Menus;
using Opsive.UltimateInventorySystem.Crafting;
using Opsive.UltimateInventorySystem.UI.Menus.Crafting;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer command: OpenShop([crafter], [craftMenu], [playerInventory])
    /// Typically used in NPC's Sequence. Opens crafting menu.
    /// 
    /// Parameters:
    /// - crafter: GameObject with Crafter. Default: speaker.
    /// - craftMenu: GameObject with CraftingMenu. Default: CraftingMenu in scene.
    /// - playerInventory: GameObject with player's Inventory. Default: listener.
    /// </summary>
    public class SequencerCommandOpenCrafting : SequencerCommand
    {

        public void Awake()
        {
            var crafterSubject = GetSubject(0, speaker);
            var crafter = (crafterSubject != null) ? crafterSubject.GetComponent<Crafter>() : null;
            var shopMenuTransform = !string.IsNullOrEmpty(GetParameter(1)) ? GetSubject(1) : null;
            var craftingMenu = (shopMenuTransform != null) ? shopMenuTransform.GetComponent<CraftingMenu>() : null;
            if (craftingMenu == null) craftingMenu = FindObjectOfType<CraftingMenu>();
            if (craftingMenu == null)
            {
                var shopMenuObject = GameObjectUtility.GameObjectHardFind("Crafting Menu");
                if (shopMenuObject != null) craftingMenu = shopMenuObject.GetComponent<CraftingMenu>();
            }
            var panelManager = (craftingMenu != null) ? craftingMenu.GetComponentInParent<DisplayPanelManager>() : null;
            var player = GetSubject(2, listener);
            var playerInventory = (player != null) ? player.GetComponentInChildren<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>() : null;
            if (playerInventory == null)
            {
                var playerObject = GameObject.FindWithTag("Player");
                if (playerObject != null) player = playerObject.transform;
                playerInventory = (player != null) ? player.GetComponentInChildren<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>() : null;
            }

            if (crafter == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: OpenCrafting(" + GetParameters() + ") can't find crafter.");
            }
            else if (panelManager == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: OpenCrafting(" + GetParameters() + ") can't find panel manager parent of crafter menu.");
            }
            else if (playerInventory == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Sequencer: OpenCrafting(" + GetParameters() + ") can't find player inventory.");
            }
            else
            {
                if (DialogueDebug.logInfo) Debug.Log("Dialogue System: Sequencer: OpenCrafting(" + GetParameters() + ")");
                var craftingMenuOpener = crafter.GetComponent<CraftingMenuOpener>();
                if (craftingMenuOpener != null)
                {
                    craftingMenuOpener.Open(playerInventory);
                }
                else
                {
                    craftingMenu.BindInventory(playerInventory);
                    craftingMenu.SetCrafter(crafter);
                    craftingMenu.DisplayPanel.SmartOpen();
                }
            }

            Stop();
        }
    }
}
