using Firesplash.GameDevAssets.GitCollab.GUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Firesplash.GameDevAssets.GitCollab
{
    internal class GitProcess : Process
    {
        public enum GitCommand { CUSTOM, INIT, ADD, COMMIT, PULL, REVERT_UNSTAGED, UNSTAGE, SUBMOD_INIT, SUBMOD_UPDATE, ABORT_MERGE, PUSH, CLEAN, STATUS, ADD_REMOTE, FETCH_ORIGIN, PUSH_INIT_REMOTE, PULL_MERGE_REMOTE, UPDATE, REMOTECHECK };

        bool logMessages = false;
        StringBuilder output, errors;

        public GitCommand runningCommand { get; private set; }

        GitControl.GitResultCallback onFinishCallback;

        public GitProcess(string gitExePath, GitCommand command, GitControl.GitResultCallback onFinish)
        {
            GitProcessInit(gitExePath, command, onFinish, null);
        }

        public GitProcess(string gitExePath, GitCommand command, GitControl.GitResultCallback onFinish, string extraData)
        {
            GitProcessInit(gitExePath, command, onFinish, extraData);
        }

        public void GitProcessInit(string gitExePath, GitCommand command, GitControl.GitResultCallback onFinish, string extraData)
        {
            runningCommand = command;
            onFinishCallback = onFinish;

            errors = new StringBuilder();
            output = new StringBuilder();
            string lastLine = "";

            this.StartInfo.UseShellExecute = false; //Changing this will break events and streams!
            this.StartInfo.FileName = gitExePath;
            this.StartInfo.CreateNoWindow = true;
            this.StartInfo.LoadUserProfile = true;
            this.EnableRaisingEvents = true;
            this.Exited += new EventHandler(GitProcessFinished);
            this.StartInfo.RedirectStandardError = true;
            this.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => {
                lastLine = e.Data;
                errors.Append(e.Data);
            });
            this.StartInfo.RedirectStandardOutput = true;
            this.StartInfo.RedirectStandardInput = true;
            this.OutputDataReceived += new DataReceivedEventHandler((sender, e) => {
                lastLine = e.Data;
                output.Append(e.Data);
            });
            
            if (GitControl.cfgIgnoreGlobalConfig) this.StartInfo.EnvironmentVariables.Add("GIT_CONFIG_NOSYSTEM", "true");

            if (!EditorPrefs.GetBool("Firesplash.Git.Identity.UseConfigFile", true))
            {
                string iName = EditorPrefs.GetString("Firesplash.Git.Identity.Name", System.Environment.GetEnvironmentVariable("USERNAME"));
                string iMail = EditorPrefs.GetString("Firesplash.Git.Identity.EMail", iName + "@localhost");
                this.StartInfo.EnvironmentVariables.Add("GIT_AUTHOR_NAME", iName);
                this.StartInfo.EnvironmentVariables.Add("GIT_COMMITTER_NAME", iName);
                this.StartInfo.EnvironmentVariables.Add("GIT_AUTHOR_EMAIL", iMail);
                this.StartInfo.EnvironmentVariables.Add("GIT_COMMITTER_EMAIL", iMail);
            }

            if (EditorPrefs.GetString("Firesplash.Git.GIT_SSH", "").Length > 0)
            {
                this.StartInfo.EnvironmentVariables.Add("GIT_SSH", EditorPrefs.GetString("Firesplash.Git.GIT_SSH", ""));
            }

            this.StartInfo.EnvironmentVariables.Add("GIT_USERNAME", EditorPrefs.GetString("Firesplash.Git.Credentials.Username", ""));
            this.StartInfo.EnvironmentVariables.Add("GIT_PASSWORD", EditorPrefs.GetString("Firesplash.Git.Credentials.Password", ""));
#if UNITY_EDITOR_WIN
            this.StartInfo.EnvironmentVariables.Add("GIT_CRED_RUN", "0");
            this.StartInfo.EnvironmentVariables.Add("GIT_ASKPASS", Application.dataPath.Replace('/', '\\') + Path.DirectorySeparatorChar + "Firesplash Entertainment" + Path.DirectorySeparatorChar + "Git Collaboration" + Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar + "Helperscripts" + Path.DirectorySeparatorChar + "credentialhelper.bat");
#else
            this.StartInfo.EnvironmentVariables.Add("GIT_ASKPASS", Application.dataPath.Replace('/', '\\') + Path.DirectorySeparatorChar + "Firesplash Entertainment" + Path.DirectorySeparatorChar + "Git Collaboration" + Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar + "Helperscripts" + Path.DirectorySeparatorChar + "credentialhelper.sh");
#endif

            switch (command)
            {
                case GitCommand.INIT:
                    this.StartInfo.Arguments = "init";
                    break;

                case GitCommand.ADD:
                    this.StartInfo.Arguments = "add " + extraData;
                    break;

                case GitCommand.COMMIT:
                    if (extraData == null) throw new InvalidOperationException("Commit requires extraData");
                    if (EditorPrefs.GetBool("Firesplash.Git.EnableSigning", false))
                    {
                        this.StartInfo.Arguments = "commit -S -F \"" + extraData + "\"";
                    } 
                    else
                    {
                        this.StartInfo.Arguments = "commit -F \"" + extraData + "\"";
                    }
                    break;

                case GitCommand.PULL:
                    this.StartInfo.Arguments = "pull --ff --no-edit --recurse-submodules --jobs=10";
                    logMessages = true;
                    break;

                case GitCommand.SUBMOD_UPDATE:
                    this.StartInfo.Arguments = "submodule update --recursive --remote";
                    logMessages = true;
                    break;

                case GitCommand.REVERT_UNSTAGED:
                    this.StartInfo.Arguments = "checkout -- " + extraData;
                    logMessages = true;
                    break;

                case GitCommand.UNSTAGE:
                    this.StartInfo.Arguments = "restore --staged " + extraData;
                    logMessages = true;
                    break;

                case GitCommand.SUBMOD_INIT:
                    this.StartInfo.Arguments = "submodule update --recursive --init";
                    logMessages = true;
                    break;

                case GitCommand.ABORT_MERGE:
                    this.StartInfo.Arguments = "merge --abort";
                    break;

                case GitCommand.PUSH:
                    this.StartInfo.Arguments = "push -q --porcelain";
                    logMessages = true;
                    break;

                case GitCommand.STATUS:
                    this.StartInfo.Arguments = "status -b --porcelain=1 -z";
                    break;

                case GitCommand.REMOTECHECK:
                    this.StartInfo.Arguments = "diff -b -z --name-only " + extraData;
                    break;

                case GitCommand.ADD_REMOTE:
                    this.StartInfo.Arguments = "remote add origin " + extraData;
                    break;

                case GitCommand.PUSH_INIT_REMOTE:
                    this.StartInfo.Arguments = "push -u origin " + GitControl.gitBranch;
                    break;

                case GitCommand.PULL_MERGE_REMOTE:
                    this.StartInfo.Arguments = "merge origin/" + GitControl.gitBranch;
                    break;

                case GitCommand.FETCH_ORIGIN:
                    this.StartInfo.Arguments = "fetch origin";
                    break;

                case GitCommand.UPDATE:
                    this.StartInfo.Arguments = "remote update";
                    break;

                case GitCommand.CUSTOM:
                    this.StartInfo.Arguments = extraData;
                    break;
            }
            try
            {
                this.Start();
                this.BeginErrorReadLine();
                this.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                if (onFinishCallback != null)
                {
                    EngineDispatcher.Enqueue(() => { 
                        UnityEngine.Debug.LogError("Git Collaboration: Exception while running git process: " + e.ToString());onFinishCallback?.Invoke(false, "Exception: " + e.ToString());
                    });
                }
            }
        }

        private void GitProcessFinished(object sender, System.EventArgs e)
        {
            bool wasSuccessful = (this.ExitCode == 0);

            if (logMessages && output.Length > 10) //We will log messages that are more than a simple Done or Error etc.
            {
                string[] outputLines = output.ToString().Split('\0');
                if (outputLines != null && outputLines.Length > 0 && outputLines[0].ToLower().StartsWith("locking support detected on remote") && GitControl.cfgAskLockingSupport) {
                    EngineDispatcher.Enqueue(new GitDialog() {
                        title = "LFS Locking Support",
                        negativeButton = "No, and don't ask again",
                        positiveButton = "Yes, please enable it for me",
                        text = "Git LFS locking support has been detected on the remote side. Do you want to enable it?",
                        onPositive = new Action(() => {
                            new GitProcess(GitControl.gitExePath, GitCommand.CUSTOM, null, outputLines[0].Substring(outputLines[0].IndexOf("$ git") + 5));
                        }),
                        onNegative = new Action(() => {
                            UnityEditor.EditorPrefs.SetBool("Firesplash.Git.RequestLockingSupport", false);
                        }),
                    });
                }
                else UnityEngine.Debug.Log("Git Collaboration: " + output.Replace("\0", " \n"));
            }

            if (!wasSuccessful)
            {
                if (onFinishCallback != null)
                {
                    EngineDispatcher.Enqueue(() => {
                        if (GitControl.cfgMoreDebugging)
                        {
                            UnityEngine.Debug.LogError("Full output of git's " + runningCommand.ToString() + " command: \n" + errors.ToString().Replace("\0", " \n"));
                        }
                        onFinishCallback.Invoke(false, (errors.Length > 0 ? errors.ToString().Replace("\0", " \n") : output.ToString().Replace("\0", " \n")));
                    });
                }
            } 
            else
            {
                if (onFinishCallback != null)
                {
                    EngineDispatcher.Enqueue(() => {
                        if (GitControl.cfgMoreDebugging)
                        {
                            UnityEngine.Debug.Log("Full output of git's " + runningCommand.ToString() + " command: \n" + output.ToString().Replace("\0", " \n"));
                        }
                        onFinishCallback.Invoke(true, output.ToString());
                    });
                }
            }

            GitWindow.DelayedRepaint();
        }
    }
}
