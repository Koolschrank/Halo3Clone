using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Fusion;

public class PlayerPickUpScan : NetworkBehaviour
{

    [Networked] public int indexOfClosestPickUp { get; private set; } = -1;

    public int IndexOfClosestPickUp
    {
        get => indexOfClosestPickUp;
        private set
        {
            if (value == indexOfClosestPickUp) return;

            indexOfClosestPickUp = value;
            if (HasStateAuthority)
                RPC_InvokeWeaponChangeEvent();
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_InvokeWeaponChangeEvent()
    {
        OnIndexOfClosesWeaponChanged?.Invoke(indexOfClosestPickUp);

    }


    [SerializeField]List<Weapon_PickUp> pickUpsInRange = new List<Weapon_PickUp>();
    [SerializeField] float pickUpCooldown = 0.5f;

    [Networked] public TickTimer pickUpCooldownTimer { get; set; }

    [SerializeField] WeaponInventory playerInventory;
    [SerializeField] PlayerTeam playerTeam;

    public Action<int> OnIndexOfClosesWeaponChanged;





    //public override void FixedUpdateNetwork()
    //{
    //    // todo: this is not a good bug fix, need to find the root cause
    //    TrySendUpdates();
    //}

    //public bool CanBeDualWielded(Weapon_PickUp pickUp)
    //{
    //    var weaponTypeOfWeaponInHand = playerArms.Weapon_RightHand.WeaponType;
    //    bool isDualWieldable = weaponTypeOfWeaponInHand == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons;
    //    var weaponTypeOfWeaponOnGround = pickUp.WeaponType;
    //    bool isDualWieldableOnGround = weaponTypeOfWeaponOnGround == WeaponType.oneHanded || playerArms.CanDualWield2HandedWeapons;
    //    return isDualWieldable && isDualWieldableOnGround;
    //}

    //public void TrySendUpdates()
    //{
    //    if (Time.time - lastPickUpTime > pickUpCooldown)
    //    {
    //        var closesWeapon = GetClosesPickUp();
    //        OnWeaponPickUpUpdate?.Invoke(closesWeapon);

    //        var weaponInRightHand = playerArms.Weapon_RightHand;
    //        if (weaponInRightHand != null && closesWeapon != null)
    //        {
    //            if (CanBeDualWielded(closesWeapon))
    //            {
    //                OnWeaponDualWieldUpdate?.Invoke(closesWeapon);
    //                return;
    //            }



    //        }

    //        OnWeaponDualWieldUpdate?.Invoke(null);

    //    }
    //}

    public override void FixedUpdateNetwork()
    {
        RestructurePickUps();
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickUp = other.GetComponent<Weapon_PickUp>();
        if (pickUp != null)
        {
            if (CheckIfPlayerOwnsWeapon(pickUp))
            {
                TransferAmmoFromWeaponOnGroundToPlayer(pickUp);

                if (pickUp != null )
                {
                    pickUpsInRange.Add(pickUp);
                }
            }
            else
            {
                if (!pickUp.CanBePickedUpByTeam(playerTeam.TeamIndex))
                    return;

                pickUpsInRange.Add(pickUp);
                
            }

            RestructurePickUps();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pickUp = other.GetComponent<Weapon_PickUp>();
        if (pickUp != null)
        {
            pickUpsInRange.Remove(pickUp);
            RestructurePickUps();
        }
    }

    public bool CheckIfPlayerOwnsWeapon(Weapon_PickUp pickup)
    {
        var weaponData = pickup.WeaponData.WeaponTypeIndex;

        return (weaponData == playerInventory.BackWeapon.weaponTypeIndex) ||
                (weaponData == playerInventory.LeftWeapon.weaponTypeIndex) ||
                (weaponData == playerInventory.RightWeapon.weaponTypeIndex);

    }

    public void TransferAmmoFromWeaponOnGroundToPlayer(Weapon_PickUp pickUp)
    {
        /*if (playerArms.HasWeapon(pickUp.WeaponData))
        {
            playerInventory.TransferAmmoFromPickUp(pickUp);


        }*/

        
    }

    public bool CanPickUpWeapon()
    {
        return pickUpCooldownTimer.ExpiredOrNotRunning(Runner) && IsWeaponInRange();
    }

    public bool IsWeaponInRange()
    {
        return pickUpsInRange.Count > 0;
    }

    public void RestructurePickUps()
    {
        RemovesNulls();
        SortPriority();
        if (pickUpsInRange.Count <= 0)
            IndexOfClosestPickUp = -1;
        else
            IndexOfClosestPickUp = pickUpsInRange[0].WeaponData.WeaponTypeIndex;
    }

    public WeaponNetworkStruct PickUpWeapon()
    {
        if (pickUpsInRange.Count <= 0)
            return new WeaponNetworkStruct()
            {
                weaponTypeIndex = -1,
            };

        var pickUp = pickUpsInRange[0];
        pickUpsInRange.RemoveAt(0);
        pickUpCooldownTimer = TickTimer.CreateFromSeconds(Runner, pickUpCooldown);
        RestructurePickUps();
        if (pickUp == null)
            return new WeaponNetworkStruct()
            {
                weaponTypeIndex = -1,
            };

        return pickUp.PickUp();
    }


    public void RemovesNulls()
    {
        pickUpsInRange.RemoveAll(x => x == null);
    }

    // sort based on which one is closest to the player, all 3 vectors
    public void SortPriority()
    {
        pickUpsInRange.Sort((x, y) => Vector3.Distance(x.transform.position, transform.position).CompareTo(Vector3.Distance(y.transform.position, transform.position)));
    }
}
