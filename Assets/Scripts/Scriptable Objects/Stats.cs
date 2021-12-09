using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusCondition {None, Asleep, Paralyzed, Slow}
public class Effect {

}

[CreateAssetMenu(fileName = "New Stats", menuName = "Player/Stats")]
public class Stats : ScriptableObject
{
    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;
    public PlayerType playerType = PlayerType.Fighter;
    public IntValue level;
    public FloatValue strength;
    public FloatValue agility;
    public FloatValue magicPower;
    public IntValue gold;
    public IntValue xp;
    public FloatValue maxMagic;
    public FloatValue maxHealth;
    public StatusCondition condition;
    public Effect effect;
}
