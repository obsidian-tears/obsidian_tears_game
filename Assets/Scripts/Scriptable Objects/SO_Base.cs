using System.Collections;
using UnityEngine;

public class SO_Base : ScriptableObject
{
    protected void StartCoroutine(IEnumerator _task)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Can not run coroutine outside of play mode.");
            return;
        }

        CoWorker coworker = new GameObject("CoWorker_" + _task.ToString()).AddComponent<CoWorker>();
        coworker.Work(_task);
    }
}
