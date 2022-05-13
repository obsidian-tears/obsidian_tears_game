// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Custom inspector for IconQuestContent assets.
    /// </summary>
    [CustomEditor(typeof(IconQuestContent), true)]
    public class IconQuestUIContentEditor : QuestSubassetEditor
    {

        protected override void Draw()
        {
            if (serializedObject == null) return;
            var imageProperty = serializedObject.FindProperty("m_image");
            var captionProperty = serializedObject.FindProperty("m_caption");
            var countProperty = serializedObject.FindProperty("m_count");
            UnityEngine.Assertions.Assert.IsNotNull(imageProperty, "Quest Machine: Internal error - m_image is null.");
            UnityEngine.Assertions.Assert.IsNotNull(captionProperty, "Quest Machine: Internal error - m_caption is null.");
            UnityEngine.Assertions.Assert.IsNotNull(countProperty, "Quest Machine: Internal error - m_count is null.");
            if (imageProperty == null || captionProperty == null || countProperty == null) return;
            EditorGUILayout.PropertyField(imageProperty, true);
            EditorGUILayout.PropertyField(captionProperty, true);
            EditorGUILayout.PropertyField(countProperty, true);
        }

    }
}
