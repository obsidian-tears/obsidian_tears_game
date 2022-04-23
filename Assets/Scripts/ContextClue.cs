using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: delete this script. this functionality is built in to unity inspector
public class ContextClue : MonoBehaviour
{
    [SerializeField] GameObject clue;
    public void Enable() {
        clue.SetActive(true);
    }

    public void Disable() {
        clue.SetActive(false);
    }
}
