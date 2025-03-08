using System;
using UnityEngine;

public class TargetHitCollector : MonoBehaviour
{
    public Action<GameObject> OnCharacterHit;
    public Action<GameObject> OnCharacterKill;
    [SerializeField] PlayerTeam playerTeam;

    public void CharacterHit(DamagePackage damage,GameObject target)
    {
        if (damage.hasHitMarkerEffect && target.GetComponent<PlayerTeam>().TeamIndex != playerTeam.TeamIndex)
        {
            OnCharacterHit?.Invoke(target);
        }

        
    }

    public void CharacterKill(DamagePackage damage, GameObject target)
    {
        OnCharacterKill?.Invoke(target);
    }
}
