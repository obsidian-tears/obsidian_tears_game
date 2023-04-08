using System.Collections;
using System.Collections.Generic;
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

    public void StartNewGameReact()
    {
        ReactController.Instance.SignalNewGame();
    }

    public void SaveGameReact()
    {
        ReactController.Instance.SignalSaveGame();
    }

    public void LoadGameReact()
    {
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
        if (CheckForJournal())
        {
            m_questJournal.ToggleJournalUI();
        }
    }

    public void HideJournalUI()
    {
        if (CheckForJournal())
        {
            m_questJournal.HideJournalUI();
        }
    }
}
