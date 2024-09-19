using TMPro;
using UnityEngine;

public class CantEquipItemNote : MonoBehaviour
{
    public static CantEquipItemNote Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject notePanel;
    public TextMeshProUGUI noteText;

    public void ShowNote(string characterClass)
    {
        noteText.text = $"Only {characterClass} NFT Heroes may equip this item";
        notePanel.SetActive(true);
    }
}