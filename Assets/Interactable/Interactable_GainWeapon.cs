using System.Collections;
using UnityEngine;

public class Interactable_GainWeapon : Interactable
{
    public int priceAfterFirstUse = 600; // Price after the first use, if applicable
    public Transform weaponSpawnPoint;
    public float weaponDropForce = 5f;
    public Weapon_Data[] weaponsToGain;

    [SerializeField] Collider activationBox;
    [SerializeField] float activationCooldown = 1f;
    [SerializeField] bool destroyAfterInteraction = false;

    IEnumerator ActivationCooldown()
    {
        activationBox.enabled = false;
        yield return new WaitForSeconds(activationCooldown);
        activationBox.enabled = true;
    }

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        var weaponToGain = GetRandomWeapon();

        Weapon_PickUp weapon_PickUp = Instantiate(weaponToGain.WeaponPickUp, weaponSpawnPoint.position, weaponSpawnPoint.rotation);
        weapon_PickUp.SetAmmoInMagazin(9999);
        weapon_PickUp.SetAmmoInReserve(9999);
        weapon_PickUp.transform.SetParent(null); // Detach from parent
        var rb = weapon_PickUp.GetComponent<Rigidbody>();
        rb.AddForce(weaponSpawnPoint.forward * weaponDropForce, ForceMode.Impulse); // Apply force to drop the weapon

        if (!destroyAfterInteraction)
            StartCoroutine(ActivationCooldown());
        else
            Destroy(gameObject, 0.01f); // Destroy the interactable object after a short delay

        currentPrice = priceAfterFirstUse; 
    }

    

    private Weapon_Data GetRandomWeapon()
    {
        if (weaponsToGain.Length == 0) return null;
        int randomIndex = Random.Range(0, weaponsToGain.Length);
        return weaponsToGain[randomIndex];

    }
}
