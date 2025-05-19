using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Ability", menuName = "Upgrades/Upgrade_Ability")]
public class Upgrade_Ability : Upgrade
{
    [SerializeField] AbilityData abilityData;

    public override void Apply(GameObject body)
    {
        body.GetComponent<AbilityInventory>().AddAbility(abilityData);
    }
}
