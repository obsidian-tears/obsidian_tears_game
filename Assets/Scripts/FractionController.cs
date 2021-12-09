using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FractionController : MonoBehaviour
{
    [SerializeField] FloatValue currentValue;
    [SerializeField] FloatValue maxValue;
    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        if (currentValue && maxValue && text)
            text.text = $"{currentValue.value} / {maxValue.value}";
    }

    public void UpdateValue()
    {
        text.text = $"{currentValue.value} / {maxValue.value}";
    }
}
