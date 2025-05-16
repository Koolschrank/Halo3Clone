using UnityEngine;

public class AI_Shoot : MonoBehaviour
{
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;


    

    float focuse = 0;

    

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
        
        if (focuse == 1)
        {
            playerArms.RightArm.UpdateWeaponTrigger(true);
        }
        else
        {
            if (focuse == 0)
            {
                playerArms.RightArm.PressReloadButton();
            }

            playerArms.RightArm.UpdateWeaponTrigger(false);
        }

    }
}
