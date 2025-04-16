using UnityEngine;

public class Arm_FPSView : InterfaceItem
{
    [SerializeField] bool leftArm = false;
    [SerializeField] Transform granadeSpawnPoint;
    Weapon_Visual weaponVisual;


    protected override void Subscribe(PlayerBody body)
    {
        var arm = GetArm(body);
        var granadeTrower = body.PlayerArms.GranadeThrower;

        arm.OnWeaponEquipStarted += EquipWeapon;
        arm.OnWeaponUnequipFinished += RemoveWeapon;
        arm.OnWeaponDroped += (weapon, pickUp) => RemoveWeapon(weapon);
        arm.OnGranadeThrowStarted += ThrowGranadeStart;
        granadeTrower.OnGranadeThrow += ThrowGranade;

        if (arm.CurrentWeapon != null)
        {
            EquipWeapon(arm.CurrentWeapon);
        }
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var arm = GetArm(body);

        var granadeTrower = body.PlayerArms.GranadeThrower;

        arm.OnWeaponEquipStarted -= EquipWeapon;
        arm.OnWeaponUnequipFinished -= RemoveWeapon;
        arm.OnWeaponDroped -= (weapon, pickUp) => RemoveWeapon(weapon);
        arm.OnGranadeThrowStarted -= ThrowGranadeStart;

        granadeTrower.OnGranadeThrow += ThrowGranade;
    }

    public Arm GetArm(PlayerBody body)
    {
        if (leftArm)
        {
            return body.PlayerArms.LeftArm;
        }
        else
        {
            return body.PlayerArms.RightArm;
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


    public void ThrowGranadeStart(GranadeStats stats, float time)
    {
        if (weaponVisual == null) return;
        weaponVisual.ThrowGranadeStart(stats, time);
    }

    public void ThrowGranade(GameObject granade, GranadeStats stats)
    {
        var clone = Instantiate(stats.GranadeClonePrefab, granadeSpawnPoint.position, granade.transform.rotation);
        var granadeScript = granade.GetComponent<Granade>();
        granadeScript.AddGranadeCopy(clone.transform);

        UtilityFunctions.SetLayerRecursively(clone, gameObject.layer);


    }

    
}
