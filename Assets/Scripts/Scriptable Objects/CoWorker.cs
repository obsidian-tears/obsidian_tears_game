using System.Collections;
using UnityEngine;

public class CoWorker : MonoBehaviour
{
    public void Work(IEnumerator _coroutine)
    {
        StartCoroutine(WorkCoroutine(_coroutine));
    }

    private IEnumerator WorkCoroutine(IEnumerator _coroutine)
    {
        yield return StartCoroutine(_coroutine);
        Destroy(this.gameObject);
    }
}
