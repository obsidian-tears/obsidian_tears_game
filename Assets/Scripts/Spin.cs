using UnityEngine;

public class Spin : MonoBehaviour
{
    private RectTransform rectComponent;
    private float rotateSpeed = 400f;

    private void Start()
    {
        rectComponent = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectComponent.Rotate(0f, 0f, rotateSpeed * Time.unscaledDeltaTime);
    }
}