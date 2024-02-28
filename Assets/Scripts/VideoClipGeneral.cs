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

    [SerializeField] private Animator fadeAnim;
    [SerializeField] private GameObject fadeCanvas;


    public void OnVideoStart(string videoUrl)
    {
        StartCoroutine(FadeIn(videoUrl));
    }

    private void OnVideoEnd(VideoPlayer video)
    {

        videoPlayerCanvas.gameObject.SetActive(false);
        videoClipGameObj.gameObject.SetActive(false);
        fadeCanvas.SetActive(false);
        foreach (GameObject gameObj in listGameObj) { gameObj.SetActive(true); }
        Time.timeScale = 1;
    }

    public void AddGameObject(GameObject gameObj)
    {

        listGameObj.Add(gameObj);

    }

    public IEnumerator FadeIn(string videoUrl)
    {

        fadeCanvas.SetActive(true);
        fadeAnim.SetTrigger("Fade");
        Time.timeScale = 0;
        yield return new WaitForSeconds(1f);
        videoPlayerCanvas.gameObject.SetActive(true);
        videoClipGameObj.gameObject.SetActive(true);
        

        videoPlayer.url = videoUrl;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }





}
