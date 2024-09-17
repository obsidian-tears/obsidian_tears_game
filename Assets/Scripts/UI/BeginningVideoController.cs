using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class BeginningVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start() => videoPlayer.loopPointReached += (VideoPlayer video) => SceneManager.LoadScene("GranGranFirst");
}
