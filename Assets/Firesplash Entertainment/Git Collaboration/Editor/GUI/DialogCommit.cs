using UnityEngine;
using UnityEditor;
using System.IO;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    class DialogCommit : EditorWindow
    {
        GitWindow gw;
        GitControl.GitResultCallback resultCallback;
        string commitMessageText = "";

        internal void Fire(GitWindow gitWindow, GitControl.GitResultCallback callback)
        {
            gw = gitWindow;
            resultCallback = callback;

            if (File.Exists(Application.temporaryCachePath.Replace('/', '\\') + "\\GitCommitText.tmp"))
            {
                commitMessageText = File.ReadAllText(Application.temporaryCachePath.Replace('/', '\\') + "\\GitCommitText.tmp");
            }
            else if(File.Exists(Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "\\.gitmessage"))
            {
                commitMessageText = File.ReadAllText(Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "\\.gitmessage");
            }

            ShowUtility();
        }

        internal DialogCommit()
        {
            titleContent = new GUIContent("Git Collaboration: Commit");
            //position = new Rect(Screen.width, Screen.height, 505, 250);
            this.minSize = new Vector2(600, 500);
            this.maxSize = new Vector2(600, 500);
        }

        Vector2 fileListScrollState;
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Commit description:", EditorStyles.boldLabel);
            GUILayout.Space(2);
            commitMessageText = EditorGUILayout.TextArea(commitMessageText, new GUILayoutOption[] {
                GUILayout.Height(140),
                GUILayout.Width(593)
            });
            EditorGUILayout.HelpBox("Please enter a valuable short description for this commit. It should state, what you have worked on and probably information about its state. Refer to your internal policies for further information.", MessageType.Info);
            GUILayout.Space(5);
            GUILayout.VerticalScope fileListScope = new GUILayout.VerticalScope();
            if (GitControl.gitStatusData != null && GitControl.gitStatusData.Length > 3)
            {
                GUILayout.Space(3);

                EditorGUILayout.LabelField((GitControl.gitChangeSet.Count > 0 ? (GitControl.gitChangeSet.Count).ToString() : "No") + " changes staged for commit" + (GitControl.gitChangeSet.Count > 0 ? ":" : "."), EditorStyles.boldLabel);
                if (GitControl.gitChangeSet.Count > EditorPrefs.GetInt("Firesplash.Git.MaxDisplayedChangeSetRows", 250))
                {
                    EditorGUILayout.HelpBox("Not showing ChangeSet containing " + GitControl.gitChangeSet.Count + " lines as it exceeds the configured maximum of " + EditorPrefs.GetInt("Firesplash.Git.MaxDisplayedChangeSetRows", 250) + ".\nYou can review the changes in a Git Bash if you like. (Command: git status)", MessageType.Warning);
                }
                else
                {
                    fileListScrollState = EditorGUILayout.BeginScrollView(fileListScrollState);
                    int hiddenChangesCount = GitWindow.DrawChangeList(false, true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndScrollView();
                    if (hiddenChangesCount > 0) EditorGUILayout.HelpBox("There are " + hiddenChangesCount + " unstaged changes in the working copy. These will not be committed or pushed.", MessageType.Warning, true);
                }
            }
            fileListScope.Dispose();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            if (GUILayout.Button("Commit"))
            {
                if (GitControl.isProcessRunning) return; //check for safety reasons

                if (commitMessageText.Length == 0) commitMessageText = "No commit message given.";

                File.WriteAllText(Application.temporaryCachePath + "\\GitCommitText.tmp", commitMessageText);

                GitControl.GitCommitConfirmed(resultCallback);

                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
