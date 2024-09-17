using PixelCrushers;
using UnityEngine;
using UnityEngine.Video;

public class EndingVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private ScenePortal scenePortal;

    void Start() => videoPlayer.loopPointReached += (VideoPlayer video) => scenePortal.UsePortal();
}
