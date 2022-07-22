using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoadingCircleController : MonoBehaviour
{
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    public void StartAnimation() {
        SetAnimating(true);
    }

    public void StopAnimation() {
        SetAnimating(false);
    }

    public void SetAnimating(bool animating) {
        GetComponent<Image>().enabled = animating;
        anim.enabled = animating;
    }

}
