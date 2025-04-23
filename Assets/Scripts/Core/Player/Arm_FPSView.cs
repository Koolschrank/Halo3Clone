using UnityEngine;

public class Arm_FPSView : InterfaceItem
{
    [SerializeField] bool leftArm = false;
    [SerializeField] Transform granadeSpawnPoint;
    Weapon_Visual weaponVisual;


    protected override void Subscribe(PlayerBody body)
    {
        var arms = body.PlayerArms;
        var granadeTrower = body.GranadeThrower;

        if (!leftArm)
        {
            arms.OnRightWeaponEquiped += EquipWeapon;
            arms.OnRightWeaponRemoved += RemoveWeapon;
            if (arms.Weapon_RightHand != null)
            {
                EquipWeapon(arms.Weapon_RightHand);
            }
        }
        else
        {
            arms.OnLeftWeaponEquiped += EquipWeapon;
            arms.OnLeftWeaponRemoved += RemoveWeapon;
            if (arms.Weapon_LeftHand != null)
            {
                EquipWeapon(arms.Weapon_LeftHand);
            }

        }




        granadeTrower.OnGranadeThrowStart += ThrowGranadeStart;
        granadeTrower.OnGranadeThrow += ThrowGranade;

        
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var arms = body.PlayerArms;

        if (!leftArm)
        {
            arms.OnRightWeaponEquiped -= EquipWeapon;
            arms.OnRightWeaponRemoved -= RemoveWeapon;
            if (arms.Weapon_RightHand != null)
            {
                EquipWeapon(arms.Weapon_RightHand);
            }
        }
        else
        {
            arms.OnLeftWeaponEquiped -= EquipWeapon;
            arms.OnLeftWeaponRemoved -= RemoveWeapon;
        }


        var granadeTrower = body.GranadeThrower;
        granadeTrower.OnGranadeThrowStart -= ThrowGranadeStart;

        granadeTrower.OnGranadeThrow -= ThrowGranade;
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
