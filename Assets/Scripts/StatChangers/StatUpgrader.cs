using UnityEngine;


// create asset menu
[CreateAssetMenu(fileName = "StatUpgrader", menuName = "ScriptableObjects/StatUpgrader", order = 1)]
public class StatUpgrader : ScriptableObject
{
    public string upgraderName;
    public string upgraderDescription;
    public StatModifier[] statModifiers;
    public PassiveModifier[] passiveModifiers;


    public void ApplyModifiers(PlayerStatsSheet player)
    {
        foreach (StatModifier stat in statModifiers)
        {
            player.AddStat(stat.type, stat.value);
        }
        foreach (PassiveModifier passive in passiveModifiers)
        {
            player.SetPassiveEffect(passive.effectType, passive.isActive);
        }

    }
}
