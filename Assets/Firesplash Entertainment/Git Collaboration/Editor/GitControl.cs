using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;
using Firesplash.GameDevAssets.GitCollab.GUI;

namespace Firesplash.GameDevAssets.GitCollab
{
    [InitializeOnLoad]
    public class GitControl
    {
        //Singleton
        static GitControl _instance;
        internal static GitControl Instance
        {
            get
            {
                if (_instance == null) { _instance = new GitControl(); }
                return _instance;
            }
        }

        //Typdefinitionen
        public delegate void GitResultCallback(bool wasSuccessful, string additionalMessage);

        public class ChangeSetEntry
        {
            public string fileName { get; internal set; } = "";
            public string changeCode { get; internal set; } = "";
            public string otherChange { get; internal set; } = "";
            public string wcChange { get; internal set; } = "";
            public string idxChange { get; internal set; } = "";
            public string fileColorState { get; internal set; } = "";
            public bool remoteChanged { get; internal set; } = false;

            internal void FlagRemoteChanged()
            {
                remoteChanged = true;
                fileColorState = "Red";
                if (otherChange == "") otherChange = "Changed remotely";
            }
        }


        [SerializeField]
        private static bool firstRun = true;
        internal static bool isGitFolderExisting = false;
        internal static bool isGitToolsFolderExisting = false;
        public static bool isProcessRunning
        {
            get
            {
                return Instance.gitProc != null && !Instance.gitProc.HasExited;
            }
        }


        [SerializeField]
        internal static string gitExePath, gitBasePath;
        Process gitBash;

        float lastStatusCheck = -900;
        float lastOnlineCheck = -9000;
        bool automatismStoppedBecauseOfErrors = false;


        #region public fields
        //Git Status
        string _gitState { get; set; } = "Neutral";
        string _gitStateWord { get; set; } = "Synced";
        public static string gitState
        {
            get
            {
                return Instance._gitState;
            }
        }
        public static string gitStateWord
        {
            get
            {
                return Instance._gitStateWord;
            }
        }

        string _gitBranch { get; set; } = "unknown";
        public static string gitBranch
        {
            get
            {
                return Instance._gitBranch;
            }
        }

        bool _gitRepoInitialized = false;
        public static bool gitRepoInitialized
        {
            get
            {
                return Instance._gitRepoInitialized;
            }
        }

        string _gitTracking = "unknown", gitTrackStatusRaw = "unknown";
        public static string gitTracking
        {
            get
            {
                return Instance._gitTracking;
            }
        }

        string _gitTrackStatusReadable;
        public static string gitTrackStatusReadable
        {
            get
            {
                return Instance._gitTrackStatusReadable;
            }
        }

        [SerializeField]
        Dictionary<string, ChangeSetEntry> _gitChangeSet;
        public static Dictionary<string, ChangeSetEntry> gitChangeSet
        {
            get
            {
                return Instance._gitChangeSet;
            }
        }
        

        [SerializeField]
        List<string> _dirtyFolderSet;
        public static List<string> dirtyFolderSet
        {
            get
            {
                return Instance._dirtyFolderSet;
            }
        }


        [SerializeField]
        List<string> _ignoredFilesList;
        public static List<string> ignoredFilesList
        {
            get
            {
                return Instance._ignoredFilesList;
            }
        }


        string _gitStatusData;
        public static string gitStatusData
        {
            get
            {
                return Instance._gitStatusData;
            }
        }

        bool _isTracking = false;
        public static bool isTracking
        {
            get
            {
                return Instance._isTracking;
            }
        }

        bool _isTrackingAhead = false;
        public static bool isTrackingAhead
        {
            get
            {
                return Instance._isTrackingAhead;
            }
        }

        bool _isTrackingBehind = false;
        public static bool isTrackingBehind
        {
            get
            {
                return Instance._isTrackingBehind;
            }
        }
        #endregion


        internal GitProcess gitProc;



        //static vars for usage in processes
        [SerializeField]
        internal static bool cfgAskLockingSupport = true;
        [SerializeField]
        internal static bool cfgMoreDebugging = false;
        [SerializeField]
        internal static bool cfgIgnoreGlobalConfig = false;

        internal static void ReadSettings()
        {
            //Process prefs
            cfgAskLockingSupport = UnityEditor.EditorPrefs.GetBool("Firesplash.Git.RequestLockingSupport", true);
            cfgMoreDebugging = UnityEditor.EditorPrefs.GetBool("Firesplash.Git.MoreDebugging", false);
            cfgIgnoreGlobalConfig = EditorPrefs.GetBool("Firesplash.Git.IgnoreGlobalConfig", false);
        }

        GitControl()
        {
            ReadSettings();

            isGitFolderExisting = Directory.Exists(".git");
            _gitChangeSet = new Dictionary<string, ChangeSetEntry>();
            _dirtyFolderSet = new List<string>();
            EditorApplication.wantsToQuit += PendingQuit;
            EditorApplication.update += EditorUpdate;
            EditorApplication.projectChanged += () =>
            {
                if (EditorPrefs.GetBool("Firesplash.Git.EnableAdaptiveChecks", true) && lastStatusCheck > Time.realtimeSinceStartup - (EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30)/ 4f))
                {
                    lastStatusCheck = Time.realtimeSinceStartup - (EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30) / 1.25f);
                }
            };


            if (firstRun)
            {
                _UpdateGitStatus(false, (bool wasSuccessful, string result) =>
                {
                    if (isTrackingBehind)
                    {
                        EngineDispatcher.Enqueue(new GitDialog()
                        {
                            title = "Your local working copy is not up to date",
                            text = "The project changed at the tracked remote. It is recommended to pull the latest changes before starting your work.\n\nDo you want to run the AiO Tool?",
                            positiveButton = "Sync all changes (AiO Tool)",
                            negativeButton = "No, let me do this manually",
                            onPositive = new Action(() =>
                            {
                                GitMagicAiOTool(null);
                            })
                        });
                    }
                });
                firstRun = false;
            }
        }

        ~GitControl()
        {
            EditorApplication.wantsToQuit -= PendingQuit;
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            //Retry until we find it
            if (!isGitToolsFolderExisting)
            {
                isGitToolsFolderExisting = (EditorPrefs.GetString("Firesplash.Git.GitPath") != null && EditorPrefs.GetString("Firesplash.Git.GitPath").Length > 0 && Directory.Exists(EditorPrefs.GetString("Firesplash.Git.GitPath")));
            }

            if (lastStatusCheck < 0) lastStatusCheck = Time.realtimeSinceStartup; //This´is done to prevent double update during startup

            if (lastStatusCheck < (Time.realtimeSinceStartup - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30)))
            {
                lastStatusCheck = Time.realtimeSinceStartup; //prevent multiple calls
                GitControl.UpdateGitStatus();
            }
        }


        static void MarkStatusDirty(int secondsToWait = 3)
        {
            Instance.lastStatusCheck = Time.realtimeSinceStartup + secondsToWait - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update after reverting
        }



        public static void OpenGitBash()
        {
            Instance._OpenGitBash();
        }

        /// <summary>
        /// Opens the given path in system's default application
        /// </summary>
        /// <param name="gitPath">The path to the file or folder, relative to the project ROOT - not assets folder!</param>
        /// <param name="reveal">If true, we will reveal the file instead of opening it</param>
        public static void _OpenWithSystemViewer(string gitPath, bool reveal)
        {
            string path = gitPath;

            path = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + path;

            if (reveal)
            {
#if UNITY_EDITOR_WIN
                Process.Start("explorer.exe", "/select,\"" + path.Replace("/", "\\") + "\"");
#elif UNITY_EDITOR_OSX
                Process.Start("open", "-R \"" + path.Replace("\\", "/") + "\"");
#else 
                EditorUtility.DisplayDialog("Not supported", "Unfortunately opening the basepath is not supported on this operating system.", "Too bad...");
#endif
            }
            else Application.OpenURL(path);
        }

        static bool CheckErrorForCommonIssues(string text)
        {
            if (text.Contains("gpg: signing failed: No secret key"))
            {
                EngineDispatcher.Enqueue(new GitDialog()
                {
                    title = "Signing Error",
                    text = "Your git configuration does not configure a GPG key. You can not use commit signing without configuring your system first.\nThis config must be done in Git Bash (or on the console for linux systems).",
                    positiveButton = "Open Documentation",
                    onPositive=() => { Application.OpenURL("https://git-scm.com/book/en/v2/Git-Tools-Signing-Your-Work"); },
                    negativeButton = "Ignore"
                });
                return true;
            }
            else if (text.Contains("invalid length character"))
            {
                EngineDispatcher.Enqueue(new GitDialog()
                {
                    title = "Authentication Error",
                    text = "Could not execute the last request to the remote repository.\n\nMost likely you have configured your git to use an agent-provided SSH-Key which is not available right now. Check your ssh key agent.",
                    positiveButton = "OK"
                });
                return true;
            }
            else if (text.Contains("Authentication denied") || text.Contains("Authentication failed"))
            {
                EngineDispatcher.Enqueue(new GitDialog()
                {
                    title = "Authentication DENIED",
                    text = "The configured remote repository denied your request because the given credentials were not authorized.\n\nIf you configured User/Password using this utility, maybe the credentials have become invalid or the remote repository does not support this authentication type anymore.\n\nRemote says: " + text.Replace("remote: ", "") + "\n\nThe current task has been cancelled.",
                    positiveButton = "OK",
                    onPositive = null,
                    negativeButton = "Configure Credentials",
                    onNegative = () => { EditorWindow.GetWindow<DialogCredentials>().ShowModal(); }
                });
                return true;
            }
            else if (text.Contains("could not read Username"))
            {
                EngineDispatcher.Enqueue(new GitDialog()
                {
                    title = "Authentication required",
                    text = "The configured remote repository requires authentication but none of the following criteria is met:\n- You can configure the credentials for your remote repository in your global git settings (stored credentials)\n- Git can be configured to authenticate using an agent-provided SSH-Key\n- You can set up User/Password in the Git Settings of your project\n\nIf your 'origin' repository supports authenticating using user and password, you can configure the credentials now. Else please close this dialog and configure your git installation accordingly so that it does not require interactive authentication (see our documentation PDF).\n\nThe current task has been cancelled.",
                    positiveButton = "OK",
                    onPositive = null,
                    negativeButton = "Configure Credentials",
                    onNegative = () => { EditorWindow.GetWindow<DialogCredentials>().ShowModal(); }
                });
                return true;
            }
            return false;
        }

        void _OpenGitBash()
        {
            EngineDispatcher.Enqueue(() =>
            {
                ProbeGitPath();
                gitBash = new Process();
                gitBash.StartInfo.WorkingDirectory = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
                gitBash.StartInfo.CreateNoWindow = false;
                gitBash.StartInfo.UseShellExecute = false;
                gitBash.StartInfo.LoadUserProfile = true;
#if UNITY_EDITOR_WIN
                gitBash.StartInfo.FileName = gitBasePath + "\\..\\git-bash.exe";
                
#elif UNITY_EDITOR_OSX
                EngineDispatcher.Enqueue(new GitDialog() { title = "Terminal on MacOS", text = "On MacOS this only opens a new terminal window. You will have to switch to the project folder manually using the following command: cd " + Application.dataPath, positiveButton = "OK" });
                gitBash.StartInfo.FileName = "open";
                gitBash.StartInfo.Arguments = "-a Terminal -n";
#else
                gitBash.StartInfo.FileName = "/bin/bash";
#endif
                gitBash.Start();
            });
        }

        static bool ProbeGitPath()
        {
            string probableExePath;
            string[] searchPaths =
            {
                EditorPrefs.GetString("Firesplash.Git.GitPath"),
#if UNITY_EDITOR_WIN
                Environment.GetEnvironmentVariable("ProgramFiles") + "\\Git\\bin",
                Environment.GetEnvironmentVariable("ProgramFiles(x86)") + "\\Git\\bin",
                Application.dataPath.Replace("/", "\\").Remove(Application.dataPath.Length - 7) + "\\PortableGit",
#else
                "/opt/homebrew/bin", //Mac homebrew
                "/bin",
                "/usr/bin",
                Application.dataPath.Remove(Application.dataPath.Length - 7) + "/PortableGit",
#endif
            };

            if (EditorPrefs.GetString("Firesplash.Git.GitPath") != null && EditorPrefs.GetString("Firesplash.Git.GitPath").Length > 0 && Directory.Exists(EditorPrefs.GetString("Firesplash.Git.GitPath")))
            {
#if UNITY_EDITOR_WIN
                probableExePath = EditorPrefs.GetString("Firesplash.Git.GitPath") + "\\git.exe";
#else
                probableExePath = EditorPrefs.GetString("Firesplash.Git.GitPath") + "/git";
#endif
                if (File.Exists(probableExePath))
                {
                    gitExePath = probableExePath;
                    gitBasePath = EditorPrefs.GetString("Firesplash.Git.GitPath");
                    return true; //We already have our git
                }
            }

            foreach (string searchPath in searchPaths)
            {
                if (searchPath == null) continue;
#if UNITY_EDITOR_WIN
                probableExePath = searchPath + "\\git.exe";
#else
                probableExePath = searchPath + "/git";
#endif
                if (Directory.Exists(searchPath) && File.Exists(probableExePath))
                {
                    EditorPrefs.SetString("Firesplash.Git.GitPath", searchPath);
                    gitExePath = probableExePath;
                    gitBasePath = searchPath;
                    return true;
                }
            }

            return false;
        }



        bool PendingQuit()
        {
            if (!_gitState.Equals("Green") && !_gitState.Equals("Neutral"))
            {
                return EditorUtility.DisplayDialog("Git Collaborative Tools", "The git status of your project is not green.\nDo you want to cancel quitting and resolve this?", "Go Ahead and Quit", "Cancel Quitting");
            }
            return true;
        }


        /// <summary>
        /// Kills the currently running git process
        /// </summary>
        public static void KillRunningProcess()
        {
            Instance._KillRunningProcess();
        }
        void _KillRunningProcess()
        {
            if (gitProc != null)
            {
                gitProc.Kill();
                //isProcessRunning = false;
            }
        }

        #region Assets Menu Commands

        static List<string> GetSelectedFiles(bool encapsulate = false)
        {
            List<string> filelist = new List<string>();

            foreach (UnityEngine.Object o in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(o);
                if (path == null) continue;
                filelist.Add(encapsulate ? "\"" + path + "\"" : path);
                if (File.Exists(path + ".meta")) filelist.Add(encapsulate ? "\"" + path + ".meta" + "\"" : path + ".meta");
            }
            
            if (filelist.Count < 1)
            {
                EditorUtility.DisplayDialog("No object selected", "You can only work with this context menu on the right side of the project view.", "OK");
            }
            
            return filelist;
        }

        static string GetSelectedFiles(string glue, bool encapsulate = false)
        {
            return string.Join(glue, GetSelectedFiles(encapsulate));
        }

        [MenuItem("Assets/Git/Ignore", true)]
        [MenuItem("Assets/Git/Stage changes", true)]
        [MenuItem("Assets/Git/Revert unstaged changes", true)]
        [MenuItem("Assets/Git/Unstage", true)]
        static bool ValidateFileSelection()
        {
            if (Selection.assetGUIDs.Length > 0 && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject))) return true;
            return false;
        }

        [MenuItem("Assets/Git/Ignore")]
        static void FileIgnore()
        {
            List<string> filesToIgnore = GetSelectedFiles();
            if (filesToIgnore.Count < 1) return;

            foreach (string file in filesToIgnore.ToArray())
            {
                if (ignoredFilesList.Contains(file)) filesToIgnore.Remove(file);
                MarkStatusDirty(6);
            }

            if (filesToIgnore.Count == 0)
            {
                EditorUtility.DisplayDialog("Nothing to ignore", "All selected files are already being ignored.", "Oh, cool!");
                return;
            }

            if (EditorUtility.DisplayDialog("Ignore asset", "Are you sure, you want to ignore the following files and/or folders from now on?\n\n" + string.Join("\n", filesToIgnore) + "\n\nThis will cause the files not to be committed anymore, but it will not delete already committed files.\nTo Un-Ignore, you must manually edit the .gitignores file", "Yes, ignore", "Cancel"))
            {
                string ignoreFilePath = Path.GetFullPath(Application.dataPath.Replace('/', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".gitignore");
                File.AppendAllLines(ignoreFilePath, filesToIgnore);
            }
        }

        [MenuItem("Assets/Git/Stage changes")]
        static void FileStage()
        {
            GitStage(GetSelectedFiles(" ", true), null);
        }

        [MenuItem("Assets/Git/Revert unstaged changes")]
        static void FileRevertUnstaged()
        {
            FileRevertUnstaged(GetSelectedFiles(" ", true));
        }

        internal static void FileRevertUnstaged(string gitPath)
        {
            if (File.Exists(gitPath + ".meta")) gitPath = gitPath + "\" \"" + gitPath + ".meta";
            if (EditorUtility.DisplayDialog("Revert files", "Are you sure, you want to revert all unstaged changes made to the following files?\n\n" + gitPath.Replace("\" \"", "\n") + "\n\nThis can not be undone!", "Yes, revert", "Cancel"))
            {
                GitRevertUnstaged("\"" + gitPath + "\"", null);
            }
        }

        [MenuItem("Assets/Git/Unstage")]
        static void FileUnstage()
        {
            GitUnstage(GetSelectedFiles(" ", true), null);
        }
#endregion

#region Git Commands
        public static void GitInit(GitControl.GitResultCallback whenDone)
        {
            Instance._GitInit(whenDone);
        }

        void _GitInit(GitControl.GitResultCallback whenDone)
        {
            //isProcessRunning = true;
            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.INIT, (bool result, string text) =>
            {
                //isProcessRunning = false;
                if (result == false)
                {
                    UnityEngine.Debug.LogError("Git Collaboration: An unknown error occured while initializing the git repository:\n" + text.Replace('\0', '\n'));

                    EngineDispatcher.Enqueue(new GitDialog()
                    {
                        text = "An unknown error occured while initializing the repository.\nInvestigate the Console. Probably you will find additional information there.",
                        positiveButton = "Investigate in Git Bash",
                        negativeButton = "OK",
                        onPositive = OpenGitBash
                    });
                }
                else
                {
                    isGitFolderExisting = Directory.Exists(".git");
                }
            });
        }
        

        public static void GitMagicAiOTool(GitControl.GitResultCallback whenDone)
        {
            Instance._GitMagicAiOTool(whenDone);
        }

        void _GitMagicAiOTool(GitControl.GitResultCallback whenDone)
        {
            //isProcessRunning = true;
            _UpdateGitStatus(true, (bool wasUpdateSuccessful, string updMsg) =>
            {
                if (!wasUpdateSuccessful)
                {
                    Debug.LogError("Git Collaboration: Could not query repository changes. Cancelling AiO batch.");
                    return;
                }

                //Stage all changes (does not hurt if there are none and saves us a refresh)
                _GitStage(".", (bool wasStagingSuccessful, string stgMsg) =>
                    {
                        if (!wasStagingSuccessful)
                        {
                            Debug.LogError("Git Collaboration: Could not stage changes. Cancelling AiO batch.");
                            return;
                        }

                        _UpdateGitStatus(true, (bool wasUpdateSuccessful, string updMsg) =>
                        {
                            if (_gitChangeSet != null && _gitChangeSet.Where((c) => { return c.Value.idxChange != ""; }).Count() > 0)
                            {
                                GitCommit((bool wasCommitSuccessful, string cmtMsg) =>
                                {
                                    if (!wasCommitSuccessful)
                                    {
                                        Debug.LogError("Git Collaboration: Could not commit changes. Cancelling AiO batch.");
                                        return;
                                    }
                                    _GitAutoSync(whenDone);
                                });
                            }
                            else
                            {
                                //Our Working Tree is clean so we will instantly sync changes
                                UnityEngine.Debug.Log("Git Collaboration: Working tree clean, skipping commit workflow. Syncing repositories...");
                                _GitAutoSync(whenDone);
                            }
                        });
                    });

            });
        }

        public static void GitAutoSync(GitControl.GitResultCallback whenDone)
        {
            Instance._GitAutoSync(whenDone);
        }

        void _GitAutoSync(GitControl.GitResultCallback whenDone)
        {
            //isProcessRunning = true;
            if (isTrackingBehind)
            {
                GitPull((bool wasSuccessful, string msg) =>
                {
                    if (!wasSuccessful)
                    {
                        Debug.LogError("Git Collaboration: Could not merge fetched changes. Cancelling AiO batch.");
                        return;
                    }
                    GitPush(whenDone);
                });
            }
            else
            {
                GitPush(whenDone);
            }
        }
        
        /// <summary>
        /// Reverts **unstaged** changes in the project using a pathspec. Reverts to staged state.
        /// </summary>
        /// <param name="pathspec">The platform specific pastspec to append to "git checkout --"</param>
        /// <param name="whenDone">callback to execute after process finished</param>
        
        public static void GitRevertUnstaged(string pathspec, GitResultCallback whenDone)
        {
            Instance._GitRevertUnstaged(pathspec, whenDone);
        }

        void _GitRevertUnstaged(string pathspec, GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                Instance.gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.REVERT_UNSTAGED, (bool result, string text) =>
                {
                    if (whenDone == null) UpdateGitStatus(null);
                    if (result == false)
                    {
                        UnityEngine.Debug.LogError("An error occured while reverting changes in your working copy:\n" + text.Replace('\0', '\n'));

                        EngineDispatcher.Enqueue(new GitDialog()
                        {
                            text = "An error occured while staging the changes:\n" + text,
                            positiveButton = "Investigate in Git Bash",
                            negativeButton = "OK",
                            onPositive = OpenGitBash
                        });
                    }
                    MarkStatusDirty(); //we need a status update after reverting
                    AssetDatabase.Refresh();
                    if (whenDone != null) whenDone.Invoke(result, text);
                }, pathspec);
            });
        }
        
        /// <summary>
        /// Unstaged files in the project using a pathspec.
        /// </summary>
        /// <param name="pathspec">The platform specific pastspec to append to "git restore --staged"</param>
        /// <param name="whenDone">callback to execute after process finished</param>
        
        public static void GitUnstage(string pathspec, GitResultCallback whenDone)
        {
            Instance._GitUnstage(pathspec, whenDone);
        }

        void _GitUnstage(string pathspec, GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                Instance.gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.UNSTAGE, (bool result, string text) =>
                {
                    if (whenDone == null) UpdateGitStatus(null);
                    if (result == false)
                    {
                        UnityEngine.Debug.LogError("An error occured while reverting changes in your working copy:\n" + text.Replace('\0', '\n'));

                        EngineDispatcher.Enqueue(new GitDialog()
                        {
                            text = "An error occured while staging the changes:\n" + text,
                            positiveButton = "Investigate in Git Bash",
                            negativeButton = "OK",
                            onPositive = OpenGitBash
                        });
                    }
                    lastStatusCheck = Time.realtimeSinceStartup + 3 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update after reverting
                    AssetDatabase.Refresh();
                    if (whenDone != null) whenDone.Invoke(result, text);
                }, pathspec);
            });
        }



        /// <summary>
        /// Stages all changes in the project
        /// </summary>
        /// <param name="whenDone">callback to execute after process finished</param>
        public static void GitStage(GitResultCallback whenDone)
        {
            Instance._GitStage(".", whenDone);
        }

        /// <summary>
        /// Stages changes in the project using a pathspec.
        /// </summary>
        /// <param name="pathspec">The platform specific pastspec to append to "git add"</param>
        /// <param name="whenDone">callback to execute after process finished</param>
        
        public static void GitStage(string pathspec, GitResultCallback whenDone)
        {
            Instance._GitStage(pathspec, whenDone);
        }

        void _GitStage(string pathspec, GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                //isProcessRunning = true;
                Instance.gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.ADD, (bool result, string text) =>
                {
                    //isProcessRunning = false;
                    if (whenDone == null) UpdateGitStatus(null);
                    if (result == false)
                    {
                        UnityEngine.Debug.LogError("An error occured while staging changes in your working copy:\n" + text.Replace('\0', '\n'));

                        EngineDispatcher.Enqueue(new GitDialog()
                        {
                            text = "An error occured while staging the changes:\n" + text,
                            positiveButton = "Investigate in Git Bash",
                            negativeButton = "OK",
                            onPositive = OpenGitBash
                        });
                    }
                    lastStatusCheck = Time.realtimeSinceStartup + 3 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update after staging
                    if (whenDone != null) whenDone.Invoke(result, text);
                }, pathspec);
            });
        }


        public static void GitCommit(GitControl.GitResultCallback whenDone)
        {
            Instance._GitCommit(whenDone);
        }
        void _GitCommit(GitControl.GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                string commitMsgFileName = Application.temporaryCachePath.Replace('/', '\\') + "\\GitCommitText.tmp";

                ScriptableObject.CreateInstance<DialogCommit>().Fire(GitWindow.Instance, (bool wasCommitSuccessful, string text) =>
                {
                    //isProcessRunning = false;
                    if (!wasCommitSuccessful)
                    {
                        if (!CheckErrorForCommonIssues(text))
                        {
                            UnityEngine.Debug.LogError("An error occured while commiting changes in your index:\n" + text.Replace('\0', '\n'));
                        }
                        if (text.Contains("Changes not staged for commit"))
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "Could not commit: Your repository contains unstaged changes.",
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                        else if (text.Contains("nothing to commit, working tree clean"))
                        {
                            if (whenDone == null) //This is not an error but an information so in scripted flow we will skip the alert
                            {
                                EngineDispatcher.Enqueue(new GitDialog()
                                {
                                    text = "No need to commit. All changes are already committed.",
                                    positiveButton = "Investigate in Git Bash",
                                    negativeButton = "OK",
                                    onPositive = OpenGitBash
                                });
                            }
                        }
                        else
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while commiting the staged changes:\n" + text,
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            File.Delete(commitMsgFileName);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Git Collaboration: Unable to delete commit message file. This is not too bad, just have an eye on the commit message next time you commit! " + e.ToString());
                        }

                        UpdateGitStatus(true, (bool success, string message) => {
                            whenDone?.Invoke(wasCommitSuccessful, text);
                            GitWindow.Repaint();
                        });
                    }
                });
            });
        }

        public static void GitCommitConfirmed(GitControl.GitResultCallback resultCallback)
        {
            Instance._GitCommitConfirmed(resultCallback);
        }

        void _GitCommitConfirmed(GitControl.GitResultCallback resultCallback)
        {
            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.COMMIT, resultCallback, Application.temporaryCachePath + "\\GitCommitText.tmp");
        }


        public static void GitPull(GitControl.GitResultCallback whenDone)
        {
            Instance._GitPull(whenDone);
        }

        void _GitPull(GitControl.GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                //isProcessRunning = true;
                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.PULL, (bool mergeResult, string text) =>
                {
                    //isProcessRunning = false;
                    if (whenDone == null) UpdateGitStatus(null);
                    if (!mergeResult)
                    {
                        if (CheckErrorForCommonIssues(text))
                        {
                            //this has already been handled
                        }
                        else if (text.StartsWith("CONFLICT"))
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pulling changes from the server:\n" + text + "\n\nThe Working Copy has been rolled back to the state before pulling.\nYou will need to investigate and merge manually using git bash.",
                                positiveButton = "Abort and open Git Bash",
                                negativeButton = "Just abort",
                                onPositive = OpenGitBash
                            });
                            //isProcessRunning = true;
                            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.ABORT_MERGE, (bool abortResult, string abortText) =>
                            {
                                //isProcessRunning = false;
                                UpdateGitStatus(null);
                            });
                        }
                        else
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pulling changes from the server:\n" + text,
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                    }
                    AssetDatabase.Refresh();
                    if (whenDone != null) whenDone.Invoke(mergeResult, text);

                    lastStatusCheck = Time.realtimeSinceStartup + 2 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update
                });
            });
        }


        public static void GitSubmodulesUpdate(GitControl.GitResultCallback whenDone)
        {
            Instance._GitSubmodulesUpdate(whenDone);
        }

        void _GitSubmodulesUpdate(GitControl.GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                //isProcessRunning = true;
                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.SUBMOD_INIT, (bool success, string text) =>
                {
                    //isProcessRunning = false;
                    if (!success)
                    {
                        if (text.StartsWith("CONFLICT"))
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while updating submodules:\n" + text + "\n\nThe Working Copy has been rolled back to the state before pulling.\nYou will need to investigate and merge manually using git bash.",
                                positiveButton = "Abort and open Git Bash",
                                negativeButton = "Just abort",
                                onPositive = OpenGitBash
                            });
                            //isProcessRunning = true;
                            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.ABORT_MERGE, (bool abortResult, string abortText) =>
                            {
                                //isProcessRunning = false;
                                UpdateGitStatus(null);
                            });
                        }
                        else
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pulling submodule updates from the server(s):\n" + text,
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                    }
                    if (whenDone != null) whenDone.Invoke(success, text);

                    lastStatusCheck = Time.realtimeSinceStartup + 2 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update
                });
            });
        }


        public static void GitSubmoduleInit(GitControl.GitResultCallback whenDone)
        {
            Instance._GitSubmoduleInit(whenDone);
        }

        void _GitSubmoduleInit(GitControl.GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                //isProcessRunning = true;
                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.SUBMOD_INIT, (bool success, string text) =>
                {
                    //isProcessRunning = false;
                    if (!success)
                    {
                        if (text.StartsWith("CONFLICT"))
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pulling submodules from the server:\n" + text + "\n\nThe Working Copy has been rolled back to the state before pulling.\nYou will need to investigate and merge manually using git bash.",
                                positiveButton = "Abort and open Git Bash",
                                negativeButton = "Just abort",
                                onPositive = OpenGitBash
                            });
                            //isProcessRunning = true;
                            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.ABORT_MERGE, (bool abortResult, string abortText) =>
                            {
                                //isProcessRunning = false;
                                UpdateGitStatus(null);
                            });
                        }
                        else
                        {
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pulling submodules from the server:\n" + text,
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                    }
                    if (whenDone != null) whenDone.Invoke(success, text);

                    lastStatusCheck = Time.realtimeSinceStartup + 2 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update
                });
            });
        }


        public static void GitPush(GitControl.GitResultCallback whenDone)
        {
            Instance._GitPush(whenDone);
        }

        void _GitPush(GitControl.GitResultCallback whenDone)
        {
            EngineDispatcher.Enqueue(() =>
            {
                //isProcessRunning = true;
                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.PUSH, (bool result, string text) =>
                {
                    //isProcessRunning = false;
                    if (whenDone == null) UpdateGitStatus(null);
                    if (!result)
                    {
                        if (CheckErrorForCommonIssues(text))
                        {
                            //this has already been handled
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("An error occured while pushing our local changes to the server:\n" + text.Replace('\0', '\n'));

                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while pushing our local changes to the server:\n" + text,
                                positiveButton = "Investigate in Git Bash",
                                negativeButton = "OK",
                                onPositive = OpenGitBash
                            });
                        }
                    }
                    if (whenDone != null) whenDone.Invoke(result, text);

                    lastStatusCheck = Time.realtimeSinceStartup + 2 - EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30); //we need a status update
                });
            });
        }

        internal static void GitRemoteAddPush(string remoteUri)
        {
            Instance._GitRemoteAddAndSync(true, remoteUri);
        }

        internal static void GitRemoteAddPull(string remoteUri)
        {
            Instance._GitRemoteAddAndSync(false, remoteUri);
        }

        internal void _GitRemoteAddAndSync(bool syncUsingPush, string remoteUri)
        {
            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.ADD_REMOTE, (bool result, string text) =>
            {
                if (!result)
                {
                    UnityEngine.Debug.LogError("Git Collaboration: An error occured while adding the remote uri '" + remoteUri + "' as 'origin':<br/>" + text.Replace("\0", "<br/>"));

                    EngineDispatcher.Enqueue(new GitDialog()
                    {
                        text = "An error occured while adding the remote uri '" + remoteUri + "' as 'origin':\n" + text.Replace("\0", "\n") + "\n\nThe batch has been cancelled, no sync was attempted.",
                        positiveButton = "Investigate in Git Bash",
                        negativeButton = "Rollback",
                        onPositive = OpenGitBash,
                        onNegative = () => {
                            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.CUSTOM, null, "remote remove origin");
                        }
                    });
                }
                else
                {
                    if (syncUsingPush)
                    {
                        gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.PUSH_INIT_REMOTE, (bool result, string text) =>
                        {
                            if (!result)
                            {
                                if (CheckErrorForCommonIssues(text))
                                {
                                    //this has already been handled
                                }
                                else
                                {
                                    UnityEngine.Debug.LogError("Git Collaboration: An error occured while adding the remote uri '" + remoteUri + "' as 'origin':\n" + text.Replace("\0", "<br/>"));

                                    EngineDispatcher.Enqueue(new GitDialog()
                                    {
                                        text = "An error occured while syncing to the remote repository:" + text.Replace("\0", "\n") + "\nThe remote 'origin' has been removed.",
                                        positiveButton = "Investigate in Git Bash",
                                        negativeButton = "OK",
                                        onPositive = OpenGitBash
                                    });
                                }
                                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.CUSTOM, null, "remote remove origin");
                            }
                            else
                            {
                                EngineDispatcher.Enqueue(new GitDialog()
                                {
                                    text = "The remote 'origin' has successfully been added using uri '" + remoteUri + "'.Your working copy has been pushed to origin / " + GitControl.gitBranch,
                                    positiveButton = "OK",
                                    negativeButton = null,
                                    onPositive = () => {
                                        GitWindow.Repaint();
                                    }
                                });
                                UpdateGitStatus(true, null);
                            }
                        });
                    } 
                    else
                    {
                        gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.FETCH_ORIGIN, (bool result, string text) =>
                        {
                            if (!result)
                            {
                                UnityEngine.Debug.LogError("Git Collaboration: An error occured while fetching origin/" + GitControl.gitBranch + " - Does it exist?<br/><br/>Detailed error message: " + text.Replace("\0", "<br/>"));

                                EngineDispatcher.Enqueue(new GitDialog()
                                {
                                    text = "An error occured while fetching origin/" + GitControl.gitBranch + " - Does it exist?\n\nDetailed error message: " + text.Replace("\0", "\n") + "\n\nThe remote has been added but the merge was aborted (no file changes yet). You have to finish this manually.",
                                    negativeButton = "Rollback",
                                    onPositive = OpenGitBash,
                                    onNegative = () => {
                                        gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.CUSTOM, null, "remote remove origin");
                                    }
                                });
                            }
                            else
                            {
                                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.PULL_MERGE_REMOTE, (bool result, string text) =>
                                {
                                    if (!result)
                                    {
                                        UnityEngine.Debug.LogError("Git Collaboration: An error occured while merging '" + remoteUri + "' into the local repository:<br/>" + text.Replace("\0", "<br/>"));

                                        EngineDispatcher.Enqueue(new GitDialog()
                                        {
                                            text = "An error occured while merging '" + remoteUri + "' into the local repository:\n\n" + text.Replace("\0", "\n"),
                                            positiveButton = "Investigate in Git Bash",
                                            negativeButton = "OK",
                                            onPositive = OpenGitBash
                                        });
                                        gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.CUSTOM, null, "remote remove origin");
                                    }
                                    else
                                    {
                                        gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.PUSH_INIT_REMOTE, (bool result, string text) =>
                                        {
                                            if (!result)
                                            {
                                                if (CheckErrorForCommonIssues(text))
                                                {
                                                    //this has already been handled
                                                }
                                                else
                                                {
                                                    UnityEngine.Debug.LogError("Git Collaboration: An error occured while pushing to the remote uri '" + remoteUri + "' as 'origin':\n" + text.Replace("\0", "<br/>"));

                                                    EngineDispatcher.Enqueue(new GitDialog()
                                                    {
                                                        text = "An error occured while syncing to the remote repository:" + text.Replace("\0", "\n"),
                                                        positiveButton = "Investigate in Git Bash",
                                                        negativeButton = "OK",
                                                        onPositive = OpenGitBash
                                                    });
                                                }
                                                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.CUSTOM, null, "remote remove origin");
                                            }
                                            else
                                            {
                                                EngineDispatcher.Enqueue(new GitDialog()
                                                {
                                                    text = "The remote 'origin' has successfully been added using uri '" + remoteUri + "'. Your working copy has been merged with to origin/" + GitControl.gitBranch + ".\n\nThe merged repository has been pushed back to the origin.",
                                                    positiveButton = "OK",
                                                    negativeButton = null,
                                                    onPositive = () => {
                                                        GitWindow.Repaint();
                                                    }
                                                });
                                            }

                                            UpdateGitStatus(true, null);
                                        });
                                    }
                                });
                            }
                        });
                    }

                }
                
            }, remoteUri);
        }
#endregion


        /***
         * Call this method to update the current status of the repository and repaint the GUI.
         * If whenReady is provided, a remote fetch is enforced, else it may happen if required (based on the settings).
         * @param GitControl.GitResultCallback whenReady is a callback that will be called after completion of the update regardless of errors.
         **/
        public static void UpdateGitStatus(GitControl.GitResultCallback whenReady)
        {
            Instance._UpdateGitStatus(false, whenReady);
        }

        /***
         * Call this method to update the current status of the repository and repaint the GUI.
         * If whenReady is provided, a remote fetch is enforced, else it may happen if required (based on the settings).
         * @param bool force if set to true, this will re-enable status updates even if it was errored out before.
         * @param GitControl.GitResultCallback whenReady is a callback that will be called after completion of the update regardless of errors.
         **/
        public static void UpdateGitStatus(bool force, GitControl.GitResultCallback whenReady)
        {
            Instance._UpdateGitStatus(force, whenReady);
        }


        internal static void UpdateGitStatus()
        {
            Instance._UpdateGitStatus(false, null);
        }

        void _UpdateGitStatus(bool force, GitControl.GitResultCallback whenReady)
        {
            if (force) automatismStoppedBecauseOfErrors = false;

            if (whenReady == null && isProcessRunning) return; //This is not an important call so avoid conflicting actions
            if (whenReady == null && gitProc != null && !gitProc.HasExited)
            {
                Debug.LogWarning("Git Collaboration Warning: Something has just caused an inconsistency in process monitoring. Skipping a status update to avoid conflicting processes. Currently running command should be " + gitProc.runningCommand.ToString());
                return;
            }

            if (gitExePath == null || !File.Exists(gitExePath))
            {
                ProbeGitPath();
                if (gitExePath == null || !File.Exists(gitExePath))
                {
                    Debug.LogError("Git Collaboration: Could not detect Git executable. Please specify it's location manually in the plugin settings.");
                    return;
                }
            }
            if (_isTracking && !automatismStoppedBecauseOfErrors && (lastOnlineCheck < (Time.realtimeSinceStartup - EditorPrefs.GetInt("Firesplash.Git.OnlineCheckInterval", 180)) || force)) //force = Always fetch remote if running in force mode (only when tracking)
            {
                //isProcessRunning = true;

                gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.UPDATE, (bool result, string text) =>
                {
                    //isProcessRunning = false;
                    if (result == false)
                    {
                        CheckErrorForCommonIssues(text);

                        if (whenReady != null)
                        {
                            automatismStoppedBecauseOfErrors = true;
                            EngineDispatcher.Enqueue(new GitDialog()
                            {
                                text = "An error occured while fetching remote status during a scripted execution:\n" + text.Replace('\0', '\n') + "\n\nThe scripted execution has been cancelled and status updates have been disabled. To re-enable, click the ignore button below or the refresh button in the Git window.",
                                positiveButton = "OK (ignore this error for now)",
                                negativeButton = "Disable automatism",
                                onPositive = () => {
                                    automatismStoppedBecauseOfErrors = false;
                                },
                                onNegative = () => {
                                    UnityEngine.Debug.LogError("Git Collaboration: Status Updates have been disabled because of previous error(s). To re-enable, click the Refresh Status Button in the Git window.");
                                }
                            });
                        }
                    } 
                    else
                    {
                        lastOnlineCheck = Time.realtimeSinceStartup;
                    }
                    _UpdateGitStatus_Inner(result, (bool success, string result) =>
                    {
                        _CalculateGitStateColor();
                        whenReady?.Invoke(success, result);
                    });
                });
            }
            else
            {
                _UpdateGitStatus_Inner(true, (bool success, string result) =>
                {
                    _CalculateGitStateColor();
                    whenReady?.Invoke(success, result);
                });
            }
        }


        void _CalculateGitStateColor()
        {
            //Distinguish overall situation and change icon accordingly
            string newGitState = "Neutral";
            string newGitStateWord = "Please wait";

            if (_gitChangeSet != null && _gitChangeSet.Count > 0)
            {
                if (_isTrackingBehind)
                {
                    //Local changes uncommitted and remote changed
                    newGitState = "Red";
                    newGitStateWord = "Changed, Behind";
                }
                else
                {
                    newGitState = "Yellow"; //Local changes uncommitted
                    newGitStateWord = "Changed";
                }
            }
            else if (_isTrackingBehind)
            {
                newGitState = "Amber"; //Local changes committed but behind origin
                newGitStateWord = "Behind";
            }
            else if (_isTrackingAhead)
            {
                newGitState = "Blue"; //Local changes committed but ahead of origin
                newGitStateWord = "Ahead";
            }
            else if (_isTracking)
            {
                //Everything is committed locally and remote is on same revision as locally
                newGitState = "Green";
                newGitStateWord = "Synced";
            } 
            else
            {
                //Everything is committed locally and wea re not tracking
                newGitState = "Green";
                newGitStateWord = "Committed";
            }

            if (newGitState != _gitState)
            {
                _gitState = newGitState;
                _gitStateWord = newGitStateWord;
                GitWindow.DelayedRepaint();
            }
        }


        void _UpdateGitStatus_Inner(bool fetchSucessful, GitControl.GitResultCallback whenReady)
        {
            //isProcessRunning = true;
            gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.STATUS, (bool result, string text) =>
            {
                Dictionary<string, ChangeSetEntry> newChangeSet = new Dictionary<string, ChangeSetEntry>();
                List<string> newDirtyFolderSet = new List<string>();
                foreach (string record in text.Split('\0'))
                {
                    if (record.Length < 1) continue;
                    if (record.Substring(0, 2).Equals("##"))
                    {
                        try
                        {
                            //Status values we detect:
                            //## master...AS-Git-Collaboration/master
                            //## master...AS-Git-Collaboration/master [ahead 1]
                            //## No commits yet on master
                            //## master

                            _gitStatusData = record;

                            if (_gitStatusData.StartsWith("## No commits yet on "))
                            {
                                //Special case if no commits on this branch
                                _gitBranch = _gitStatusData.Substring(21);
                                _gitRepoInitialized = false;
                                _gitTracking = null;
                                _isTracking = false;
                                _isTrackingBehind = false;
                                _isTrackingBehind = false;
                                continue;
                            }

                            int posRemote = _gitStatusData.IndexOf("...");
                            _gitRepoInitialized = true;

                            if (posRemote < 0)
                            {
                                _gitBranch = _gitStatusData.Substring(3); //There is no remote stuff so just read to end
                                _gitTracking = null;
                                _isTracking = false;
                                _isTrackingBehind = false;
                                _isTrackingBehind = false;
                            }
                            else
                            {
                                _gitBranch = _gitStatusData.Substring(3, posRemote - 3);
                                _isTracking = true;

                                if (_gitStatusData.IndexOf("[") > 0)
                                {
                                    _gitTracking = _gitStatusData.Substring(posRemote + 3, _gitStatusData.IndexOf(" [") - posRemote - 3);

                                    gitTrackStatusRaw = _gitStatusData.Substring(_gitStatusData.IndexOf("[") + 1, _gitStatusData.IndexOf("]") - _gitStatusData.IndexOf("[") - 1);
                                    _isTrackingBehind = gitTrackStatusRaw.Contains("behind");
                                    _isTrackingAhead = gitTrackStatusRaw.Contains("ahead");
                                    _gitTrackStatusReadable = "";
                                    if (_isTrackingAhead)
                                    {
                                        if (_isTrackingBehind) _gitTrackStatusReadable += gitTrackStatusRaw.Substring(gitTrackStatusRaw.IndexOf(' ') + 1, gitTrackStatusRaw.IndexOf(", behind") - gitTrackStatusRaw.IndexOf(' ') - 1) + " commit(s) ahead";
                                        else _gitTrackStatusReadable += gitTrackStatusRaw.Substring(gitTrackStatusRaw.IndexOf(' ') + 1) + " commit(s) ahead";
                                    }
                                    if (_isTrackingBehind)
                                    {
                                        if (_gitTrackStatusReadable.Length > 1) _gitTrackStatusReadable += ", ";
                                        _gitTrackStatusReadable += gitTrackStatusRaw.Substring(gitTrackStatusRaw.LastIndexOf(' ') + 1) + " commit(s) behind";
                                    }
                                    _gitTrackStatusReadable += " remote";
                                }
                                else
                                {
                                    _gitTracking = _gitStatusData.Substring(posRemote + 3);

                                    gitTrackStatusRaw = "";
                                    _gitTrackStatusReadable = "Both sides are up to date";
                                    _isTrackingBehind = false;
                                    _isTrackingAhead = false;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _isTracking = true; //We don't know it so assume we are tracking.
                            UnityEngine.Debug.LogWarning("Git Collaboration Error: Could not parse general status information '" + _gitStatusData + "' as of " + e.ToString() + " - Please report this to assets@firesplash.de");
                        };
                    }
                    else
                    {
                        //This is called for each file in the change set

                        if (record.Length < 3 || record.Substring(0, 2).Equals("##")) continue; //not a changed file

                        string changeType = record.Substring(0, 2);
                        string changeTextual = "";
                        string changedFile = record.Substring(2).Trim().Replace("\"", "");
                        string fileColor = "Amber";
                        string wcChange = "", idxChange = "";

                        //Merge conflict states
                        if (changeType.Equals("DD")) changeTextual = "deleted on both sides";
                        else if (changeType.Equals("AU")) changeTextual = "Added - Only here";
                        else if (changeType.Equals("UD")) changeTextual = "Obsolete - Del. there";
                        else if (changeType.Equals("UA")) changeTextual = "Missing - Added there";
                        else if (changeType.Equals("DU")) changeTextual = "Deleted here";
                        else if (changeType.Equals("AA")) changeTextual = "Added @ both sides";
                        else if (changeType.Equals("UU")) changeTextual = "modified @ both sides";
                        else if (changeType.Equals("AU")) changeTextual = "Added - Only here";
                        else
                        {

                            //Staged but uncommitted
                            if (changeType.Substring(0, 1).Equals("M")) idxChange = "Staged modifications";
                            else if (changeType.Substring(0, 1).Equals("R")) idxChange = "Staged Rename/Move";
                            else if (changeType.Substring(0, 1).Equals("D")) idxChange = "Staged Delete";
                            else if (changeType.Substring(0, 1).Equals("A")) idxChange = "Staged Create";
                            else if (changeType.Substring(0, 1).Equals("C")) idxChange = "Staged Copy";

                            //unstaged
                            if (changeType.Substring(1, 1).Equals("M")) wcChange = "Unstaged modifications";
                            else if (changeType.Substring(1, 1).Equals("R")) wcChange = "Unstaged Rename/Move";
                            else if (changeType.Substring(1, 1).Equals("D")) wcChange = "Unstaged Delete";
                            else if (changeType.Substring(1, 1).Equals("A")) wcChange = "New unstaged file";
                            else if (changeType.Substring(1, 1).Equals("C")) wcChange = "New unstaged file (Copy)";
                            else if (changeType.Substring(1, 1).Equals("?")) wcChange = "Untracked file";
                            else if (changeType.Substring(1, 1).Equals("!")) wcChange = "Ignored file";


                            //Evaluate results for storing file states
                            if (wcChange != "")
                            {
                                fileColor = "Amber";
                            }
                            else if (idxChange != "")
                            {
                                //This means that this file is staged (and not changed afterwards)
                                fileColor = "Green";
                            }

                        }

                        newChangeSet.Add(changedFile, new ChangeSetEntry
                        {
                            fileName = changedFile,
                            changeCode = changeType,
                            otherChange = changeTextual,
                            wcChange = wcChange,
                            idxChange = idxChange,
                            fileColorState = fileColor
                        });

                        if (EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewFolderIcons", true)) {
                            string path = changedFile;
                            while (path != null)
                            {
                                path = Path.GetDirectoryName(path).Replace("\\", "/");
                                if (path == null || !path.StartsWith("Assets") || path.Equals("Assets")) break;
                                if (newDirtyFolderSet.Contains(path)) break; //as soon as we find a matching deep path, all parents must also be already there
                                newDirtyFolderSet.Add(path);
                            }
                        }
                    }
                }

                if (gitTracking != null && isTrackingBehind)
                {
                    gitProc = new GitProcess(gitExePath, GitProcess.GitCommand.REMOTECHECK, (bool result, string text) =>
                    {
                        foreach (string record in text.Split('\0'))
                        {
                            if (newChangeSet.ContainsKey(record))
                            {
                                newChangeSet[record].FlagRemoteChanged();
                            }
                            else
                            {
                                newChangeSet.Add(record, new ChangeSetEntry
                                {
                                    fileName = record,
                                    fileColorState = "Red",
                                    remoteChanged = true,
                                    otherChange = "Changed remotely"
                                });
                            }
                        }

                        _gitChangeSet = newChangeSet;
                        _dirtyFolderSet = newDirtyFolderSet;
                        lastStatusCheck = Time.realtimeSinceStartup;
                        if (whenReady != null)
                        {
                            EngineDispatcher.Enqueue(() =>
                            {
                                whenReady.Invoke((fetchSucessful ? result : false), text);
                            });
                        }
                        GitWindow.DelayedRepaint();
                    }, gitBranch + "..." + gitTracking);
                } 
                else
                {
                    _gitChangeSet = newChangeSet;
                    _dirtyFolderSet = newDirtyFolderSet;
                    
                    lastStatusCheck = Time.realtimeSinceStartup;
                    if (whenReady != null)
                    {
                        EngineDispatcher.Enqueue(() =>
                        {
                            whenReady.Invoke((fetchSucessful ? result : false), text);
                        });
                    }
                    GitWindow.DelayedRepaint();
                }

                //Read gitignores if present
                _ignoredFilesList = new List<string>();
                string ignoreFilePath = Path.GetFullPath(Application.dataPath.Replace('/', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".gitignore");
                if (File.Exists(ignoreFilePath))
                {
                    foreach(string line in File.ReadAllLines(ignoreFilePath))
                    {
                        if (line.StartsWith("Assets/"))
                        {
                            _ignoredFilesList.Add(line);
                        }
                    }
                }
            });
        }
    }
}
