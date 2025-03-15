using UnityEngine;

public class Arm_FPSView : MonoBehaviour
{
    [SerializeField] Arm playerArm;
    [SerializeField] Transform granadeSpawnPoint;
    Weapon_Visual weaponVisual;

    public void SetUp(Arm newArm)
    {
        if (playerArm != null)
        {
            playerArm.OnWeaponEquipStarted -= EquipWeapon;
            playerArm.OnWeaponUnequipFinished -= RemoveWeapon;
            playerArm.OnWeaponDroped -= RemoveWeapon;
            playerArm.OnGranadeThrowStarted -= ThrowGranadeStart;
            playerArm.OnGranadeThrow -= ThrowGranade;
        }



        playerArm = newArm;

        playerArm.OnWeaponEquipStarted += EquipWeapon;
        playerArm.OnWeaponUnequipFinished += RemoveWeapon;
        playerArm.OnWeaponDroped += RemoveWeapon;
        playerArm.OnGranadeThrowStarted += ThrowGranadeStart;
        playerArm.OnGranadeThrow += ThrowGranade;

        if (playerArm.CurrentWeapon != null)
        {
            EquipWeapon(playerArm.CurrentWeapon);
        }
    }


    public void EquipWeapon(Weapon_Arms weapon, float time)
    {
        EquipWeapon(weapon);
    }

    public void EquipWeapon(Weapon_Arms weapon)
    {
        if (weaponVisual != null)
        {
            Destroy(weaponVisual.gameObject);
        }

        weaponVisual = Instantiate(weapon.WeaponFPSModel, transform);
        weaponVisual.SetUp(weapon);
        UtilityFunctions.SetLayerRecursively(weaponVisual.gameObject, gameObject.layer);
    }

    public void RemoveWeapon(Weapon_Arms weapon_Arms)
    {
        if (weaponVisual == null) return;
        Destroy(weaponVisual.gameObject);
    }


    public void ThrowGranadeStart(GranadeStats stats)
    {
        if (weaponVisual == null) return;
        weaponVisual.ThrowGranadeStart(stats);
    }

    public void ThrowGranade(GameObject granade, GranadeStats stats)
    {
        var clone = Instantiate(stats.GranadeClonePrefab, granadeSpawnPoint.position, granade.transform.rotation);
        var granadeScript = granade.GetComponent<Granade>();
        granadeScript.AddGranadeCopy(clone.transform);

        UtilityFunctions.SetLayerRecursively(clone, gameObject.layer);


    }

    
}
