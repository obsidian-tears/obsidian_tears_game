using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemActivator : MonoBehaviour
{

    // --------------------------------------------------
    // Variables:

    [SerializeField]
    private int distanceFromPlayer = 50;

    private GameObject player;

    public List<ActivatorItem> activatorItems;

    // --------------------------------------------------

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        activatorItems = new List<ActivatorItem>();

        AddToList();

        StartCoroutine("CheckActivation");
    }

    void AddToList()
    {
        foreach(SpriteRenderer child in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            activatorItems.Add(new ActivatorItem { item = child.gameObject, itemPos = child.transform.position });
        }
        
    }

    IEnumerator CheckActivation()
    {
        List<ActivatorItem> removeList = new List<ActivatorItem>();

        foreach (ActivatorItem item in activatorItems)
        {
            if (item.item == null)
            {
                removeList.Add(item);
                continue;
            }

            item.item.SetActive(Vector3.Distance(player.transform.position, item.itemPos) < distanceFromPlayer);
        }


        yield return new WaitForSeconds(0.1f);

        foreach (ActivatorItem item in removeList)
        {
            activatorItems.Remove(item);
        }

        yield return new WaitForSeconds(0.1f);
        StartCoroutine("CheckActivation");
    }
}

public class ActivatorItem
{
    public GameObject item;
    public Vector3 itemPos;
}
