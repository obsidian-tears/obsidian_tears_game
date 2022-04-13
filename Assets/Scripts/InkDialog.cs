using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class InkDialog : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] Color selectedColor;
    public int lettersPerSecond;

    //  ink dialog stuff
    [SerializeField] GameObject answerBox; // need this to add answer buttons to dialog
    public Story story;
    public static event Action<Story> OnCreateStory;
    [SerializeField] private TextAsset inkJSONAsset = null; // ink dialog tree

    /*
    1. create story. 
    2. create callback to start story
    3. type text for every response
    4. loop through responses and instantiate button prefab for each
    5. after selection destroy choices.
    */

    // [SerializeField] Button buttonPrefab;

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null) OnCreateStory(story); // does something. instantiates story?
        // RefreshView();
    }

    // // This is the main function called every time the story changes. It does a few things:
    // // Destroys all the old content and choices.
    // // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView()
    {
        // Remove all the UI on screen
        // RemoveChildren();
        // 
        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            // CreateContentView(text);
        }
        // 
        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                // Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                // button.onClick.AddListener(delegate
                // {
                // OnClickChoiceButton(choice);
                // });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            // Button choice = CreateChoiceView("Leave");
            // choice.onClick.AddListener(delegate
            // {
            // TODO: leave dialog
            // });
        }
    }

    // // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    // // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        StartCoroutine(TypeDialog(text));
    }

    // // Creates a button showing the choice text
    // Button CreateChoiceView(string text)
    // {
    // Creates the button from a prefab
    // Button choice = Instantiate(buttonPrefab) as Button;
    // choice.transform.SetParent(canvas.transform, false);
    // Gets the text from the button prefab
    // Text choiceText = choice.GetComponentInChildren<Text>();
    // choiceText.text = text;
    // Make the button expand to fit the text
    // HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
    // layoutGroup.childForceExpandHeight = false;
    // return choice;
    // }

    // // Destroys all the children of this gameobject (all the UI)
    // void RemoveChildren()
    // {
    // int childCount = dialogBox.transform.childCount;
    // for (int i = childCount - 1; i >= 0; --i)
    // {
    // GameObject.Destroy(dialogBox.transform.GetChild(i).gameObject);
    // }
    // childCount = actionSelector.transform.childCount;
    // for (int i = childCount - 1; i >= 0; --i)
    // {
    // GameObject.Destroy(actionSelector.transform.GetChild(i).gameObject);
    // }
    // }
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

