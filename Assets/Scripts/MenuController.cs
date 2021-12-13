using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject questsPanel;
    [SerializeField] GameObject gameOptionsPanel;
    [SerializeField] int selectedMenuIndex;

    void Start()
    {
        selectedMenuIndex = 0;
        inventoryPanel.SetActive(true);
    }

    public void PageLeft()
    {
        int newIndex = selectedMenuIndex - 1;
        newIndex %= 3;
        SetPage(selectedMenuIndex, newIndex);
        selectedMenuIndex--;
        selectedMenuIndex %= 3;
    }

    public void PageRight()
    {
        int newIndex = selectedMenuIndex + 1;
        newIndex %= 3;
        SetPage(selectedMenuIndex, newIndex);
        selectedMenuIndex++;
        selectedMenuIndex %= 3;
    }

    void SetPage(int prevIndex, int newIndex)
    {
        if (prevIndex == 0)
        {
            inventoryPanel.SetActive(false);
        }
        else if (prevIndex == 1)
        {
            questsPanel.SetActive(false);
        }
        else
        {
            gameOptionsPanel.SetActive(false);
        }

        if (newIndex == 0)
        {
            inventoryPanel.SetActive(true);
        }
        else if (newIndex == 1)
        {
            questsPanel.SetActive(true);
        }
        else
        {
            gameOptionsPanel.SetActive(true);
        }
    }
}
