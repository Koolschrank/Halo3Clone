using System;
using UnityEngine;

[Serializable]
public class StatModifier
{
    public StatType type;
    public float value;
}

[Serializable]
public class PassiveModifier
{
    public PassiveEffectType effectType;
    public bool isActive;
}

