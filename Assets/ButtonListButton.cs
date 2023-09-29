using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListButton : MonoBehaviour
{

    [SerializeField]
    private Text myText;

    public void SetText(string textString)
    {
     myText.text = textString;
    }

    public void onClick()
    {
        
    }
}
