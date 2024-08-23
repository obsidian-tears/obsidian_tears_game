using PixelCrushers.QuestMachine;
using PixelCrushers;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class GatesData
{
    public bool isOpen = false;
}

public class GateOpener : Saver, IMessageHandler
{
    [SerializeField] private GameObject _openGate, _closedGate;

    private GatesData _gatesData;

    public override void Awake()
    {
        _gatesData = new GatesData();
    }

    private void Start()
    {
        MessageSystem.AddListener(this, "Quest State Changed", "");
    }

    // Unregister when this GameObject is destroyed.
    public override void OnDestroy()
    {
        MessageSystem.RemoveListener(this);
    }

    public override string RecordData()
    {
        return JsonConvert.SerializeObject(_gatesData);
    }

    public override void ApplyData(string s)
    {
        if (string.IsNullOrEmpty(s)) return;

        Debug.Log("gates" + s);
        _gatesData = JsonConvert.DeserializeObject<GatesData>(s);
        Debug.Log("gates result " + _gatesData.isOpen);

        if (_gatesData.isOpen)
            Open();
    }

    // Handle messages from Quest Machine.
    public void OnMessage(MessageArgs messageArgs)
    {
        OnMessage(messageArgs.message, messageArgs.parameter, messageArgs.sender);
    }

    private void OnMessage(string message, string parameter, object s)
    {
        if (QuestMachine.GetQuestNodeState("TanasArtifacts", "VillageKey2") == QuestNodeState.Active)
        {
            _gatesData.isOpen = true;
            Open();
        }
    }

    private void Open()
    {
        _closedGate.SetActive(false);
        _openGate.SetActive(true);
    }
}