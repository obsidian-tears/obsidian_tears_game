using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpellBook", menuName = "Player/Spellbook")]
public class PlayerSpells : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    public List<Spell> mySpells = new List<Spell>();
}
