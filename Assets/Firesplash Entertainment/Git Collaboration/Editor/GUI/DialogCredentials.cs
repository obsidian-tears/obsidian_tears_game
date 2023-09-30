using UnityEngine;
using UnityEditor;
using System.IO;

namespace Firesplash.GameDevAssets.GitCollab.GUI
{
    class DialogCredentials : EditorWindow
    {
        string _username;
        string _password;

        internal DialogCredentials()
        {
            titleContent = new GUIContent("Git Collaboration: Enter credentials for remote repository");

            this.minSize = new Vector2(600, 170);
            this.maxSize = new Vector2(600, 300);
        }

        private void OnEnable()
        {
            _username = EditorPrefs.GetString("Firesplash.Git.Credentials.Username");
            _password = EditorPrefs.GetString("Firesplash.Git.Credentials.Password");
        }

        [MenuItem("Tools/FSE Git Collaboration/Setup Password Authentication (legacy)")]
        public static void ShowWindow()
        {
            ((DialogCredentials)EditorWindow.GetWindow(typeof(DialogCredentials))).ShowModal();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("If your remote repository requires authenticating using username and password, you can provide them here. If it requires a more complex authentication, you will have to set up your git installation to automatically authenticate using \"git config\". See the documentation PDF for more information on how to achieve this.\n\nPlease note, that most public git providers do not allow User/Password authentication anymore.", MessageType.Info);

            if (!File.Exists(Application.dataPath + Path.DirectorySeparatorChar + "Firesplash Entertainment" + Path.DirectorySeparatorChar + "Git Collaboration" + Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar + "Helperscripts"))
            {
                EditorGUILayout.HelpBox("You did not install the helper scripts. This must be done manually according to Unity's Asset Store Agreement. Please visit our documentation for instructions.", MessageType.Error);
            }
            else
            {
                _username = EditorGUILayout.TextField("Username", _username);
                _password = EditorGUILayout.PasswordField("Password", _password);

                EditorGUILayout.HelpBox("Credentials are stored on your computer in unencrypted form using EditorPrefs.\nThey are not validated in this dialog.", MessageType.Warning);

                GUILayout.Space(2);
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            if (GUILayout.Button("Save"))
            {
                EditorPrefs.SetString("Firesplash.Git.Credentials.Username", _username);
                EditorPrefs.SetString("Firesplash.Git.Credentials.Password", _password);
                GitControl.UpdateGitStatus(true, null);
                Close();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
