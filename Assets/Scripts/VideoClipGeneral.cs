using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoClipGeneral : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoRawImage;
    [SerializeField] private Animator screenFader;

    private List<GameObject> listGameObj = new List<GameObject>();
    private Player playercomponent;

    public void AddGameObject(GameObject gameObj) => listGameObj.Add(gameObj);

    public void StartVideo(string videoUrl)
    {
        playercomponent = GameObject.FindWithTag("Player").GetComponent<Player>();
        playercomponent.enabled = false;

        foreach (var gameObj in listGameObj) { gameObj.SetActive(true); }

        StartCoroutine(FadeIn(videoUrl));
    }

    public IEnumerator FadeIn(string videoUrl)
    {
        screenFader.transform.root.gameObject.SetActive(true);
        screenFader.SetTrigger("FadeIn");
        yield return new WaitForSeconds(screenFader.GetCurrentAnimatorStateInfo(0).length);
        videoRawImage.SetActive(true);
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.url = videoUrl;
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer video)
    {
        videoRawImage.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        screenFader.SetTrigger("FadeOut");
        yield return new WaitForSeconds(screenFader.GetCurrentAnimatorStateInfo(0).length);
        screenFader.transform.root.gameObject.SetActive(false);
        playercomponent.enabled = true;
    }
}