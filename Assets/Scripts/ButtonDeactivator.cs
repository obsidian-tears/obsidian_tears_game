using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDeactivator : MonoBehaviour
{
    public void DisableButton()
    {
        var button = GetComponent<Button>();
        button.enabled = false;
    }
}

