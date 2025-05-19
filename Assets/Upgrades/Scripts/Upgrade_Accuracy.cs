using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Accuracy", menuName = "Upgrades/Upgrade_Accuracy")]
public class Upgrade_Accuracy : Upgrade
{
    [SerializeField] float accuracyMultiplier = 0.5f;
    public override void Apply(GameObject body)
    {
        var bulletSpawner = body.GetComponent<PlayerArms>().RightArm.GetBulletSpawner();
        bulletSpawner.AccuracyMultiplier =accuracyMultiplier;


    }
}
