using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnKeyPress : MonoBehaviour
{
    /*Use this script when you want pressing a button to call a UnityEvent (which is different than an event).*/

    public KeyCode useKey;

    public UnityEvent onKeyPress;

    void Start()
    {
        if (onKeyPress == null)
            onKeyPress = new UnityEvent();
    }

    void Update()
    {
        if (Input.GetKeyDown(useKey))
        {
            onKeyPress.Invoke();
        }
    }
}