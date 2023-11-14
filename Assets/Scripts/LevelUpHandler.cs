using UnityEngine;

public class LevelUpHandler : MonoBehaviour
{
    public LevelManager levelManager;
    public LevelUpMenu levelUpMenu;
    public CharStatsSaver charStatsSaver;

    [SerializeField]
    public int pointsToLevelUp = 10;

    private bool isProcessingLevelUp = false;

    private void Start()
    {
        // Ensure necessary components are assigned.
        if (levelManager == null || levelUpMenu == null || charStatsSaver == null)
        {
            Debug.LogError("LevelUpHandler: Ensure LevelManager, LevelUpMenu, and CharStatsSaver are assigned.");
            return;
        }

        // Register the level-up event to trigger the level-up logic.
        DialogueManager.Instance.OnDialogueEvent += HandleDialogueEvent;
    }

    private void HandleDialogueEvent(string eventName)
    {
        // Check if the dialogue event corresponds to triggering a level-up.
        if (eventName.Equals("TriggerLevelUp"))
        {
            if (!isProcessingLevelUp)
            {
                isProcessingLevelUp = true;

                // Check if the user has enough points to level up.
                if (levelManager.getLevel(PlayerType.Fighter, 1, pointsToLevelUp).level > 0)
                {
                    // Perform level-up logic.
                    PerformLevelUp();
                }
                else
                {
                    Debug.Log("Not enough points to level up.");
                }

                isProcessingLevelUp = false;
            }
            else
            {
                Debug.Log("Level-up is already in progress. Avoiding infinite loop.");
            }
        }
    }

    private void PerformLevelUp()
    {
        // Get the current character stats.
        CharStats charStats = GetComponent<CharStats>();

        // Update the character stats.
        int currentLevel = charStats.level;
        BattleResult battleResult = levelManager.getLevel(PlayerType.Fighter, currentLevel, pointsToLevelUp);

        // Apply the level-up changes to the character.
        charStats.level = currentLevel + battleResult.level;
        charStats.pointsRemaining = battleResult.pointsRemaining;

        // Save the updated character stats.
        charStatsSaver.RecordData();

        // Show the level-up menu.
        levelUpMenu.gameObject.SetActive(true);
        levelUpMenu.ResetMenu();

        // Update the UI with the new stats.
        levelUpMenu.SetNewLevels();
        levelUpMenu.SetPickerQuantity();
        levelUpMenu.SaveChanges();

        // Update other UI elements or perform additional logic as needed.

        Debug.Log("Level up complete!");
    }
}
