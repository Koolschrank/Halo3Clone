using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade_ReloadSpeed", menuName = "Upgrades/Reload Speed Upgrade")]
public class Upgrade_ReloadSpeed : Upgrade
{
    [SerializeField] float reloadSpeedMultiplier = 0.5f;

    public override void Apply(GameObject body)
    {
        var leftArm = body.GetComponent<LeftArm>();
        var rightArm = body.GetComponent<RightArm>();

        rightArm.SetReloadWeaponSpeedMultiplier(reloadSpeedMultiplier);
        leftArm.SetReloadWeaponSpeedMultiplier(reloadSpeedMultiplier);
    }
}
