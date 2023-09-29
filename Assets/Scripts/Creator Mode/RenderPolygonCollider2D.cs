using UnityEngine;

public class DebugPolygonCollider2D : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        DrawLines();
    }

    public void DrawLines()
    {
        if (polygonCollider == null)
        {
            polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
        }

        Vector2[] points = polygonCollider.points;
        int pointCount = points.Length;

        lineRenderer.positionCount = pointCount;
        lineRenderer.loop = true;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(points[i]);
            lineRenderer.SetPosition(i, worldPoint);
        }

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }
}
