using System;
using UnityEngine;

public class TargetHitCollector : MonoBehaviour
{
    public Action<GameObject> OnCharacterHit;
    public Action<GameObject> OnCharacterKill;

    public void CharacterHit(DamagePackage damage,GameObject target)
    {
        if (damage.hasHitMarkerEffect)
        {
            OnCharacterHit?.Invoke(target);
        }

        
    }

    public void CharacterKill(DamagePackage damage, GameObject target)
    {
        OnCharacterKill?.Invoke(target);
    }
}
