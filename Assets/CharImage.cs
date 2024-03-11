using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private string imageUrl;


    private void Start()
    {
        string imageUrl = ICConnect.characterUrl;

        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                spriteRenderer.sprite = sprite;
            }
          
        }
    }
}
