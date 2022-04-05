using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
