using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSaver : MonoBehaviour
{

    [SerializeField]
    public Transform saveSpace;

    public void SaveAll() {
        foreach (GameObject go in saveSpace) {
            Save(go);
        }
    }

    private void Save(GameObject go) {

    }
}
