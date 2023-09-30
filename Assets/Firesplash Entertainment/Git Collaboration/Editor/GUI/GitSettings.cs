using System.IO;
using UnityEditor;
using UnityEngine;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    internal class GitSettings : EditorWindow
    {
        Vector2 scrollPosition;
        string _authorName;
        string _authorEMail;
        string _gitSSH;
        bool _useGitAuthorConfigFile;
        int _maxDisplayedChangeSetRows;
        int _normalStatusUpdateInterval;
        int _onlineStatusUpdateInterval;
        bool _enableAdaptiveIntervals;
        bool _enableGuiOverlays;
        bool _enableGuiTreeOverlays;
        bool _enableGuiIgnoreOverlays;
        bool _askLockingSupport;
        bool _moreDebugging;
        bool _ignoreGlobalConfig;
        bool _signCode;

        GUIStyle boldtext;

        public GitSettings()
        {
            this.minSize = new Vector2(700, 400);
        }

        private void OnEnable()
        {
            _useGitAuthorConfigFile = EditorPrefs.GetBool("Firesplash.Git.Identity.UseConfigFile", true);
            _authorName = EditorPrefs.GetString("Firesplash.Git.Identity.Name", System.Environment.GetEnvironmentVariable("USERNAME"));
            _authorEMail = EditorPrefs.GetString("Firesplash.Git.Identity.EMail", _authorName + "@localhost");

            _gitSSH = EditorPrefs.GetString("Firesplash.Git.GIT_SSH", "");

            _normalStatusUpdateInterval = EditorPrefs.GetInt("Firesplash.Git.StatusCheckInterval", 30);
            _onlineStatusUpdateInterval = EditorPrefs.GetInt("Firesplash.Git.OnlineCheckInterval", 180);
            _maxDisplayedChangeSetRows = EditorPrefs.GetInt("Firesplash.Git.MaxDisplayedChangeSetRows", 250);
            _enableAdaptiveIntervals = EditorPrefs.GetBool("Firesplash.Git.EnableAdaptiveChecks", true);
            _enableGuiOverlays = EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewIcons", true);
            _enableGuiTreeOverlays = EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewFolderIcons", true);
            _enableGuiIgnoreOverlays = EditorPrefs.GetBool("Firesplash.Git.EnableProjectViewIgnoreIcons", false);
            _askLockingSupport = EditorPrefs.GetBool("Firesplash.Git.RequestLockingSupport", true);
            _moreDebugging = EditorPrefs.GetBool("Firesplash.Git.MoreDebugging", false);
            _ignoreGlobalConfig = EditorPrefs.GetBool("Firesplash.Git.IgnoreGlobalConfig", false);
            _signCode = EditorPrefs.GetBool("Firesplash.Git.EnableSigning", false);
        }

        [MenuItem("Tools/FSE Git Collaboration/Plugin Settings")]
        public static void ShowWindow()
        {
            ((GitSettings)EditorWindow.GetWindow(typeof(GitSettings))).ShowPopup();
        }

        private void OnGUI()
        {
            if (boldtext == null) boldtext = new GUIStyle(UnityEngine.GUI.skin.label) { fontStyle = FontStyle.Bold };

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            titleContent = EditorGUIUtility.TrTextContentWithIcon("Git Collaboration Settings", "Version Control System utilizing Git", "Assets/Firesplash Entertainment/Git Collaboration/Editor/Resources/FS_Git_Neutral.png");

            EditorGUILayout.LabelField("Commit Identity setup", boldtext);
            _useGitAuthorConfigFile = EditorGUILayout.ToggleLeft("Use the identity values from my git configuration", _useGitAuthorConfigFile);
            if (_useGitAuthorConfigFile)
            {
                EditorGUILayout.HelpBox("This plugin basically works by controlling git calls and converting them to an editor-integrated GUI. All configurations of your system's git config also apply to this plugin by default. To configure your identity, see the following link", MessageType.Info);
                if (GUILayout.Button("Show docs: https://git-scm.com/docs/user-manual#telling-git-your-name"))
                {
                    Application.OpenURL("https://git-scm.com/docs/user-manual#telling-git-your-name");
                }
            }
            else
            {
                _authorName = EditorGUILayout.TextField("Author name", _authorName);
                _authorEMail = EditorGUILayout.TextField("Author E-Mail", _authorEMail);
            }
            EditorGUILayout.Space(16);

            EditorGUILayout.LabelField("Status Update Intervals in seconds", boldtext);
            _normalStatusUpdateInterval = (int)EditorGUILayout.Slider("Local changes", _normalStatusUpdateInterval, 10, 900);
            _enableAdaptiveIntervals = EditorGUILayout.ToggleLeft("Make the local interval adaptive", _enableAdaptiveIntervals);
            _onlineStatusUpdateInterval = (int)EditorGUILayout.Slider("Tracked repository", _onlineStatusUpdateInterval, 30, 3600);
            EditorGUILayout.HelpBox("The Git Window works with caching to reduce the overhead caused by communicating with Git. You can set the update interval for your local repositroy (this is effectivaly the maximum time between a change in your project and the change popping up in the Git window) as well as a different interval for checking against your remote repository (which usually changes slower).\nAdditionally the remote update is always only done in connection to a local update so the maximum time between two remote checks in your current configuration is between " + _onlineStatusUpdateInterval + " and " + (_normalStatusUpdateInterval + _onlineStatusUpdateInterval) + " seconds.\nIf you enable adaptive intervals, the local update will happen more often if the project has changed (about every " + Mathf.FloorToInt(_normalStatusUpdateInterval/4f) + " to " + _normalStatusUpdateInterval + " seconds)\nBaseline: The bigger your project and the worse your hardware is, the higher values you should set. Tracked can be very high for single devs and should be lower for teams.", MessageType.Info);
            EditorGUILayout.Space(16);

            EditorGUILayout.LabelField("Other Settings", boldtext);
#if UNITY_EDITOR_WIN
            EditorGUILayout.BeginHorizontal();
            _gitSSH = EditorGUILayout.TextField("Override GIT_SSH", _gitSSH);
            if (GUILayout.Button("Browse...", new GUILayoutOption[] { GUILayout.Width(100) }))
            {
                _gitSSH = EditorUtility.OpenFilePanel("Find a working SSH-Implementation for Git (e.g. plink.exe)", "C:\\", "exe").Replace('/', '\\');
            }
            EditorGUILayout.EndHorizontal();
            if (_gitSSH.Length < 1)
            {
                if (System.Environment.GetEnvironmentVariable("GIT_SSH")?.Length > 0)
                {
                    EditorGUILayout.HelpBox("If you keep this field empty, GIT_SSH is not overridden and your system wide setting '" + System.Environment.GetEnvironmentVariable("GIT_SSH") + "' is used.", MessageType.Info);
                } 
                else
                {
                    EditorGUILayout.HelpBox("If you keep this field empty, GIT_SSH is not overridden. You can use this for example to configure SSH-Key-Authentication.\nA typical windows value is C:\\Program Files\\PuTTY\\plink.exe to enable agent based key authentication (A full Putty needs to be installed)", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Excerpt from the official documentation: If specified, [$GIT_SSH] is invoked like '$GIT_SSH [username@]host [-p <port>] <command>'\nA typical windows value is C:\\Program Files\\PuTTY\\plink.exe to enable agent based key authentication (A full Putty needs to be installed)", MessageType.Info);
                if (GUILayout.Button("Show docs: https://git-scm.com/book/en/v2/Git-Internals-Environment-Variables#_miscellaneous"))
                {
                    Application.OpenURL("https://git-scm.com/book/en/v2/Git-Internals-Environment-Variables#_miscellaneous");
                }
            }
#endif
            _maxDisplayedChangeSetRows = (int)EditorGUILayout.Slider("Max. ChangeSet Rows", _maxDisplayedChangeSetRows, 100, 9999);
            _enableGuiOverlays = EditorGUILayout.ToggleLeft("Enable git icons in project view", _enableGuiOverlays);
            if (_enableGuiOverlays)
            {
                _enableGuiTreeOverlays = EditorGUILayout.ToggleLeft("  |-> Add additional amber dot to dirty folders", _enableGuiTreeOverlays);
                _enableGuiIgnoreOverlays = EditorGUILayout.ToggleLeft("  '-> Add icon to ignored files (Only works for explicit full path ignores!)", _enableGuiIgnoreOverlays);
            }
            _askLockingSupport = EditorGUILayout.ToggleLeft("Ask to enable locking support if git recommends it", _askLockingSupport);


            _ignoreGlobalConfig = EditorGUILayout.ToggleLeft("Ignore system-wide git configuration (Experimental)", _ignoreGlobalConfig);
            if (_ignoreGlobalConfig && !_useGitAuthorConfigFile)
            {
                EditorGUILayout.HelpBox("This option should only be set in very rare cases when you got a global configuration that is entirely incompatible with this plugin as it removes many configuration options. If it is possible for you, you should preferr adjusting the config over setting this option.", MessageType.Warning);
            } 
            else if (_ignoreGlobalConfig && _useGitAuthorConfigFile)
            {
                EditorGUILayout.HelpBox("You can not use your configured identity without using the configuration file.", MessageType.Error);
            } 
            else
            {
                _signCode = EditorGUILayout.ToggleLeft("Sign Commits using an externally configured (git config) GPG-Key (Experimental)", _signCode);
            }
            _moreDebugging = EditorGUILayout.ToggleLeft("Enable extended debug logging", _moreDebugging);
            EditorGUILayout.Space(16);

            EditorGUILayout.LabelField("This plugin is using git executable from:   " + EditorPrefs.GetString("Firesplash.Git.GitPath", "-- No git found --"));
            if (GUILayout.Button("Pick (another) git executable")) OpenGitPicker();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUILayout.Button("Save settings"))
            {
                EditorPrefs.SetBool("Firesplash.Git.Identity.UseConfigFile", _useGitAuthorConfigFile);
                EditorPrefs.SetString("Firesplash.Git.Identity.Name", _authorName);
                EditorPrefs.SetString("Firesplash.Git.Identity.EMail", _authorEMail);
                EditorPrefs.SetInt("Firesplash.Git.StatusCheckInterval", _normalStatusUpdateInterval);
                EditorPrefs.SetInt("Firesplash.Git.OnlineCheckInterval", _onlineStatusUpdateInterval);
                EditorPrefs.SetInt("Firesplash.Git.MaxDisplayedChangeSetRows", _maxDisplayedChangeSetRows);
                EditorPrefs.SetBool("Firesplash.Git.EnableAdaptiveChecks", _enableAdaptiveIntervals);
                EditorPrefs.SetBool("Firesplash.Git.EnableProjectViewIcons", _enableGuiOverlays);
                EditorPrefs.SetBool("Firesplash.Git.EnableProjectViewFolderIcons", _enableGuiTreeOverlays);
                EditorPrefs.SetBool("Firesplash.Git.EnableProjectViewIgnoreIcons", _enableGuiIgnoreOverlays);
                EditorPrefs.SetBool("Firesplash.Git.RequestLockingSupport", _askLockingSupport);
                EditorPrefs.SetBool("Firesplash.Git.IgnoreGlobalConfig", _ignoreGlobalConfig);
                EditorPrefs.SetBool("Firesplash.Git.MoreDebugging", _moreDebugging);
                EditorPrefs.SetBool("Firesplash.Git.EnableSigning", _signCode);
                EditorPrefs.SetString("Firesplash.Git.GIT_SSH", _gitSSH);
                GitControl.ReadSettings();
                Close();
            }
        }

        internal static void OpenGitPicker()
        {
            while (true)
            {
#if UNITY_EDITOR_WIN
                string path = EditorUtility.OpenFilePanel("Find git.exe to use with Git Collaboraiton Tools", "", "exe");
                if (path.Length != 0 && Path.GetFileName(path) == "git.exe")
#else
                string path = EditorUtility.OpenFilePanel("Find git executable to use with Git Collaboraiton Tools", "", "");
                if (path.Length != 0 && Path.GetFileName(path) == "git")
#endif
                {
                    EditorPrefs.SetString("Firesplash.Git.GitPath", Path.GetDirectoryName(path));
                }
                else if (path.Length == 0)
                {
                    break;
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Git Path", "The path you entered seems invalid. Please pick the git executable file from your Git installation.", "Repick");
                }
            }
        }
    }
}
