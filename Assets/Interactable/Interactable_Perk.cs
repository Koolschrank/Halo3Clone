using UnityEngine;
using System.Collections;

public class Interactable_Perk : Interactable
{
    [SerializeField] StatUpgrader perk;

    [SerializeField] Collider activationBox;
    [SerializeField] float activationCooldown = 1f;

    IEnumerator ActivationCooldown()
    {
        activationBox.enabled = false;
        yield return new WaitForSeconds(activationCooldown);
        activationBox.enabled = true;
    }

    protected override void Awake()
    {
        base.Awake();
        discription = "gain: " + perk.upgraderName;
        extraDiscription = perk.upgraderDescription;
    }

    public override bool CanUse(GameObject player)
    {
        if (!base.CanUse(gameObject)) return false;


        var statSheet = player.GetComponent<PlayerBodyStatSheet>();
        if (statSheet == null)
        {
            return false;
        }

        return !statSheet.playerStatsSheetInstance.HasModifier(perk);
    }

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        var body = player.GetComponent<BodyMindConnection>();
        var statSheet = body.GetComponent<PlayerBodyStatSheet>();
        statSheet.ApplyStatUpgrade(perk);
        
        StartCoroutine(ActivationCooldown());


    }
}
