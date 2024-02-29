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
    private GameObject player;
    private Player playercomponent;

   public void OnVideoStart(string videoUrl)
    {
        
        player = GameObject.FindWithTag("Player");
        playercomponent = player.GetComponent<Player>();        
        playercomponent.enabled = false;
        foreach (GameObject gameObj in listGameObj) { gameObj.SetActive(true); }
        StartCoroutine(FadeIn(videoUrl));
    }

    private void OnVideoEnd(VideoPlayer video)
    {
        playercomponent.enabled = true;

        videoPlayerCanvas.gameObject.SetActive(false);
        videoClipGameObj.gameObject.SetActive(false);
        fadeCanvas.SetActive(false);
        
        //Time.timeScale = 1;
    }

    public void AddGameObject(GameObject gameObj)
    {

        listGameObj.Add(gameObj);

    }

    public IEnumerator FadeIn(string videoUrl)
    {

        fadeCanvas.SetActive(true);
        fadeAnim.SetTrigger("FadeIn");
       // Time.timeScale = 0;
        videoPlayerCanvas.gameObject.SetActive(true);
        videoClipGameObj.gameObject.SetActive(true);        
        yield return new WaitForSeconds(fadeAnim.GetCurrentAnimatorStateInfo(0).length);
        fadeAnim.SetTrigger("FadeOut");
        videoPlayer.url = videoUrl;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }




}
