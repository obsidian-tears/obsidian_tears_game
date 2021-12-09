using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBox : MonoBehaviour
{
    [SerializeField] GameObject spellSelector;
    [SerializeField] PlayerSpells playerSpells;
    [SerializeField] Color selectedColor;
    [SerializeField] GameObject spellSlotTemplate;
    [SerializeField] AttackController attackController;
    List<SpellSlot> spellSlots = new List<SpellSlot>();
    //change to new object

    void Start()
    {
        InstantiateSlots();
    }

    void InstantiateSlots()
    {
        if (playerSpells)
        {
            foreach (Spell spell in playerSpells.mySpells)
            {
                if (spell.canUseInBattle)
                {
                    SpellSlot newSlot = Instantiate(spellSlotTemplate, transform.position, transform.rotation)
                        .GetComponent<SpellSlot>();
                    newSlot.transform.SetParent(spellSelector.transform);
                    newSlot.Setup(spell);
                    spellSlots.Add(newSlot);
                }
            }
            if (spellSlots.Count > 0)
                spellSlots[0].spellNameText.color = selectedColor;
        }
    }

    public void UpdateSpellSelection(int indexCount)
    {
        if (spellSlots.Count == 0) return;
        int selectedIndex = indexCount % spellSlots.Count;

        for (int i = 0; i < spellSlots.Count; i++)
        {
            if (i == selectedIndex)
            {
                spellSlots[i].spellNameText.color = selectedColor;
            }
            else
            {
                spellSlots[i].spellNameText.color = Color.black;
            }
        }
    }
    public float CastSpell(int selectedSpellIndex)
    {
        // make attack result class that all attacks and spells return. specify misses, insufficient mp, etc.
        if (spellSlots.Count == 0) return -1f;
        int selectedIndex = selectedSpellIndex % spellSlots.Count;
        return attackController.Spell(spellSlots[selectedIndex].spell);
    }

    public void AddSpell(Spell newSpell) {
        playerSpells.mySpells.Add(newSpell);
    }

    public void EnableSpellSelector()
    {
        this.gameObject.SetActive(true);
    }
    public void DisableSpellSelector()
    {
        this.gameObject.SetActive(false);
    }
}
