using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_FireRateUp", menuName = "Upgrades/Fire Rate Up")]
public class Upgrade_FireRateUp : Upgrade
{
    [SerializeField] private float fireRateIncrease = 0.3f; // Amount to increase fire rate by

    public override void Apply(GameObject body)
    {
        var leftArm = body.GetComponent<LeftArm>();
        var rightArm = body.GetComponent<RightArm>();


        leftArm.AddToFireRateMultiplier(fireRateIncrease);
        rightArm.AddToFireRateMultiplier(fireRateIncrease);

    }
}
