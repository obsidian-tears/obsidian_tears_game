using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class UIButtonScrollList : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;
    [SerializeField]
    private List<GameObject> buttons;

    /* 
    void Start()
    {
        string folderPath = "Assets/Prefabs";
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in guids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
            string standardizedButtonName = StandardizeButtonName(prefabName);
            Debug.Log("Prefab Name: " + prefabName);

            // Create button for each prefab
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            // Set button name
            button.GetComponent<UIListButton>().SetText(standardizedButtonName);

            //Set button image
            SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {

                        Sprite prefabImage = prefab.GetComponent<SpriteRenderer>().sprite;
                        button.GetComponent<UIListButton>().SetImage(prefabImage);
            }
            else{
                Debug.LogError("SpriteRenderer is not attached to the game object. Please add it before using the script.");
            }


            button.transform.SetParent(buttonTemplate.transform.parent, false);
            buttons.Add(button);

            // Update button name in the Unity Editor
            button.name = standardizedButtonName;
            UnityEditor.EditorUtility.SetDirty(button);
        }
    }

    public void ButtonClicked(string myTextString)
    {
        Debug.Log(myTextString);
    }

    private string StandardizeButtonName(string name)
    {
        // Remove '_' characters
        string withoutUnderscores = name.Replace("_", " ");

        // Convert to Sentence case
        string sentenceCase = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(withoutUnderscores.ToLower());

        return sentenceCase;
    } 
    */
}
