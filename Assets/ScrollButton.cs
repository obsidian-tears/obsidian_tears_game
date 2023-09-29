using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollButton : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI ButtonText;

    public void SetText(string textString)
    {

       ButtonText.text = textString;
    }

    public void OnClick()
    {
        
    }
}
