using UnityEngine;

public class SaveLoadConfirmation : MonoBehaviour
{
    public bool showSaveConfirmation = true;
    public bool showLoadConfirmation = true;
    public GameObject saveConfirmationPanel, loadConfirmationPanel;

    public static SaveLoadConfirmation Instance;
    void Awake() => Instance = this;

    public void ShowSaveConfirmationPanel()
    {
        if (showSaveConfirmation)
            saveConfirmationPanel.gameObject.SetActive(true);
        else
            ReactController.Instance.SignalSaveGame();
    }

    public void ShowLoadConfirmationPanel()
    {
        if (showLoadConfirmation)
            loadConfirmationPanel.gameObject.SetActive(true);
        else
            ReactController.Instance.SignalLoadGame();
    }

    public void Save() => ReactController.Instance.SignalSaveGame();
    public void Load() => ReactController.Instance.SignalLoadGame();

    public void DiscardSave()
    {
        if (Autosave.Instance && Autosave.Instance.isEnabled)
            Autosave.Instance.RestartAutosaveCoroutine();
    }
}