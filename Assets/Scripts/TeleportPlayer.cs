using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public Transform target;
    public void Teleport()
    {
        var player = GameObject.FindWithTag("Player");
        player.transform.position = target.transform.position;
    }
}
