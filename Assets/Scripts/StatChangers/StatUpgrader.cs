using UnityEngine;


// create asset menu
[CreateAssetMenu(fileName = "StatUpgrader", menuName = "ScriptableObjects/StatUpgrader", order = 1)]
public class StatUpgrader : ScriptableObject
{
    public string upgraderName;
    public string upgraderDescription;
    public StatModifier[] statModifiers;
    public PassiveModifier[] passiveModifiers;
    

}
