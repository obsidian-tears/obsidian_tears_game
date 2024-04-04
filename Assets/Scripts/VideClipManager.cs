using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideClipManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private ScenePortal scenePortal;

    void OnEnable()
    {
        string videoURL = PlayerPrefs.GetString("urlParaReproducir");
        Debug.Log(videoURL);
        videoPlayer.url = videoURL;

        if (!string.IsNullOrEmpty(videoURL))
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogWarning("No se encontró una URL de video para reproducir.");
        }
    }


    void Update()
    {
        /*if (!videoPlayer.isPlaying && hasStartedPlaying)
        {
            PlayerPrefs.DeleteKey("urlAReproducir");            

        }*/
    }

    private void OnVideoEnd(VideoPlayer video)
    {
        PlayerPrefs.DeleteKey("urlParaReproducir");
        
        //SceneManager.LoadScene("overworld");
        scenePortal.UsePortal();
    }




}
