using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SceneManager", menuName = "Game/SceneManager")]
public class SceneManagerObject : SO_Base
{
    public GameObject transition;
    public float transitionWait = 1.0f;

    private Stack<int> loadedLevels;

    [System.NonSerialized]
    private bool initialized;

    private void Init()
    {
        loadedLevels = new Stack<int>();
        initialized = true;
    }

    public UnityEngine.SceneManagement.Scene GetActiveScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
    }

    public void LoadScene(int buildIndex)
    {
        if (!initialized) Init();

        StartCoroutine(LoadSceneEnum(buildIndex));
    }

    public IEnumerator LoadSceneEnum(int buildIndex)
    {
        Time.timeScale = 0;
        if (transition)
        {
            Instantiate(transition, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSecondsRealtime(transitionWait);
        Time.timeScale = 1;

        loadedLevels.Push(GetActiveScene().buildIndex);
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex);
        while (!asyncOperation.isDone) yield return null;
    }

    public void LoadScene(string sceneName)
    {
        if (!initialized) Init();

        StartCoroutine(LoadSceneEnum(sceneName));
    }

    public IEnumerator LoadSceneEnum(string sceneName)
    {
        Time.timeScale = 0;
        if (transition)
        {
            Instantiate(transition, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSecondsRealtime(transitionWait);
        Time.timeScale = 1;

        loadedLevels.Push(GetActiveScene().buildIndex);
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone) yield return null;
    }

    public void LoadPreviousScene()
    {
        if (!initialized)
        {
            Debug.LogError("You haven't used the LoadScene functions of the scriptable object. Use them instead of the LoadScene functions of Unity's SceneManager.");
        }
        if (loadedLevels.Count > 0)
        {
            StartCoroutine(LoadSceneEnum(loadedLevels.Pop()));
            //UnityEngine.SceneManagement.SceneManager.LoadScene(loadedLevels.Pop());
        }
        else
        {
            Debug.LogError("No previous scene loaded");
        }
    }

    
}