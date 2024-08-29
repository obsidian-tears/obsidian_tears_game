#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SpriteMerger : MonoBehaviour
{
    public List<Transform> mergeObjects;

    [Tooltip("Pixels Per Unit to use, should usually match for all sprites (check their import settings)")]
    public float pixelsPerUnit = 16.0f;
    public int edgePadding = 2;

    private GameObject lastMergedObject;
    private Texture2D lastMergedTexture;

    //TODO:    private void SortMergeObjectsByYAxis(){}

    public void Merge()
    {
        var spriteRenderers = new List<SpriteRenderer>();

        foreach (var child in mergeObjects)
            GetSpriteRenderers(child.gameObject, spriteRenderers);

        if (spriteRenderers.Count == 0)
        {
            Debug.LogWarning("No SpriteRenderers found in mergeObjects");
            return;
        }

        Vector2 minPixelPos, maxPixelPos;
        CalculateFinalTextureSize(spriteRenderers, out minPixelPos, out maxPixelPos);

        var finalTextureSize = Vector2Int.RoundToInt(maxPixelPos) - Vector2Int.RoundToInt(minPixelPos) + new Vector2Int(edgePadding * 2, edgePadding * 2);
        var pixelCenter = (minPixelPos + maxPixelPos) * 0.5f;
        var worldPos = pixelCenter / pixelsPerUnit;

        lastMergedTexture = new Texture2D(finalTextureSize.x, finalTextureSize.y, TextureFormat.RGBA32, false);
        lastMergedTexture.filterMode = FilterMode.Point;

        // Initialize the final texture with transparent pixels
        var finalPixels = new Color[finalTextureSize.x * finalTextureSize.y];
        for (int i = 0; i < finalPixels.Length; i++)
            finalPixels[i] = Color.clear;

        // Merge all sprites into the final texture
        for (int i = 0; i < spriteRenderers.Count; ++i)
        {
            var sprite = spriteRenderers[i].sprite;
            if (sprite == null)
                continue;

            var spriteRect = sprite.textureRect;
            var spritePixels = sprite.texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);
            var startPixelPosition = Vector2Int.RoundToInt((Vector2)spriteRenderers[i].transform.position * pixelsPerUnit - minPixelPos - sprite.pivot) + new Vector2Int(edgePadding, edgePadding);

            for (int y = 0; y < spriteRect.height; y++)
            {
                for (int x = 0; x < spriteRect.width; x++)
                {
                    var finalX = startPixelPosition.x + x;
                    var finalY = startPixelPosition.y + y;
                    var finalPixelIndex = finalX + finalY * finalTextureSize.x;
                    var pixelIndex = x + y * (int)spriteRect.width;
                    var color = spritePixels[pixelIndex];
                    var finalColor = finalPixels[finalPixelIndex];

                    var alpha = color.a + finalColor.a * (1f - color.a);
                    var blendedColor = Color.clear;

                    // Blend colors only if alpha is greater than 0 to avoid black pixels
                    if (alpha > 0f)
                    {
                        blendedColor = (color * color.a + finalColor * finalColor.a * (1f - color.a)) / alpha;
                        blendedColor.a = alpha;
                    }

                    finalPixels[finalPixelIndex] = blendedColor;
                }
            }
        }

        lastMergedTexture.SetPixels(finalPixels);
        lastMergedTexture.Apply();
        var mergedSprite = Sprite.Create(lastMergedTexture, new Rect(0, 0, finalTextureSize.x, finalTextureSize.y), new Vector2(0.5f, 0.5f), pixelsPerUnit);

        // Create a sprite from the final texture
        var name = gameObject.scene.name + "_MergedSprite_" + System.DateTime.UtcNow.ToString("yyyy-MM-dd-T-hh-mm-ss");
        lastMergedObject = new GameObject(name);
        var sceneSR = lastMergedObject.AddComponent<SpriteRenderer>();

        lastMergedObject.transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        sceneSR.sprite = mergedSprite;
        sceneSR.sortingLayerID = spriteRenderers[0].sortingLayerID;
        sceneSR.sortingOrder = spriteRenderers[0].sortingOrder;
        sceneSR.sharedMaterial = spriteRenderers[0].sharedMaterial;
        sceneSR.color = spriteRenderers[0].color;
    }

    private void CalculateFinalTextureSize(List<SpriteRenderer> spriteRenderers, out Vector2 minPixelPos, out Vector2 maxPixelPos)
    {
        minPixelPos = maxPixelPos = Vector2.zero;
        Vector2 currentPos, currentMinPos, currentMaxPos;
        var initialized = false;

        foreach (var sr in spriteRenderers)
        {
            currentPos = sr.transform.position * pixelsPerUnit;
            currentMinPos = currentPos - sr.sprite.pivot;
            currentMaxPos = currentMinPos + sr.sprite.textureRect.size;

            if (!initialized)
            {
                minPixelPos = currentMinPos;
                maxPixelPos = currentMaxPos;
                initialized = true;
            }
            else
            {
                if (currentMinPos.x < minPixelPos.x) minPixelPos.x = currentMinPos.x;
                if (currentMinPos.y < minPixelPos.y) minPixelPos.y = currentMinPos.y;
                if (currentMaxPos.x > maxPixelPos.x) maxPixelPos.x = currentMaxPos.x;
                if (currentMaxPos.y > maxPixelPos.y) maxPixelPos.y = currentMaxPos.y;
            }
        }
    }

    private void GetSpriteRenderers(GameObject obj, List<SpriteRenderer> spriteRenderers)
    {
        //Don't merge animated sprites
        if (obj.GetComponent<Animator>())
            return;

        var sr = obj.GetComponent<SpriteRenderer>();

        //Don't merge disabled sprites
        if (sr != null && sr.enabled && sr.gameObject.activeInHierarchy)
            spriteRenderers.Add(sr);

        foreach (Transform child in obj.transform)
            GetSpriteRenderers(child.gameObject, spriteRenderers);
    }

    public void ToggleMergeObjects(bool isVisible)
    {
        foreach (var obj in mergeObjects)
            obj.gameObject.SetActive(isVisible);
    }

    public void MakeSpritesReadable()
    {
        var spriteRenderers = new List<SpriteRenderer>();
        foreach (var child in mergeObjects)
            GetSpriteRenderers(child.gameObject, spriteRenderers);

        var importerSettings = new TextureImporterSettings();

        foreach (var sr in spriteRenderers)
        {
            var importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(sr.sprite.texture));
            importer.ReadTextureSettings(importerSettings);
            var needReadWriteSet = !importer.isReadable;
            var needMeshTypeSet = importerSettings.spriteMeshType != SpriteMeshType.FullRect;

            if (needMeshTypeSet)
            {
                importerSettings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(importerSettings);
            }

            if (needReadWriteSet)
                importer.isReadable = true;

            if (needReadWriteSet || needMeshTypeSet)
                importer.SaveAndReimport();
        }
    }

    public void MakeSpritesUnreadable()
    {
        var spriteRenderers = new List<SpriteRenderer>();
        foreach (var child in mergeObjects)
            GetSpriteRenderers(child.gameObject, spriteRenderers);

        var importerSettings = new TextureImporterSettings();

        foreach (var sr in spriteRenderers)
        {
            var importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(sr.sprite.texture));
            importer.ReadTextureSettings(importerSettings);
            var needReadWriteSet = importer.isReadable;
            var needMeshTypeSet = importerSettings.spriteMeshType == SpriteMeshType.FullRect;

            if (needMeshTypeSet)
            {
                importerSettings.spriteMeshType = SpriteMeshType.Tight;
                importer.SetTextureSettings(importerSettings);
            }

            if (needReadWriteSet)
                importer.isReadable = false;

            if (needReadWriteSet || needMeshTypeSet)
                importer.SaveAndReimport();
        }
    }

    public void SaveSpriteToFile()
    {
        if (!lastMergedObject)
        {
            Debug.LogWarning("No merged object found.");
            return;
        }

        var filename = lastMergedObject.name + ".png";
        var fullPath = EditorUtility.SaveFilePanel("Save texture as PNG", Application.dataPath, filename, "png");

        if (fullPath.Length == 0)
            return;

        var pngData = lastMergedTexture.EncodeToPNG();
        if (pngData != null)
            File.WriteAllBytes(fullPath, pngData);

        AssetDatabase.Refresh();

        //Set saved texture settings
        var relativePath = "Assets" + fullPath.Substring(Application.dataPath.Length);
        var importer = (TextureImporter)TextureImporter.GetAtPath(relativePath);
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.filterMode = FilterMode.Point;

        var importerSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(importerSettings);
        importerSettings.spriteGenerateFallbackPhysicsShape = false;
        importer.SetTextureSettings(importerSettings);
        importer.SaveAndReimport();

        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(relativePath));

        //Set saved texture as a sprite to the merged object's sprite renderer
        lastMergedObject.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
        EditorSceneManager.MarkAllScenesDirty();
    }
}
#endif