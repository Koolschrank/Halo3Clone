using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade_Magazin", menuName = "Upgrades/Upgrade Magazin")]
public class Upgrade_Magazin : Upgrade
{
    [SerializeField] int extraBullets = 3;


    public override void Apply(GameObject body)
    {

        var leftArm = body.GetComponent<LeftArm>();
        var rightArm = body.GetComponent<RightArm>();

        leftArm.AddExtraBullets(extraBullets);
        rightArm.AddExtraBullets(extraBullets);

    }
}
