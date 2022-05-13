using UnityEngine;

namespace PixelCrushers.UISSupport
{
    using Opsive.UltimateInventorySystem.UI.CompoundElements;

    /// <summary>
    /// Used by the integration demo. Add to the SaveLoadButton, and assign 
    /// the SaveLoadMenu. This redirects the button to open the Pixel Crushers
    /// Save System test menu. The test menu is only intended to be used for
    /// testing the integration, not for production.
    /// </summary>
    [RequireComponent(typeof(ActionButton))]
    public class RedirectSaveLoadButtonToTestMenu : MonoBehaviour
    {
        public GameObject saveLoadMenu;

        private void Start()
        {
            var saveSystemTestMenu = gameObject.AddComponent<SaveSystemTestMenu>();
            saveSystemTestMenu.saveSlot = 0;
            saveSystemTestMenu.menuInputButton = string.Empty; // Will be opened using this button, not direct user input.
            var actionButton = GetComponent<ActionButton>();
            actionButton.OnSubmitE += () => { saveLoadMenu.SetActive(false); saveSystemTestMenu.ToggleMenu(); };
        }
    }
}