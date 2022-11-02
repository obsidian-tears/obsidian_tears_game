using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour
{
    public Text spellNameText;
    public SpellObject spell;
    public void Setup(SpellObject newSpell) {
        spell = newSpell;
        spellNameText.text = newSpell.spellName;
    }
}