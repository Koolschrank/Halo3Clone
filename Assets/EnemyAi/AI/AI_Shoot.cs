using System;
using UnityEngine;

public class AI_Shoot : MonoBehaviour
{
    [SerializeField] AI_Target target;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] AbilityInventory abilityInventory;

	[SerializeField] Vector2 distanceToThrowGranade = new Vector2(8, 12);

    [NonSerialized]
    public bool cannotShoot = false;


	float focuse = 0;

    [NonSerialized]
    public bool hasShild = false;
	private void Start()
	{
        hasShild = playerArms.LeftArm.CurrentWeapon != null;

	}

    public void DropLeftWeapon()
    {
        if (hasShild)
        {
            playerArms.LeftArm.DropWeapon();
            hasShild = false;
		}
    }

	private void Update()
    {
        

        if (playerAim.OnTarget)
        {
            focuse += Time.deltaTime * playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.focusGainWhenOnTarget;
            if (focuse > 1)
            {
                focuse = 1;
            }
        }
        else
        {
            focuse -= Time.deltaTime * playerArms.RightArm.GetWeaponInHand().Data.GunAiBehaviour.focusLossWhenNotOnTarget;
            if (focuse < 0)
            {
                focuse = 0;
            }
        }
        
        if (focuse == 1 && !cannotShoot)
        {

            playerArms.RightArm.ForceWeaponTriggerDown(); // bassically like trigger down but everying turns automatic

            var distanceToTarget = Vector3.Distance(transform.position, target.GetTargetPosition());
            if (abilityInventory.HasAbility() &&distanceToTarget > distanceToThrowGranade.x && distanceToTarget < distanceToThrowGranade.y )
            {
                playerArms.RightArm.TryThrowGranade();
            }
        }
        else
        {
            if (focuse == 0)
            {
                playerArms.RightArm.PressReloadButton();
            }

            playerArms.RightArm.UpdateWeaponTrigger(false);
        }
        if (hasShild)
        {
            if (focuse > 0)
            {
				playerArms.LeftArm.UpdateWeaponTrigger(true);
			}
            else
            {
				playerArms.LeftArm.UpdateWeaponTrigger(false);
			}
                
		}

    }

    private void OnDisable()
    {
        playerArms.RightArm.UpdateWeaponTrigger(false);
    }
}
