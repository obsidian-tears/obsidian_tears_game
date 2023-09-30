using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Firesplash.GameDevAssets.GitCollab
{
    internal class GitQuickCommands
    {
        [MenuItem("Tools/FSE Git Collaboration/&Install a standard Git-Ignores file")]
        public static void InstallGitignoreTemplate()
        {
            string path = Path.GetFullPath(Application.dataPath.Replace('/', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + ".gitignore");
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog("Overwrite existing file?", "You are trying to install a default .gitignore template recommended for Unity projects.\nYour working tree already has a .gitignore file. Do you really want to overwrite it with the template?\n\nHint: This is a public template provided by GitHub under the CC0 license.", "Yes overwrite the file", "No, cancel .gitignore installation")) return;
            }

            UnityEditorWebRequest uwr = new UnityEditorWebRequest(UnityWebRequest.Get("https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore"));
            uwr.SendWebRequest((uwr) =>
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    File.WriteAllText(path, uwr.downloadHandler.text + "\n\n#Do not commit git collaboration tool (this should be downloaded from the asset store on each workstation)\n/Assets/Firesplash Entertainment/Git Collaboration/\n/Assets/Firesplash Entertainment/Git Collaboration.meta\n");
                    Debug.Log("A default .gitignore file has been installed into " + path + " - Please review the file before continuing your work!");
                }
                else
                {
                    Debug.LogError("Error downloading gitignore template: " + uwr.result.ToString() + " (" + uwr.error + ")");
                }
            });
        }

        [MenuItem("Tools/FSE Git Collaboration/Initialize and update &submodules (only)")]
        public static void GitInitSubmodules()
        {
            GitControl.GitSubmoduleInit(null);
        }

        [MenuItem("Tools/FSE Git Collaboration/Update (pull) submodules from remote")]
        public static void GitUpdateSubmodules()
        {
            GitControl.GitSubmodulesUpdate(null);
        }
    }
}
