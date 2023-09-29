using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIListButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI myText;
    [SerializeField]
    private Sprite myImage;
    private string myTextString;
    [SerializeField]
    private UIButtonScrollList buttonControl;
    public int generatedTileId;

    public void SetText(string textString)
    {
        myTextString = textString;
        myText.text = textString;
        
    }

public void SetImage(Sprite sprite)
{
    Image[] childImages = GetComponentsInChildren<Image>();
    foreach (Image childImage in childImages)
    {
        if (childImage.gameObject != gameObject)
        {
            childImage.sprite = sprite;
        }
    }

    myImage = sprite;
}

public void SetName(string buttonName){
    name = buttonName;
}
 private void Start()
    {
        //Set Tile ID Value
        GenerateUniqueTileId();
        Debug.Log("Generated Tile ID: " + generatedTileId);
    }
    private void GenerateUniqueTileId()
    {
        int min = 15;
        int max = 100;
        int range = max - min + 1;

        // Create an array of available tile IDs within the range
        int[] availableTileIds = new int[range];
        for (int i = 0; i < range; i++)
        {
            availableTileIds[i] = min + i;
        }

        // Shuffle the array to randomize the order
        for (int i = 0; i < range - 1; i++)
        {
            int randomIndex = Random.Range(i, range);
            int temp = availableTileIds[i];
            availableTileIds[i] = availableTileIds[randomIndex];
            availableTileIds[randomIndex] = temp;
        }

        // Assign the first available tile ID and remove it from the array
        generatedTileId = availableTileIds[0];
        availableTileIds = availableTileIds[1..];

         // Set Title ID
         ButtonTileMapper buttonTileMapper = GetComponent<ButtonTileMapper>();
        if (buttonTileMapper != null)
        {
            buttonTileMapper.tileId = generatedTileId;
            buttonTileMapper.tilemapLayer = 0;
            Debug.Log("titleId Assigned: " + generatedTileId);
        }

        // //Set Prefab ID if random
        // ButtonPrefabMapper buttonPrefabMapper = GetComponent<ButtonPrefabMapper>();
        // if (buttonPrefabMapper != null)
        // {
        //     buttonPrefabMapper.prefabIds[0] = generatedTileId;
        //     Debug.Log("titleId Assigned: " + generatedTileId);
        // }

    }

}


