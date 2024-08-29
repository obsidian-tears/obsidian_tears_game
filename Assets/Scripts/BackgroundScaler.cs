using System.Collections;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        var cameraHeight = Camera.main.orthographicSize * 2;
        var cameraWidth = cameraHeight * Camera.main.aspect;

        Vector2 spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;

        var cameraAspectRatio = cameraWidth / cameraHeight;
        var spriteAspectRatio = spriteSize.x / spriteSize.y;

        float scaleFactor;

        if (cameraAspectRatio > spriteAspectRatio)
            scaleFactor = cameraWidth / spriteSize.x;
        else
            scaleFactor = cameraHeight / spriteSize.y;

        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
}