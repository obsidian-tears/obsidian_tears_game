using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private bool gameMode;
    public GameObject phendrin;
    public Transform saveSpace;

    public void StartGame()
    {
        SpawnPhendrin(new Vector3(0, 0, 0));
    }

    private void SpawnPhendrin(Vector3 loc)
    {
        GameObject go = Instantiate(phendrin, loc, Quaternion.identity, saveSpace);
    }

}
