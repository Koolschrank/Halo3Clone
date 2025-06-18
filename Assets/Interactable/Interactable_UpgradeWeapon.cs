using UnityEngine;
using System.Collections;

public class Interactable_UpgradeWeapon : Interactable
{

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


        var arms = player.GetComponent<PlayerArms>();

        var canRightWeaponUpgrade = arms.RightArm.CurrentWeapon != null && arms.RightArm.CurrentWeapon.Data.UpgradedWeaponData != null;
        var canLeftWeaponUpgrade = arms.LeftArm.CurrentWeapon != null && arms.LeftArm.CurrentWeapon.Data.UpgradedWeaponData != null;

        return canRightWeaponUpgrade || canLeftWeaponUpgrade;
    }


    public override void Interact(GameObject player)
    {
        base.Interact(player);

        var arms = player.GetComponent<PlayerArms>();
        var rightHand = arms.RightArm;
        var rightHandWeapon = rightHand.CurrentWeapon;

        if (rightHandWeapon != null && rightHandWeapon.Data.UpgradedWeaponData != null)
        {
            var upgradedWeapon = new Weapon_Arms(rightHandWeapon.Data.UpgradedWeaponData, 999);
            rightHand.ReplaceWeapon(upgradedWeapon);


        }
        else
        {
            var leftHand = arms.LeftArm;
            var leftHandWeapon = leftHand.CurrentWeapon;
            if (leftHandWeapon != null && leftHandWeapon.Data.UpgradedWeaponData != null)
            {
                var upgradedWeapon = new Weapon_Arms(leftHandWeapon.Data.UpgradedWeaponData, 999);
                leftHand.ReplaceWeapon(upgradedWeapon);
            }
        }

        StartCoroutine(ActivationCooldown());


    }
}
