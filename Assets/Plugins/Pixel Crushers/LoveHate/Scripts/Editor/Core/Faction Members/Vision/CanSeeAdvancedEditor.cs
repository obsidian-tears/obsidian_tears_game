// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// Custom editor for CanSeeAdvanced that shows debug info in the Scene view.
    /// </summary>
    [CustomEditor(typeof(CanSeeAdvanced), true)]
    public class CanSeeAdvancedEditor : Editor
    {

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
#else
        [DrawGizmo(GizmoType.SelectedOrChild)]
#endif
        public static void RenderCustomGizmo(CanSeeAdvanced canSeeAdvanced, GizmoType gizmoType)
        {
            DrawFOVs(canSeeAdvanced);
            DrawLastPointChecked(canSeeAdvanced);
        }

        private static void DrawFOVs(CanSeeAdvanced canSeeAdvanced)
        {
            if (canSeeAdvanced == null || canSeeAdvanced.fieldsOfView == null) return;
            for (int i = 0; i < canSeeAdvanced.fieldsOfView.Length; i++)
            {
                DrawFOV(canSeeAdvanced, canSeeAdvanced.fieldsOfView[i]);
            }
        }

        private static void DrawFOV(CanSeeAdvanced canSeeAdvanced, FieldOfView fov)
        {
            if (canSeeAdvanced == null || fov == null) return;
            Handles.color = fov.gizmoColor;
            if (canSeeAdvanced.dimension == Dimension.Is2D)
            {
                Handles.DrawSolidArc(canSeeAdvanced.transform.position,
                                     canSeeAdvanced.transform.forward,
                                     Quaternion.AngleAxis(-0.5f * fov.verticalFOV, Vector3.forward) * canSeeAdvanced.transform.right,
                                     fov.verticalFOV,
                                     fov.maxDistance);
            }
            else
            {
                Handles.DrawSolidArc(canSeeAdvanced.transform.position,
                                     canSeeAdvanced.transform.up,
                                     Quaternion.AngleAxis(-0.5f * fov.horizontalFOV, Vector3.up) * canSeeAdvanced.transform.forward,
                                     fov.horizontalFOV,
                                     fov.maxDistance);
            }
        }

        private static void DrawLastPointChecked(CanSeeAdvanced canSeeAdvanced)
        {
            if (!Application.isPlaying || canSeeAdvanced.raySource == null) return;
            var hit = !string.IsNullOrEmpty(canSeeAdvanced.lastPointGizmoName);
            Gizmos.color = hit ? Color.green : Color.red;
            Gizmos.DrawLine(canSeeAdvanced.raySource.position, canSeeAdvanced.lastPointChecked);
            Gizmos.color = Color.white;
            if (hit) Gizmos.DrawIcon(canSeeAdvanced.lastPointChecked, canSeeAdvanced.lastPointGizmoName, true);
        }   
    }

}
