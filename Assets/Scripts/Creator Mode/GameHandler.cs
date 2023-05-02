using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private bool gameMode;
    public GameObject phendrin;

    public void StartGame()
    {
        SpawnPhendrin(new Vector3(0, 0, 0));
        gameMode = true;
    }

    private void SpawnPhendrin(Vector3 loc)
    {
        GameObject go = Instantiate(phendrin, loc, Quaternion.identity);
    }

}
