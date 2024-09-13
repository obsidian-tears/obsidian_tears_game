using UnityEngine;
using UnityEditor;

public class RemoveObjectsByKeyword : EditorWindow
{
    private string filterKeywords = "tree,grass,bush,rock";
    private float radius = 5f;
    private const float radiusChangeAmount = 0.5f;
    private const float minRadius = 0.5f;
    private const float maxRadius = 20f;
    private bool isRemoving = false;

    [MenuItem("Tools/Temo_Koki/Remove Objects by Keywords")]
    public static void ShowWindow() => GetWindow<RemoveObjectsByKeyword>("Remove Objects by Keywords");

    private void OnGUI()
    {
        GUILayout.Label("Remove Objects by Name in Radius", EditorStyles.boldLabel);

        filterKeywords = EditorGUILayout.TextField("Filter Keywords:", filterKeywords);
        radius = EditorGUILayout.FloatField("Removal Radius:", radius);

        GUILayout.Space(10);
        GUILayout.Label("Hold Shift to activate tool", EditorStyles.boldLabel);
        GUILayout.Label("Shift+ / Shift- to adjust radius", EditorStyles.boldLabel);
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    private void OnSceneGUI(SceneView sceneView)
    {
        var e = Event.current;

        if (e.control || e.command || e.alt)
        {
            isRemoving = false;
            return;
        }

        if (e.type == EventType.KeyDown)
        {
            if (e.shift)
            {
                if (e.keyCode == KeyCode.Equals || e.keyCode == KeyCode.KeypadPlus)
                    radius = Mathf.Clamp(radius + radiusChangeAmount, minRadius, maxRadius);
                else if (e.keyCode == KeyCode.Minus || e.keyCode == KeyCode.KeypadMinus)
                    radius = Mathf.Clamp(radius - radiusChangeAmount, minRadius, maxRadius);
                else isRemoving = true;

                e.Use();
            }
        }
        else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftShift)
            isRemoving = false;

        if (isRemoving)
        {
            var worldPos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            worldPos.z = 0;

            Handles.color = Color.red;
            Handles.DrawWireDisc(worldPos, Vector3.forward, radius);

            var hitColliders = Physics2D.OverlapCircleAll(worldPos, radius);
            var keywords = filterKeywords.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var collider in hitColliders)
            {
                var colliderName = collider.name.ToLower();
                foreach (var keyword in keywords)
                {
                    if (colliderName.Contains(keyword))
                    {
                        Undo.DestroyObjectImmediate(collider.gameObject);
                        break;
                    }
                }
            }

            sceneView.Repaint();
        }
    }
}