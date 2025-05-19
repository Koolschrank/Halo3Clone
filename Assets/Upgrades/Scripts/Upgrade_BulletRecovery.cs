using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade_BulletRecovery", menuName = "Upgrades/Upgrade_BulletRecovery")]
public class Upgrade_BulletRecovery : Upgrade
{

    [SerializeField] float bulletRecoveryAmount = 0.5f;

    public override void Apply(GameObject body)
    {
        var leftArm = body.GetComponent<LeftArm>();
        var rightArm = body.GetComponent<RightArm>();

        if (leftArm != null)
        {
            leftArm.SetBulletRecoveryChance(bulletRecoveryAmount);
        }
        if (rightArm != null)
        {
            rightArm.SetBulletRecoveryChance(bulletRecoveryAmount);
        }

    }
}
