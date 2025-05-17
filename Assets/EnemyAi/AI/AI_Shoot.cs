using UnityEngine;

public class AI_Shoot : MonoBehaviour
{
    [SerializeField] AI_Target target;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;

    [SerializeField] Vector2 distanceToThrowGranade = new Vector2(8, 12);




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

            var distanceToTarget = Vector3.Distance(transform.position, target.GetTargetPosition());
            if (distanceToTarget > distanceToThrowGranade.x && distanceToTarget < distanceToThrowGranade.y)
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

    }

    private void OnDisable()
    {
        playerArms.RightArm.UpdateWeaponTrigger(false);
    }
}
