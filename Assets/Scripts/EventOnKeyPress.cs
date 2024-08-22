using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnKeyPress : MonoBehaviour
{
    /*Use this script when you want pressing a button to call a UnityEvent (which is different than an event).*/

    public string useButton;

    public UnityEvent onButtonPress;

    void Start()
    {
        if (onButtonPress == null)
            onButtonPress = new UnityEvent();
    }

    void Update()
    {
        if (Input.GetButtonDown(useButton))
        {
            //Don't open main menu during battle scene as it pauses game without any UI appearing and looks like a bug
            if (FindObjectOfType<BattleSystem>() != null)
                return;

            onButtonPress.Invoke();
        }
    }
}