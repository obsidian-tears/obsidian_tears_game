using UnityEngine;
using UnityEditor;
using System;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    class DialogAddRemote : EditorWindow
    {
        GitWindow gw;
        string _urlText = "";

        internal void Fire(GitWindow gitWindow)
        {
            gw = gitWindow;

            ShowUtility();
        }

        internal DialogAddRemote()
        {
            titleContent = new GUIContent("Git Collaboration: Connect a remote repository");
            this.minSize = new Vector2(600, 220);
            this.maxSize = new Vector2(600, 220);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Please enter the remote repository URL:");
            _urlText = EditorGUILayout.TextField(_urlText);
            GUILayout.Space(2);

            EditorGUILayout.LabelField("How do you want to sync the remote repository?");
            EditorGUILayout.HelpBox("Adding a remote to an existing repository can be a tricky thing.\nIf the remote repo is empty or does not know a " + GitControl.gitBranch + " branch, you can simply push your project (we will only push the current branch).\nIf you want to sync to an existing remote, you should clone the reopsitory instead if possible (this can't be done with our tool). If not possible, you can try to use the sync approach but it will likely produce merge conflicts which you will have to solve manually using Git Bash.", MessageType.Info);
            GUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!(_urlText.Contains("@") || Uri.IsWellFormedUriString(_urlText, UriKind.Absolute)));
            if (GUILayout.Button("Push my repository 'as is'"))
            {
                if (GitControl.isProcessRunning) return;
                GitControl.GitRemoteAddPush(_urlText);
                Close();
            }

            if (GUILayout.Button("Full Sync (Pull -> Merge -> Push)"))
            {
                if (GitControl.isProcessRunning) return;
                GitControl.GitRemoteAddPull(_urlText);
                Close();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
