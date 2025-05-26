using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_HealthRegen", menuName = "Upgrades/Health Regen Upgrade")]
public class Upgrade_HealthRegen : Upgrade
{
    [SerializeField] float healthRegenIncrease = 5f;

    public override void Apply(GameObject body)
    {
        if (body.TryGetComponent(out CharacterHealth health))
        {
            health.IncreaseHealthRegen(healthRegenIncrease);
        }
        else
        {
            Debug.LogWarning("CharacterHealth component not found on the body.");
        }
    }
}
