using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Autosave : MonoBehaviour
{
    public static Autosave Instance;

    public int saveIntervalSeconds = 600;
    public GameObject DialoguePanel;

    bool haveAsked;
    [HideInInspector]
    public bool isEnabled;
    Coroutine autosaveCoroutine;

    void Awake() => Instance = this;
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!haveAsked && scene.name != "GranGranFirst" && scene.name != "Battle")
            StartCoroutine(AskForAutosave());
    }

    void Start()
    {
        if (haveAsked) return;

        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "GranGranFirst" && sceneName != "Battle")
            StartCoroutine(AskForAutosave());
    }

    IEnumerator AskForAutosave()
    {
        if (!haveAsked)
        {
            yield return new WaitForSeconds(1);
            DialoguePanel.SetActive(true);
            haveAsked = true;
        }
    }

    public void EnableAutoSave(bool enabled)
    {
        if (enabled)
            autosaveCoroutine = StartCoroutine(AutosaveCoroutine());

        DialoguePanel.SetActive(false);
        isEnabled = enabled;
    }

    public void RestartAutosaveCoroutine()
    {
        if (!isEnabled)
            return;

        StopCoroutine(autosaveCoroutine);
        autosaveCoroutine = StartCoroutine(AutosaveCoroutine());
    }

    IEnumerator AutosaveCoroutine()
    {
        yield return new WaitForSeconds(saveIntervalSeconds);

        while (SceneManager.GetActiveScene().name == "Battle")
            yield return new WaitForSeconds(20);

        if (SaveLoadConfirmation.Instance.showSaveConfirmation)
            SaveLoadConfirmation.Instance.ShowSaveConfirmationPanel();
        else
            ReactController.Instance.SignalSaveGame();
    }
}