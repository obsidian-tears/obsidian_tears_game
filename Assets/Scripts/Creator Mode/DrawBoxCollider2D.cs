using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DrawBoxCollider2D : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = false;
    }

    private void Update()
    {
        DrawBox();
    }

    private void DrawBox()
    {
        Vector2 bottomLeft = new Vector2(boxCollider.offset.x - boxCollider.size.x / 2f, boxCollider.offset.y - boxCollider.size.y / 2f);
        Vector2 topLeft = new Vector2(boxCollider.offset.x - boxCollider.size.x / 2f, boxCollider.offset.y + boxCollider.size.y / 2f);
        Vector2 topRight = new Vector2(boxCollider.offset.x + boxCollider.size.x / 2f, boxCollider.offset.y + boxCollider.size.y / 2f);
        Vector2 bottomRight = new Vector2(boxCollider.offset.x + boxCollider.size.x / 2f, boxCollider.offset.y - boxCollider.size.y / 2f);

        lineRenderer.positionCount = 5;
        lineRenderer.SetPositions(new Vector3[]
        {
            bottomLeft, topLeft, topRight, bottomRight, bottomLeft
        });
    }
}
