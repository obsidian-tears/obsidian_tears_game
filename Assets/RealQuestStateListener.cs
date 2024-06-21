using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.QuestMachine;
using PixelCrushers;

public class RealQuestStateListener : MonoBehaviour, IMessageHandler
{
    [SerializeField] private string questId;
    [SerializeField] private UnityEvent onQuestSuccess;

    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", questId);
    }   

 public void OnMessage(MessageArgs messageArgs)
    {
        OnMessage(messageArgs.message, messageArgs.parameter, messageArgs.sender);
    }
    
    private void OnMessage(string message, string parameter, object s)
    {
        if (QuestMachine.GetQuestState(questId) == QuestState.Successful)   {
            onQuestSuccess.Invoke();
        }
    }
}
