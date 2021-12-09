using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour
{
    public Text spellNameText;
    public Spell spell;
    public void Setup(Spell newSpell) {
        spell = newSpell;
        spellNameText.text = newSpell.spellName;
    }
}