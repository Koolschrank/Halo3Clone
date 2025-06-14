using UnityEngine;
using System.Collections;

public class Interactable_GainAbility : Interactable
{
    [SerializeField] AbilityData abilityData;

    [SerializeField] Collider activationBox;
    [SerializeField] float activationCooldown = 1f;

    IEnumerator ActivationCooldown()
    {
        activationBox.enabled = false;
        yield return new WaitForSeconds(activationCooldown);
        activationBox.enabled = true;
    }

    public override bool CanUse(GameObject player)
    {
        if (!base.CanUse(player)) return false;

        var abilityInventory = player.GetComponent<AbilityInventory>();

        var abilityCount = abilityInventory.Abilities.Count;

        if (abilityCount >= abilityInventory.maxAbilities)
        {
            
            return false;
        }

        return true;
    }

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        var abilityInventory = player.GetComponent<AbilityInventory>();
        if (abilityInventory == null) return;
        abilityInventory.AddAbility(abilityData);
        StartCoroutine(ActivationCooldown());
    }
}
