using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JournalTextSpacing : MonoBehaviour
{
    public TextMeshProUGUI text;

    public RectTransform rt;

    private void Start()
    {
        /*Canvas canvas = gameObject.GetComponent<Canvas>();
        rt = canvas.GetComponent(typeof(RectTransform)) as RectTransform;
        rt = gameObject.GetComponent<RectTransform>();*/
    }

    private void OnEnable()
    {
        if(text.text.Length < 30)
        {
            rt.sizeDelta = new Vector2(298.4269f, 24f);
        }
        else
        {
            Debug.Log("setting text to be longer");
            rt.sizeDelta = new Vector2(298.4269f, 48f);
        }
    }
}
