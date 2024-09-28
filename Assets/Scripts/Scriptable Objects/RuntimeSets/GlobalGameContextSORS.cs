using System.Collections;
using System.Collections.Generic;
using GameManagers;
using PixelCrushers;
using PixelCrushers.QuestMachine;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Global Game Context Accessor")]
public class GlobalGameContextSORS : ScriptableObject
{
    private Player m_player;
    private QuestJournal m_questJournal;

    /// <summary>
    /// Registers the player. Don't forget do to this in Player's Awake method!
    /// </summary>
    /// <param name="player"></param>
    public void RegisterPlayerObject(Player player)
    {
        m_player = player;
        m_questJournal = m_player.GetComponent<QuestJournal>();
    }

    public void SaveGameReact()
    {
        if (SaveLoadConfirmation.Instance && SaveLoadConfirmation.Instance.showSaveConfirmation)
            SaveLoadConfirmation.Instance.ShowSaveConfirmationPanel();
        else
            ReactController.Instance.SignalSaveGame();
    }

    public void LoadGameReact()
    {
        if (SaveLoadConfirmation.Instance && SaveLoadConfirmation.Instance.showLoadConfirmation)
            SaveLoadConfirmation.Instance.ShowLoadConfirmationPanel();
        else
            ReactController.Instance.SignalLoadGame();
    }

    /// <summary>
    /// TODO Actually those journal-related methods should all go to the UI Manager
    /// </summary>
    /// <returns></returns>
    private bool CheckForJournal()
    {
        bool journalAssigned = m_questJournal != null;
        if (!journalAssigned)
        {
            Debug.LogError("Cannot find quest journal in Global game context !", this);
        }

        return journalAssigned;
    }

    public void ToggleJournalUI()
    {
        // Journal is not present in the battle
        if (GameUIManager.Instance.CurrentMode == UIMode.BATTLE)
            return;

        if (CheckForJournal())
        {
            m_questJournal.ToggleJournalUI();
        }
    }

    public void HideJournalUI()
    {
        // Journal is not present in the battle
        if (GameUIManager.Instance.CurrentMode == UIMode.BATTLE)
            return;

        if (CheckForJournal())
        {
            m_questJournal.HideJournalUI();
        }
    }
}
