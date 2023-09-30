using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    internal class GitWindow : EditorWindow
    {
        static GitWindow _instance;
        public static GitWindow Instance
        {
            get
            {
                if (_instance == null && EditorWindow.HasOpenInstances<GitWindow>())
                {
                    _instance = (GitWindow)EditorWindow.GetWindow(typeof(GitWindow));
                }
                return _instance;
            }
        }

        static GUILayoutOption[] guiLayoutIcon = new GUILayoutOption[] {
            GUILayout.Width(25)
        };

        static GUILayoutOption[] guiLayoutStatsLeft = new GUILayoutOption[] {
            GUILayout.Width(100)
        };

        static Dictionary<string, Texture> gitIcons;

        //GUI states
        bool needRepaint = true;
        int delayedRepaintIn = 0;
        string oldGitState = "";
        static bool areHandlersInstalled = false;
        [SerializeField]
        bool unstagedFoldoutState = false;
        [SerializeField]
        Vector2 unstagedScrollState;

        //Styles
        static GUIStyle _rightAlignedStyle;
        private static GUIStyle RightAlignedStyle
        {
            get
            {
                if (_rightAlignedStyle == null)
                {
                    _rightAlignedStyle = new GUIStyle(EditorStyles.label);
                    _rightAlignedStyle.alignment = TextAnchor.MiddleRight;
                }
                return _rightAlignedStyle;
            }
        }




        public GitWindow()
        {
            this.minSize = new Vector2(400, 220);
        }

        private void OnDestroy()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectViewIconOverlays;
        }

        new internal static void Repaint()
        {
            if (Instance == null) return;
            Instance.needRepaint = true;
        }

        internal static void DelayedRepaint()
        {
            Instance.delayedRepaintIn = 7;
            Instance.UpdateTitle();
        }

        private void UpdateTitle()
        {
            titleContent = EditorGUIUtility.TrTextContentWithIcon("Git (" + GitControl.gitStateWord + ")", "Version Control System utilizing Git", "Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_" + GitControl.gitState + ".png");
            Repaint();
        }

        private void OnGUI()
        {
            UpdateTitle();

            if (gitIcons == null)
            {
                gitIcons = new Dictionary<string, Texture>();
                gitIcons.Add("MRG", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Conflict.png"));
                gitIcons.Add("M", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Modified.png"));
                gitIcons.Add("R", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Modified.png"));
                gitIcons.Add("D", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Trash.png"));
                gitIcons.Add("A", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Plus.png"));
                gitIcons.Add("C", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Plus.png"));
                gitIcons.Add("?", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Untracked.png"));
                gitIcons.Add("!", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Neutral.png"));

                //Editor icons
                gitIcons.Add("_QB_Stage", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_QB_Stage.png"));
                gitIcons.Add("_QB_Unstage", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_QB_Unstage.png"));
                gitIcons.Add("_QB_Revert", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_QB_Revert.png"));
                gitIcons.Add("_QB_Open", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_QB_Open.png"));
            }

            if (!DrawGeneralStatus()) DrawGitGUI();
        }

        void OnInspectorUpdate()
        {
            if (delayedRepaintIn > 0)
            {
                if (--delayedRepaintIn <= 0)
                {
                    needRepaint = true;
                }
            }

            if (oldGitState != GitControl.gitState || needRepaint)
            {
                needRepaint = false;
                oldGitState = GitControl.gitState;

                //should not be required anymore: delayedRepaintIn = EditorPrefs.GetInt("Firesplash.Git.AutoRepaintInterval", 3) * 10;
                UpdateTitle();
                base.Repaint();
            }

            if (!areHandlersInstalled)
            {
                EditorApplication.projectWindowItemOnGUI += ProjectViewIconOverlays;
                areHandlersInstalled = true;
            }
        }

        /// <summary>
        /// Draws the general status of the git integration
        /// </summary>
        /// <returns>true if a "breaking" status view is drawn - used to hide the normal controls</returns>
        bool DrawGeneralStatus()
        {
            EditorWindow openFlowDialog = (EditorWindow.HasOpenInstances<DialogCommit>() ? EditorWindow.GetWindow(typeof(DialogCommit)) : null);
            if (openFlowDialog == null) openFlowDialog = (EditorWindow.HasOpenInstances<DialogAddRemote>() ? EditorWindow.GetWindow(typeof(DialogAddRemote)) : null);

            if (GitControl.isProcessRunning)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Integration Status: Git is executing \"" + GitControl.Instance.gitProc.runningCommand.ToString() + "\"");
                if (GUILayout.Button("Kill Process"))
                {
                    EngineDispatcher.Enqueue(new GitDialog()
                    {
                        text = "Killing a running git process can have unpredictable results. Are you sure you want to do this?",
                        positiveButton = "Yes. I know what I am doing.",
                        negativeButton = "Damn, NO!",
                        onPositive = () =>
                        {
                            GitControl.KillRunningProcess();
                        }
                    });
                }
                EditorGUILayout.EndHorizontal();
                return false;
            }
            else if (openFlowDialog != null)
            {
                EditorGUILayout.LabelField("A Dialog is currently open and awaiting input.");
                if (GUILayout.Button("Bring the dialog to front"))
                {
                    openFlowDialog.Show();
                }
                EditorGUILayout.Space();
#if UNITY_EDITOR_WIN
                if (GUILayout.Button("Open Git Bash"))
#else
                if (GUILayout.Button("Open Terminal"))
#endif
                {
                    GitControl.OpenGitBash();
                }
                return true;
            }
            else if (!GitControl.isGitToolsFolderExisting)
            {
                EditorGUILayout.HelpBox(new GUIContent("Git executable not found. Please point me to its install directory."));
                EditorGUILayout.TextField("Git installation path", EditorPrefs.GetString("Firesplash.Git.GitPath"));
                if (GUILayout.Button("Update Path"))
                {
                    GitSettings.OpenGitPicker();
                };
                return true;
            }
            else if (!GitControl.isGitFolderExisting)
            {
                EditorGUILayout.HelpBox(new GUIContent("This project is not an initialized Git repository yet. This tool can run 'git init' for you but you should consider if you'd like to initialize and/or clone a repository manually.\n\nNote: This tool is running inside a project so it is not possible to clone a repo using Git Collaboration Tools."));
                if (GUILayout.Button("Initialize"))
                {
                    //Safety check
                    if (GitControl.isProcessRunning) return true;

                    GitControl.GitInit((bool success, string txt) =>
                    {
                        GitControl.UpdateGitStatus();
                        Repaint();
                    });
                };
                return true;
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Integration Status: Git is idle, overall status is " + GitControl.gitState);
                EditorGUILayout.EndHorizontal();
                return false;
            }
        }



        internal static int DrawChangeList(bool showUnstaged, bool showStaged)
        {
            int hiddenChangesCount = 0;

            using (EditorGUILayout.HorizontalScope fileEntry = new EditorGUILayout.HorizontalScope())
            {
                if (showUnstaged) EditorGUILayout.LabelField(new GUIContent("WC", "Working Copy (unstaged)"), guiLayoutIcon);
                if (showStaged) EditorGUILayout.LabelField(new GUIContent("IDX", "Index (staging)"), guiLayoutIcon);
                EditorGUILayout.LabelField("File", guiLayoutIcon);
            }

            if (GitControl.gitChangeSet == null)
            {
                EditorGUILayout.LabelField("No changeset data available.");
                return 0;
            }

            GitControl.ChangeSetEntry[] fileList = GitControl.gitChangeSet.Values.ToArray();

            GUILayoutOption[] quickButtonLayout = new GUILayoutOption[] { GUILayout.Width(24), GUILayout.Height(20) };

            foreach (GitControl.ChangeSetEntry change in fileList)
            {
                if (change.wcChange == "" && change.idxChange == "") continue;

                bool shallFileBeShown = false;

                using (EditorGUILayout.HorizontalScope fileEntry = new EditorGUILayout.HorizontalScope())
                {
                    try
                    {
                        if (showUnstaged)
                        {
                            if (gitIcons.ContainsKey(change.changeCode.Substring(1, 1)))
                            {
                                EditorGUILayout.LabelField(new GUIContent(gitIcons[change.changeCode.Substring(1, 1)], change.wcChange), guiLayoutIcon);
                                shallFileBeShown = true;
                            }
                            else if (showStaged) EditorGUILayout.LabelField(change.changeCode.Substring(1, 1), guiLayoutIcon);
                        }
                        else if (change.changeCode.Substring(1, 1) != " ") hiddenChangesCount++;

                        if (showStaged)
                        {
                            if (gitIcons.ContainsKey(change.changeCode.Substring(0, 1)))
                            {
                                EditorGUILayout.LabelField(new GUIContent(gitIcons[change.changeCode.Substring(0, 1)], change.idxChange), guiLayoutIcon);
                                shallFileBeShown = true;
                            }
                            else if (showUnstaged) EditorGUILayout.LabelField(change.changeCode.Substring(0, 1), guiLayoutIcon);
                        }
                        else if (change.changeCode.Substring(0, 1) != " ") hiddenChangesCount++;

                        if (shallFileBeShown)
                        {
                            if (change.wcChange == "" && change.idxChange == "")
                            {
                                if (gitIcons.ContainsKey("MRG")) EditorGUILayout.LabelField(new GUIContent(gitIcons["MRG"], "Merge Conflict"), guiLayoutIcon);
                                EditorGUILayout.LabelField(change.otherChange);
                            }

                            if (EditorPrefs.GetBool("Firesplash.Git.ResolveAssets", true) && change.fileName.Length > 5 && change.fileName.Substring(0, 6).Equals("Assets"))
                            {
                                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(change.fileName, typeof(UnityEngine.Object));
                                if (asset != null)
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(asset, asset.GetType(), false);
                                    EditorGUI.EndDisabledGroup();
                                } 
                                else EditorGUILayout.LabelField(change.fileName);
                            } 
                            else EditorGUILayout.LabelField(change.fileName);

                            EditorGUI.BeginDisabledGroup(!gitIcons.ContainsKey(change.changeCode.Substring(0, 1))); //has staged changes?
                            if (GUILayout.Button(new GUIContent("", gitIcons["_QB_Unstage"], "Unstage this file"), quickButtonLayout))
                            {
                                string pathSpec = change.fileName;
                                if (File.Exists(change.fileName + ".meta")) pathSpec = pathSpec + "\" \"" + pathSpec + ".meta";

                                GitControl.GitUnstage("\"" + pathSpec + "\"", (success, text) => {
                                    if (success)
                                    {
                                        GitControl.UpdateGitStatus((s, t) => { Repaint(); });
                                    }
                                });
                            }
                            EditorGUI.EndDisabledGroup();

                            EditorGUI.BeginDisabledGroup(!gitIcons.ContainsKey(change.changeCode.Substring(1, 1))); //has unstaged changes?
                            if (GUILayout.Button(new GUIContent("", gitIcons["_QB_Stage"], "Stage this file"), quickButtonLayout))
                            {
                                string pathSpec = change.fileName;
                                if (File.Exists(change.fileName + ".meta")) pathSpec = pathSpec + "\" \"" + pathSpec + ".meta";

                                GitControl.GitStage("\"" + pathSpec + "\"", (success, text) => { 
                                    if (success)
                                    {
                                        GitControl.UpdateGitStatus((s, t) => { Repaint(); });
                                    }
                                });
                            }
                            if (GUILayout.Button(new GUIContent("", gitIcons["_QB_Revert"], "Revert unstaged changes to this file"), quickButtonLayout))
                            {
                                GitControl.FileRevertUnstaged(change.fileName);
                            }
                            EditorGUI.EndDisabledGroup();

                            if (GUILayout.Button(new GUIContent("", gitIcons["_QB_Open"], "Open Folder in Explorer/Finder"), quickButtonLayout))
                            {
                                GitControl._OpenWithSystemViewer(change.fileName, !Event.current.shift);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        EditorGUI.EndDisabledGroup();
                        UnityEngine.Debug.LogWarning("Git Collaboration Error: Could not parse line of changeset: <b>" + change + "</b>\nException: " + e.ToString() + "\nPlease report this to assets@firesplash.de");
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }
            return hiddenChangesCount;
        }



        //This is called when everything is working and will draw the usual Git Tools GUI
        void DrawGitGUI()
        {
            EditorGUI.BeginDisabledGroup(GitControl.isProcessRunning && GitControl.Instance.gitProc.runningCommand != GitProcess.GitCommand.UPDATE && GitControl.Instance.gitProc.runningCommand != GitProcess.GitCommand.STATUS);
            //Magic buttons
            if (GUILayout.Button(new GUIContent("Magic All-In-One Solution", "This function will stage all changes, do a commit (asking for confirmation), pull remote changes and push the updated repository to the remote repository automatically with the ease of a single button-click. The process will abort if there are merge conflicts."), EditorStyles.miniButton))
            {
                GitControl.GitMagicAiOTool((bool result, string text) =>
                {
                    EngineDispatcher.Enqueue(() =>
                    {
                        if (!result)
                        {
                            UnityEngine.Debug.LogWarning("Git Collaboration: AiO-Tool was not able to automatically sync all changes");
                            GitControl.UpdateGitStatus(null);
                        }
                        else
                        {
                            UnityEngine.Debug.Log("Git Collaboration: AiO-Tool has successfully run through all stages. Your Working tree is clean and all changes are synced with your remote.");
                            GitControl.UpdateGitStatus(null);
                        }
                        Repaint();
                    });
                });
            }

            //Staging buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Stage all changes", EditorStyles.miniButtonLeft, new GUILayoutOption[] { GUILayout.Width(position.width / 3 - 2) }))
            {
                GitControl.GitStage(null);
                Repaint();
            }

            if (GUILayout.Button("Revert all changes", EditorStyles.miniButtonMid, new GUILayoutOption[] { GUILayout.Width(position.width / 3) }))
            {
                if (EditorUtility.DisplayDialog("Revert project", "Are you sure, you want to REVERT ALL CHANGES made to the ENTIRE PROJECT since your last commit?\n\nThis can not be undone!", "Yes, revert all changes", "Cancel"))
                {
                    GitControl.GitUnstage(".", (bool result, string text) => {
                        GitControl.GitRevertUnstaged(".", null);
                    });
                    Repaint();
                }
            }

            if (GUILayout.Button("Commit staged changes", EditorStyles.miniButtonRight, new GUILayoutOption[] { GUILayout.Width(position.width / 3 - 2) }))
            {
                GitControl.GitCommit(null);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            //Transfer Buttons
            EditorGUI.BeginDisabledGroup(!GitControl.isTracking);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Pull changes from server", EditorStyles.miniButtonLeft, new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 2) }))
            {
                GitControl.GitPull(null);
                Repaint();
            }
            if (GUILayout.Button("Push commits to server", EditorStyles.miniButtonRight, new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 2) }))
            {
                GitControl.GitPush(null);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();



            EditorGUILayout.BeginHorizontal();
#if UNITY_EDITOR_WIN
            if (GUILayout.Button("Open Git Bash", EditorStyles.miniButtonLeft, new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 2) }))
#else
            if (GUILayout.Button("Open Terminal", EditorStyles.miniButtonLeft, new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 2) }))
#endif
            {
                GitControl.OpenGitBash();
            }

            if (GUILayout.Button("Refresh Status", EditorStyles.miniButtonRight, new GUILayoutOption[] { GUILayout.Width(position.width / 2 - 2) }))
            {
                GitControl.UpdateGitStatus(true, (bool success, string txt) =>
                {
                    Repaint();
                });
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("On Branch", guiLayoutStatsLeft);
            EditorGUILayout.LabelField(GitControl.gitBranch);
            EditorGUILayout.EndHorizontal();

            if (!GitControl.gitRepoInitialized)
            {
                EditorGUILayout.HelpBox("This repository has not been initialized yet.\nTo enable all features and be able to add a remote repository, you have to make a first commit.\nIf you intended to download a repository, you need to run git clone manually into a new folder because it is not possible to clone into an existing project folder.", MessageType.Warning);
            }
            else
            {
                if (GitControl.gitTracking == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("No Remote:", guiLayoutStatsLeft);
                    if (GUILayout.Button("Connect a remote repository"))
                    {
                        ScriptableObject.CreateInstance<DialogAddRemote>().Fire(this);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tracking Status", guiLayoutStatsLeft);
                    EditorGUILayout.LabelField("This repository is not tracking a remote");
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tracking Remote", guiLayoutStatsLeft);
                    EditorGUILayout.LabelField(GitControl.gitTracking);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tracking Status", guiLayoutStatsLeft);
                    EditorGUILayout.LabelField(GitControl.gitTrackStatusReadable);
                    EditorGUILayout.EndHorizontal();
                }
            }

            //Unstaged changes list
            GUILayout.VerticalScope fileListScope = new GUILayout.VerticalScope();
            if (GitControl.gitStatusData != null && GitControl.gitStatusData.Length > 3 && GitControl.gitChangeSet != null)
            {
                GUILayout.Space(10);

                unstagedFoldoutState = EditorGUILayout.Foldout(unstagedFoldoutState, (GitControl.gitChangeSet.Count > 0 ? (GitControl.gitChangeSet.Count).ToString() : "No") + " unstaged (WC) or uncommitted (IDX) changes" + (unstagedFoldoutState ? ":" : ""));
                if (unstagedFoldoutState && GitControl.gitChangeSet.Count > EditorPrefs.GetInt("Firesplash.Git.MaxDisplayedChangeSetRows", 250))
                {
                    EditorGUILayout.HelpBox("Not showing ChangeSet containing " + GitControl.gitChangeSet.Count + " lines as it exceeds the configured maximum of " + EditorPrefs.GetInt("Firesplash.Git.MaxDisplayedChangeSetRows", 250) + ".\nYou can review the changes in a Git Bash if you like. (Command: git status)", MessageType.Warning);
                }
                else if (unstagedFoldoutState)
                {
                    unstagedScrollState = EditorGUILayout.BeginScrollView(unstagedScrollState);
                    DrawChangeList(true, true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndScrollView();
                }
            }
            fileListScope.Dispose();
        }




        [MenuItem("Tools/FSE Git Collaboration/Open Git Panel")]
        [MenuItem("Window/Git")]
        public static void ShowWindow()
        {
            ((GitWindow)EditorWindow.GetWindow(typeof(GitWindow))).Show();
        }





#region ProjectView Overlays
        static void ProjectViewIconOverlays(string assetGuid, Rect r)
        {
            //Ignore repains and don't run in play mode
            if (Application.isPlaying || Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewIcons", true)) {
                string fileName = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (!(fileName.StartsWith("Assets") || fileName.StartsWith("Packages")))
                {
                    //This is not an asset
                    return;
                }

                if (EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewIgnoreIcons", false) && GitControl.ignoredFilesList != null && GitControl.ignoredFilesList.Contains(fileName))
                {
                    //This is an ignored file
                    //Draw icon
                    Rect iconRect = new Rect(r);
                    if (r.height < 40)
                    {
                        iconRect.height = 16;
                        iconRect.width = 16;
                        iconRect.x = r.x + (r.height / 5.3f);
                        iconRect.y = r.y + (r.height / 6.8f);
                    }
                    else
                    {
                        iconRect.height = r.height / 2;
                        iconRect.width = iconRect.height;
                        iconRect.x = r.x + (r.height / 3f);
                        iconRect.y = r.y + (r.height / 3.5f);
                    }
                    UnityEngine.GUI.DrawTexture(iconRect, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Overlay_Ignored" + (r.height > 45 ? "" : "_Mini") + ".png", typeof(Texture2D)));
                }
                else if (EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewFolderIcons", true) && GitControl.dirtyFolderSet != null && GitControl.dirtyFolderSet.Contains(fileName))
                {
                    //This is a dirty folder
                    //Draw icon
                    Rect iconRect = new Rect(r);
                    if (r.height < 40)
                    {
                        iconRect.height = 16;
                        iconRect.width = 16;
                        iconRect.x = r.x + (r.height / 5.3f);
                        iconRect.y = r.y + (r.height / 6.8f);
                    }
                    else
                    {
                        iconRect.height = r.height / 2;
                        iconRect.width = iconRect.height;
                        iconRect.x = r.x + (r.height / 3f);
                        iconRect.y = r.y + (r.height / 3.5f);
                    }
                    UnityEngine.GUI.DrawTexture(iconRect, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Overlay_Folder" + (r.height > 45 ? "" : "_Mini") + ".png", typeof(Texture2D)));
                }
                else if (GitControl.gitChangeSet != null && GitControl.gitChangeSet.ContainsKey(fileName))
                {
                    //This is the actual changed asset
                    //Draw icon
                    Rect iconRect = new Rect(r);
                    //if (iconRect.height > 64) iconRect.height = 64;
                    iconRect.width = iconRect.height;
                    if (r.height < 46) iconRect.x -= 14;
                    else
                    {
                        iconRect.x += iconRect.width * 0.1f - 10;
                        iconRect.y += iconRect.height * 0.04f - 5;
                    }
                    UnityEngine.GUI.DrawTexture(iconRect, (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Overlay_" + GitControl.gitChangeSet[fileName].fileColorState + (r.height > 45 ? "" : "_Mini") + ".png", typeof(Texture2D)));

                    //Don't show Detail view column if the view is too small, this is a big icon view or a sub asset
                    if (r.width < 200 || r.height > 20 || r.x > 16)
                    {
                        return;
                    }
                    else
                    {
                        // Right align label:
                        const int width = 120;
                        r.x += r.width - width;
                        r.width = width;
                        Color oldColor = UnityEngine.GUI.contentColor;
                        UnityEngine.GUI.contentColor = Color.red;
                        if (GitControl.gitChangeSet[fileName].remoteChanged) UnityEngine.GUI.Label(r, "REMOTE CHANGED", RightAlignedStyle);
                        UnityEngine.GUI.contentColor = oldColor;
                    }
                }
            }
        }
#endregion
    }
}
