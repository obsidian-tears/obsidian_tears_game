using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.QuestMachine;
using PixelCrushers;

public class RealQuestStateListener : MonoBehaviour, IMessageHandler
{
    [Tooltip("The ID of the quest to listen for.")]
    [SerializeField] private string questId;
    [Tooltip("The ID of the quest node to optionally listen for.")]
    [SerializeField] private string nodeId;
    [Tooltip("What to do when the quest succeeds.")]
    [SerializeField] private UnityEvent onQuestSuccess;
    [Tooltip("What to do when the quest node becomes true, if specified.")]
    [SerializeField] private UnityEvent onQuestNodeTrue;
    private bool alreadyTriggeredQuest = false;
    private bool alreadyTriggeredNode = false;


    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", questId);
    }

    public void OnMessage(MessageArgs messageArgs)
    {
        if (!alreadyTriggeredQuest && QuestMachine.GetQuestState(questId) == QuestState.Successful) {
            onQuestSuccess.Invoke();
            alreadyTriggeredQuest = true;
        }
        if (nodeId != "" && QuestMachine.GetQuestNodeState(questId, nodeId) 
                == QuestNodeState.True)
        {
            onQuestNodeTrue.Invoke();
            alreadyTriggeredNode = true;
        }
        if (alreadyTriggeredQuest && alreadyTriggeredNode) {
            MessageSystem.RemoveListener(this);
        }
    }
}
