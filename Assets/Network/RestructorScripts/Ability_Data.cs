using UnityEngine;

public class Ability_Data : ScriptableObject
{
    public int AbilityTypeIndex { get; set; } = -1;

    [SerializeField] public int maxUses;

    public int MaxUses => maxUses;




    [SerializeField] float rechargeTime;

    public float RechargeTime => rechargeTime;
}
