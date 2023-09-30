using UnityEngine;
using UnityEditor;
using System;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    class DialogGitInput : EditorWindow
    {
        string _enteredText, lText, lPositive, lNegative;
        Action<string> positiveAction;
        Action negativeAction;

        internal DialogGitInput(string title, string text, Action<string> onPositive, string positive, Action onNegative, string negative)
        {
            titleContent = new GUIContent("Git Collaboration: " + title);
            lPositive = positive;
            lNegative = negative;
            lText = text;
            positiveAction = onPositive;
            negativeAction = onNegative;

            this.minSize = new Vector2(600, 120);
            this.maxSize = new Vector2(600, 120);

            ShowModal();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(lText, MessageType.None);
            _enteredText = EditorGUILayout.TextField(_enteredText);

            GUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(lNegative))
            {
                if (negativeAction != null) negativeAction.Invoke();
                Close();
            }

            if (GUILayout.Button(lPositive))
            {
                if (positiveAction != null) positiveAction.Invoke(_enteredText);
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
