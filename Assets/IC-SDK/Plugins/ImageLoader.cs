using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;

namespace IC_SDK
{
    public class ImageLoader : MonoBehaviour
    {
        public static ImageLoader Instance;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There is an existing Image Loader, destroying...");
                Destroy(this);
                return;
            }

            Instance = this;
        }
        
        public void AssignImage(string url, Image component, Action<Sprite> callback)
        {
            StartCoroutine(DownloadImage(url, component, callback));
        }

        private IEnumerator DownloadImage(string url, Image component, Action<Sprite> callback)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback(null);
            }
            else
            {
                var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                component.sprite = sprite;
                callback(sprite);
            }
        }
    }
}
