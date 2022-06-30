using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    private const float DISAPPEAR_TIMER_MAX = 0.35f;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText(damageAmount.ToString());
        
        if (isCriticalHit)
        {
            textMesh.fontSize = 300;
            textColor = Color.red;
        }
        else
        {
            textMesh.fontSize = 250;
            textColor = new Color(255, 163, 0, 255);
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;
    }

    private void Update()
    {
        float moveYSpeed = 20f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        if(disappearTimer > DISAPPEAR_TIMER_MAX * 0.5f)
        {
            //First half of popup
            float increaseScaleAmount = 0.15f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            //Second half of popup
            float decreaseScaleAmount = 0.15f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if(disappearTimer < 0)
        {
            //Start disappearing
            float disappearSpeed = 500f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
