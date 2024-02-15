using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoClipGeneral : MonoBehaviour
{

    [SerializeField] private VideoPlayer videoPlayer;

    
    public void OnVideoStart(string videoUrl)
    {

        gameObject.SetActive(true);
        videoPlayer.url = videoUrl;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;

    }

    private void OnVideoEnd(VideoPlayer video)
    {

        gameObject.SetActive(false);

    }
}
