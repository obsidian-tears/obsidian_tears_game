using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InkDialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Color selectedColor;
    public int lettersPerSecond;

    /*
    1. create story. 
    2. create callback to start story
    3. type text for every response
    4. loop through responses and instantiate button prefab for each
    5. after selection destroy choices.
    */

    // Start is called before the first frame update
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
