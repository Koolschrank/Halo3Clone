using System;
using UnityEngine;

public class TargetHitCollector : MonoBehaviour
{
    public Action<GameObject> OnCharacterHit;
    public Action<GameObject> OnCharacterKill;

    public void CharacterHit(GameObject target)
    {
        OnCharacterHit?.Invoke(target);
    }

    public void CharacterKill(GameObject target)
    {
        OnCharacterKill?.Invoke(target);
    }
}
