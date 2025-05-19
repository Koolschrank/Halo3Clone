using UnityEngine;


[CreateAssetMenu(fileName = "fashShild", menuName = "Upgrades/FastShild")]
public class Upgrade_FastShild : Upgrade
{
    [SerializeField] float timeReduction = 1f;

    public override void Apply(GameObject body)
    {
        var health = body.GetComponent<CharacterHealth>();
        health.ReduceShildRegenTime(timeReduction);
    }
}
