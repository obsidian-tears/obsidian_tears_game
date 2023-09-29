using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    private const float MAX_ZOOM = 0.5f;
    private const float MIN_ZOOM = 150f;

    public float moveSpeed = 500f;
    public float scrollSpeed = 1.5f;
    public Camera cam;
    public Transform cam_location;

    Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Zoom Camera In/Out based on mouse scroll-wheel
        cam.orthographicSize += -Input.mouseScrollDelta.y * scrollSpeed;
        // Cap Zoom to stay between MIN_ZOOM and MAX_ZOOM
        cam.orthographicSize = Mathf.Max(cam.orthographicSize, MAX_ZOOM);
        cam.orthographicSize = Mathf.Min(cam.orthographicSize, MIN_ZOOM);

        // Get Left/Right keys
        movement.x = Input.GetAxisRaw("Horizontal");
        // Get Up/Down keys
        movement.y = Input.GetAxisRaw("Vertical");
        // Update Camera Coords (Faster when zoomed out more)
        cam_location.position +=
            new Vector3(movement.x, movement.y, 0)
            * moveSpeed
            * Time.deltaTime
            * (cam.orthographicSize / MIN_ZOOM);
    }
}
