using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade_Health", menuName = "Upgrades/Health Upgrade")]
public class Upgrade_Health : Upgrade
{
    [SerializeField] float healthIncrease = 120f;

    public override void Apply(GameObject body)
    {
        if (body.TryGetComponent(out CharacterHealth health))
        {
            health.IncreaseMaxHealth(healthIncrease);
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on the body.");
        }
    }
}
