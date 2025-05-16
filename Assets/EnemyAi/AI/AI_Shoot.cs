using UnityEngine;

public class AI_Shoot : MonoBehaviour
{
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;


    [SerializeField] float focusGainWhenOnTarget = 2f;

    [SerializeField] float focusLossWhenNotOnTarget = 0.5f;

    float focuse = 0;

    

    private void Update()
    {
        if (playerAim.OnTarget)
        {
            focuse += Time.deltaTime * focusGainWhenOnTarget;
            if (focuse > 1)
            {
                focuse = 1;
            }
        }
        else
        {
            focuse -= Time.deltaTime * focusLossWhenNotOnTarget;
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
