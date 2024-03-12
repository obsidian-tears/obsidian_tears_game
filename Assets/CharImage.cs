using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CharImage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private string imageUrl;



    private void Start()
    {

        imageUrl = ICConnect.characterUrl;
        StartCoroutine(LoadImage(imageUrl, _spriteRenderer));


    }


    IEnumerator LoadImage(string image, SpriteRenderer imagRenderer)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        imagRenderer = playerObject.GetComponent<SpriteRenderer>();

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(image))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                imagRenderer.sprite = sprite;
            }
          
        }
    }
}
