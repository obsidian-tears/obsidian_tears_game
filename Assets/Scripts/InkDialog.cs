using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class InkDialog : MonoBehaviour
{
    //TODO: see how we can integrate this with love/hate to make responses to chosen dialog paths
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] Button buttonPrefab;
    [SerializeField] Color selectedColor;
    public int lettersPerSecond;

    //  ink dialog stuff
    [SerializeField] GameObject answerBox; // need this to add answer buttons to dialog
    public Story story;
    public static event Action<Story> OnCreateStory;

    [SerializeField] private DialogAsset dialogAsset; // ink dialog tree 
    [SerializeField] private MySignal leaveDialogSignal;

    /*
    1. create story. 
    2. create callback to start story
    3. type text for every response
    4. loop through responses and instantiate button prefab for each
    5. after selection destroy choices.
    */


    public void StartStory()
    {
        story = new Story(dialogAsset.value.text);
        if (OnCreateStory != null) OnCreateStory(story); // does something. instantiates story?
        StartCoroutine(RefreshView());
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    IEnumerator RefreshView()
    {
        // Remove all the UI on screen
        RemoveChildren();
        // 
        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            Debug.Log(text);
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            yield return StartCoroutine(CreateContentView(text));
        }
        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                print(choice.text.Trim());
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate
                {
                    StartCoroutine(OnClickChoiceButton(choice));
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            Button choice = CreateChoiceView("Leave");
            choice.onClick.AddListener(delegate
            {
                RemoveChildren();
                dialogAsset.value = null;
                this.gameObject.SetActive(false);
                leaveDialogSignal.Raise();
                // TODO: leave dialog
            });
        }
    }

    // // When we click the choice button, tell the story to choose that choice!
    IEnumerator OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        yield return StartCoroutine(RefreshView());
        //TODO: store the choices and pass back to a function that updates server
    }

    // // Creates a textbox showing the the line of text
    IEnumerator CreateContentView(string text)
    {
        yield return StartCoroutine(TypeDialog(text));
    }

    // // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(answerBox.transform, false);
        // Gets the text from the button prefab
        Text choiceText = choice.GetComponentInChildren<Text>();
        choiceText.text = text;
        // Make the button expand to fit the text
        // HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        // layoutGroup.childForceExpandHeight = false;
        return choice;
    }

    // Destroys all the children and clears text
    void RemoveChildren()
    {
        SetDialog("");
        int childCount = answerBox.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(answerBox.transform.GetChild(i).gameObject);
        }
    }
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

