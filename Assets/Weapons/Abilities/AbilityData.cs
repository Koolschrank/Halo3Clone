using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public Sprite icon;
    public float cooldownTime;
    public int maxCharges;
}
