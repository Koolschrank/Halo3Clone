using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Dualwield", menuName = "Upgrades/Upgrade_Dualwield")]
public class Upgrade_Dualwield : Upgrade
{
    [SerializeField] bool canDualWield = false;
    [SerializeField] bool canDualWield2HandedWeapons = false;

    public override void Apply(GameObject body)
    {
        PlayerArms playerArms = body.GetComponent<PlayerArms>();
        if (playerArms != null)
        {
            playerArms.SetCanDualWield2HandedWeapons(canDualWield2HandedWeapons);
            playerArms.SetCanDualWield(canDualWield);
        }
    }
}
