using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: make this more generic as a script that takes a string (scene name) and sets a scenenamevariable and sends the signal to navigate.
public class MainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void OnButtonClick() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

