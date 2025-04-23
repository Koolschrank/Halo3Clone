using Fusion;
using System;
using UnityEngine;
using static PlayerArms;

public class Arm : NetworkBehaviour
{
    /*
    [Networked] public int WeaponIndex { get; private set; } = -1;
    int previousWeaponIndex = -1;

    [Networked] public int AmmoInMagazine { get; set; } = 0;

    



    [SerializeField] protected PlayerArms playerArms;
    [SerializeField] Transform dropPosition;

    bool isTriggerPressed;

    public float reloadTimer = 0;
    public float switchOutTimer = 0;
    public float switchInTimer = 0;
    public float meleeAttackTimer = 0;
    public float meleeAttackHitTimer = 0;

    protected Weapon_Arms weaponInHand;


    public bool IsTriggerPressed => isTriggerPressed;
    public bool InReload => reloadTimer > 0;
    public bool InSwitchOut => switchOutTimer > 0;
    public bool InSwitchIn => switchInTimer > 0;

    public bool InMeleeAttack => meleeAttackTimer > 0;


    public bool InIdle => !InReload && !InSwitchOut && !InSwitchIn && !InMeleeAttack;

    private void Start()
    {
        //granadeThrower.OnGranadeThrow += SendGranadeThrowSignal;
        //characterHealth.OnDeath += () => DropWeapon(0);
        playerArms.Inventory.OnAmmoChanged += TrySendEventToUpdateReserve;

    }

    public override void FixedUpdateNetwork()
    {
        if (previousWeaponIndex != WeaponIndex)
        {
            previousWeaponIndex = WeaponIndex;
            EquipWeapon(WeaponIndex);
        }
    }




    public void CancelTimers()
    {
        reloadTimer = 0;
        switchOutTimer = 0;
        switchInTimer = 0;
        meleeAttackTimer = 0;
        meleeAttackHitTimer = 0;
    }

    public void ArmUpdate()
    {
        if (weaponInHand != null)
        {
            weaponInHand.UpdateWeapon(Runner.DeltaTime);
        }


        if (InReload)
        {
            reloadTimer -= Runner.DeltaTime;
            if (reloadTimer <= 0)
            {
                ReloadFinished();
            }
        }
       
        if (InSwitchIn)
        {
            switchInTimer -= Runner.DeltaTime;
            if (switchInTimer <= 0)
            {
                SwitchInFinished();
                switchInTimer = 0;
            }
        }

        if (InMeleeAttack)
        {
            meleeAttackTimer -= Runner.DeltaTime;
            if (meleeAttackHitTimer > 0)
            {
                meleeAttackHitTimer -= Runner.DeltaTime;
                if (meleeAttackHitTimer <= 0)
                {
                    meleeAttackHitTimer = 0;
                    var meleeAttack = weaponInHand.MeleeAttack;
                    if (meleeAttack == null)
                    {
                        meleeAttack = playerArms.BasicMeleeAttack;
                    }
                    playerArms.MeleeAttacker.Attack(meleeAttack);
                }
            }
            


            if (meleeAttackTimer <= 0)
            {
                meleeAttackTimer = 0;
            }
        }
    }

    public void TriggerHeld()
    {
        if (!InIdle) return;

        bool wasTriggerPressed = isTriggerPressed;
        isTriggerPressed = true;

        bool hasShotThisTick = false;

        switch (weaponInHand.ShootType)
        {
            case ShootType.Single:
            

                if (!wasTriggerPressed)
                {
                    hasShotThisTick = weaponInHand.TryShoot();
                }
                break;
            case ShootType.Burst:
                hasShotThisTick = weaponInHand.TryBurstShoot();
                break;
            case ShootType.Auto:
                hasShotThisTick = weaponInHand.TryShoot();
                break;
            case ShootType.Melee:
                if (!wasTriggerPressed)
                {
                    TryMelee();
                }
                break;
        }

        if (hasShotThisTick)
        {
            Debug.Log("Shot");
            OnWeaponShot?.Invoke(weaponInHand);
        }
    }

    public void TriggerReleased()
    {
        isTriggerPressed = false;
        
    }

    public bool TryMelee()
    {
        if ( !InMeleeAttack && !InSwitchOut)
        {
            CancelTimers();
            var meleeAttack = weaponInHand.MeleeAttack;
            if (meleeAttack == null)
            {
                meleeAttack = playerArms.BasicMeleeAttack;
            }
            float timeMultiplier = 1;
            if (playerArms.IsDualWielding)
            {
                timeMultiplier = playerArms.MeleeAttackTimeMultiplierInDualWielding;
            }

            playerArms.MeleeAttacker.AttackStart(meleeAttack);
            meleeAttackTimer = meleeAttack.MeleeTime * timeMultiplier;
            meleeAttackHitTimer = meleeAttack.Delay * timeMultiplier;
            weaponInHand.MeleeStart(meleeAttackTimer);
            OnMeleeWithWeaponStarted?.Invoke(weaponInHand, meleeAttackTimer);

            return true;
        }
        return false;
    }

    public bool TryReload()
    {
        if (weaponInHand != null &&
            !weaponInHand.IsInShootCooldown() &&
            weaponInHand.CanReload() &&
            InIdle &&
            playerArms.Inventory.HasAmmoInReserve(weaponInHand.Data.WeaponTypeIndex))
        {
            reloadTimer = weaponInHand.ReloadTime;
            OnWeaponReloadStarted?.Invoke(weaponInHand, weaponInHand.ReloadTime);
            weaponInHand.ReloadStart(reloadTimer);
            return true;
        }
        return false;
    }

    void ReloadFinished()
    {
        reloadTimer = 0;
        if (weaponInHand != null)
        {
            int ammoNeeded = weaponInHand.Data.MagazineSize - weaponInHand.Magazine;
            int ammoAdded = playerArms.Inventory.TakeAmmoFromReserve(weaponInHand.Data.WeaponTypeIndex, ammoNeeded);
            weaponInHand.ReloadFinished(ammoAdded);
        }

    }

    public bool TryPickUpWeapon()
    {
        if (InSwitchOut) return false;
        

        var inventory = playerArms.Inventory;
        var pickUpScan = playerArms.PickUpScan;

        if (pickUpScan.CanPickUpWeapon(this))
        {

            var newWeapon = pickUpScan.PickUpWeapon();

            OnWeaponPickedUp?.Invoke(newWeapon);

            if (inventory.HasWeaponInInventory)
            {
                DropWeapon(playerArms.WeaponDropForce);
                EquipWeapon(newWeapon);
            }
            else
            {
                inventory.SetWeaponInInventory(newWeapon);
                playerArms.TrySwitchWeapon(this);
                playerArms.Inventory.SetAmmoInReserve(newWeapon.weaponTypeIndex, newWeapon.ammoInReserve);
            }
            return true;
        }
        return false;
    }

    public void EquipWeapon(int index)
    {
        if (index == -1)
        {
            if (weaponInHand != null)
            {
                OnWeaponDroped?.Invoke(weaponInHand, null);
                OnWeaponUnequipFinished?.Invoke(weaponInHand);
                weaponInHand.DropWeapon();
                weaponInHand = null;
            }

            
            return;
        }

        var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(index);
        var weapon = new Weapon_Arms(weaponData, AmmoInMagazine);


        if (weapon == null)
        {
            return;
        }

        if (weaponInHand != null)
        {
            OnWeaponUnequipFinished?.Invoke(weaponInHand);
            var weaponStruct = weaponInHand.GetWeaponNetworkStruct();
            //weaponStruct.ammoInReserve = playerArms.Inventory.TakeAllAmmo(weaponInHand.Data.WeaponIndex);
            playerArms.Inventory.SetWeaponInInventory(weaponStruct);
        }

        CancelTimers();
        weaponInHand = weapon;
        switchInTimer = weaponInHand.SwitchInTime;
        OnWeaponEquipStarted?.Invoke(weaponInHand, weaponInHand.SwitchInTime);

        weapon.SetBulletSpawner(playerArms.BulletSpawner);
        weaponInHand.SwitchInStart(switchInTimer);
        weapon.SetIsBeingDualWielded(playerArms.IsDualWielding);
    }

    public void EquipWeapon(WeaponNetworkStruct weapon)
    {
        WeaponIndex = weapon.weaponTypeIndex;
        AmmoInMagazine = weapon.ammoInMagazine;
        playerArms.Inventory.SetAmmoInReserve(weapon.weaponTypeIndex, weapon.ammoInReserve);
    }

    //public void EquipWeaponOld(Weapon_Arms weapon)
    //{
    //    WeaponNetworkStruct weaponNetworkStruct = new WeaponNetworkStruct()
    //    {
    //        weaponIndex = ItemIndexList.Instance.GetIndexViaWeapondData(weapon.Data),
    //        ammoInMagazine = weapon.Magazine,
    //        ammoInReserve = 0
    //    };



    //    //RPC_EquipWeapon(weaponNetworkStruct);
    //}

    /*
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EquipWeapon(WeaponNetworkStruct weaponNetworkStruct)
    {
        var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponNetworkStruct.weaponIndex);
        var weapon = new Weapon_Arms(weaponData, weaponNetworkStruct.ammoInMagazine);


        if (weapon == null)
        {
            return;
        }

        if (weaponInHand != null)
        {
            OnWeaponUnequipFinished?.Invoke(weaponInHand);
            playerArms.Inventory.AddWeapon(weaponInHand);
        }

        CancelTimers();
        weaponInHand = weapon;
        switchInTimer = weaponInHand.SwitchInTime;
        OnWeaponEquipStarted?.Invoke(weaponInHand, weaponInHand.SwitchInTime);

        weapon.SetBulletSpawner(playerArms.BulletSpawner);
        weaponInHand.SwitchInStart(switchInTimer);
        weapon.SetIsBeingDualWielded(playerArms.IsDualWielding);
    }
    */

    /*
    void SwitchInFinished()
    {
        switchInTimer = 0;
    }

    public void DropWeapon()
    {
        DropWeapon(0);
    }

    public virtual void DropWeapon(float dropForce)
    {

        if (weaponInHand == null) return;
        CancelTimers();
        
        var pickUp = LetGoOfWeapon();
        if (pickUp == null) return;

        pickUp.AddImpulse(dropPosition.forward, dropForce);

        
    }

    public void CancelReload()
    {
        reloadTimer = 0;
    }

    public void DeleteWeapon()
    {
        if (weaponInHand != null)
        {
            WeaponIndex = -1;
           
            
        }
    }

    public int AmmoOfWeaponInReserve
    {
        get
        {
            if (weaponInHand == null) return 0;
            return playerArms.Inventory.GetAmmoInReserve(weaponInHand.Data.WeaponTypeIndex);
        }
    }


    Weapon_PickUp LetGoOfWeapon()
    {
        if (weaponInHand == null) return null;

        var inventory = playerArms.Inventory;

        // if weapon is empty return null
        if (weaponInHand.Magazine == 0 && inventory.GetAmmoInReserve(weaponInHand.Data.WeaponTypeIndex) == 0)
        {
            return null;
        }

        var pickUpVersion = weaponInHand.PickUpVersion;
        var pickUp = Runner.Spawn(pickUpVersion, dropPosition.position, dropPosition.rotation);
        //var pickUp = Instantiate(pickUpVersion, dropPosition.position, dropPosition.rotation);


        if (playerArms.HasMultipleOfTheSameWeapon(weaponInHand.Data.WeaponTypeIndex))
        {
            pickUp.SetAmmo(weaponInHand.Magazine, 0);
        }
        else
        {
            pickUp.SetAmmo(weaponInHand.Magazine, inventory.TakeAllAmmo(weaponInHand.Data.WeaponTypeIndex));
        }
        OnWeaponDroped?.Invoke(weaponInHand, pickUp);
        weaponInHand.DropWeapon();
        weaponInHand = null;
        WeaponIndex = -1;
        return pickUp;
    }






    public Action<Weapon_Arms, float> OnWeaponEquipStarted;
    public Action<Weapon_Arms, float> OnWeaponUnequipStarted;
    public Action<Weapon_Arms, float> OnWeaponReloadStarted;
    public Action<Weapon_Arms, float> OnMeleeWithWeaponStarted;
    public Action<Weapon_Arms> OnWeaponShot;
    public Action<Weapon_Arms> OnWeaponUnequipFinished;
    public Action<Weapon_Arms, Weapon_PickUp> OnWeaponDroped;
    public Action<GranadeStats, float> OnGranadeThrowStarted;
    public Action<int> OnReserveAmmoChanged;

    public Action<WeaponNetworkStruct> OnWeaponPickedUp;















    public void TrySendEventToUpdateReserve(int weaponIndex, int ammo)
    {
        if (weaponInHand != null && weaponInHand.Data.WeaponTypeIndex == weaponIndex)
        {
            OnReserveAmmoChanged?.Invoke(ammo);
        }
    }

   
        


    // TODO: this function can be cleared up



    

    

    /*

    public virtual void TryThrowGranade()
    {
        if (armState != ArmState.Ready) return;
        if (inventory.HasGranades)
        {
            IfZoomedInExitZoom();
            var granade = inventory.GranadeStats;
            float timeMultiplier = 1;
            if (playerArms.IsDualWielding)
            {
                timeMultiplier = granadeThrowTimeMultiplierInDualWielding;
            }
            granadeThrower.ThrowGranadeStart(granade, timeMultiplier);
            inventory.UseGranade();
            armState = ArmState.InGranadeThrow;
            granadeThrowTimer = granade.ThrowTime * timeMultiplier;
            OnGranadeThrowStarted?.Invoke(granade, granadeThrowTimer);
        }
    }*/

    /*
    void SendGranadeThrowSignal(GameObject granade)
    {
        OnGranadeThrow?.Invoke(granade, inventory.GranadeStats);
    }*/


    /*
    public virtual void TryMeleeAttack()
    {
        if (armState != ArmState.Ready && armState != ArmState.Shooting && armState != ArmState.Reloading) return;
        IfZoomedInExitZoom();
        var meleeAttack = weaponInHand.MeleeAttack;
        if (meleeAttack == null)
        {
            meleeAttack = basicMeleeAttack;
        }
        armState = ArmState.InMeleeAttack;
        float timeMultiplier = 1;
        if (playerArms.IsDualWielding)
        {
            timeMultiplier = meleeAttackTimeMultiplierInDualWielding;
        }


        meleeAttackTimer = meleeAttack.MeleeTime * timeMultiplier;
        meleeAttacker.AttackStart(meleeAttack, timeMultiplier);
        weaponInHand.MeleeStart(meleeAttackTimer);
        OnMeleeWithWeaponStarted?.Invoke(weaponInHand, meleeAttackTimer);


    }*/


    /*

    public void SetWeaponToIfDualWielding(bool isDualWielding)
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.SetIsBeingDualWielded(isDualWielding);
        }
    }


    public Weapon_Arms GetWeaponInHand()
    {
        return weaponInHand;
    }

    public float GetWeaponInHandSwitchInTime()
    {
        if (weaponInHand == null) return 0;

        return weaponInHand.SwitchInTime;
    }


   

    public void UpdateWeaponTrigger(bool value)
    {
        isTriggerPressed = value;
    }

    public Weapon_Arms CurrentWeapon
    {
        get
        {
            return weaponInHand;
        }
    }

    */

}
