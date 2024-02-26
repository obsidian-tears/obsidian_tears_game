using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoClipGeneral : MonoBehaviour
{

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoPlayerCanvas;
    [SerializeField] private GameObject videoClipGameObj;
    private List<GameObject> listGameObj = new List<GameObject>();

    public void OnVideoStart(string videoUrl)
    {

        videoPlayerCanvas.gameObject.SetActive(true);
        videoClipGameObj.gameObject.SetActive(true);

        videoPlayer.url = videoUrl;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;

    }

    private void OnVideoEnd(VideoPlayer video)
    {

        videoPlayerCanvas.gameObject.SetActive(false);
        videoClipGameObj.gameObject.SetActive(false);

        foreach (GameObject gameObj in listGameObj) { gameObj.SetActive(true); }

    }

    public void AddGameObject(GameObject gameObj)
    {

        listGameObj.Add(gameObj);

    }
}
