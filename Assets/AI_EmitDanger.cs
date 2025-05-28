using UnityEngine;

public class AI_EmitDanger : MonoBehaviour
{
    [SerializeField] GameObject dangerObject;
    [SerializeField] float minDamageForDanger = 80f;
    [SerializeField] CharacterHealth health;



    private void Start()
    {
        health.OnDamageTaken += EmitDanger;
    }

    private void EmitDanger(DamagePackage damage)
    {

        if (damage.damageAmount < minDamageForDanger)
            return;

        var dangerObject = Instantiate(this.dangerObject, transform.position, Quaternion.identity);
        Destroy(dangerObject, 1f); // Destroy the danger object after 2 seconds
    }

}
