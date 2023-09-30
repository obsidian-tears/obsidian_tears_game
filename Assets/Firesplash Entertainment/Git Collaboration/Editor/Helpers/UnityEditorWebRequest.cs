using System;
using System.Collections;
using UnityEditor;
using UnityEngine.Networking;

namespace Firesplash.GameDevAssets.GitCollab
{
    public class UnityEditorWebRequest
    {
        UnityWebRequest request;
        Action<UnityWebRequest> callback;

        public UnityEditorWebRequest(UnityWebRequest uwr)
        {
            this.request = uwr;
        }

        public void SendWebRequest(Action<UnityWebRequest> callback)
        {
            this.callback = callback;
            request.SendWebRequest();
            EditorApplication.update += this.EditorWebRequestStep;
        }
        
        void EditorWebRequestStep()
        {
            if (request.isDone)
            {
                EditorApplication.update -= this.EditorWebRequestStep;
                callback.Invoke(request);
            }
        }
    }
}
