using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCamera : MonoBehaviour
{
    private Canvas canvas;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if(canvas != null && camera != null)
        {
            canvas.worldCamera = camera;
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
