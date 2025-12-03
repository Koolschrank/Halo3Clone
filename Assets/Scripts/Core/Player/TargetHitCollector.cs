using System;
using UnityEngine;

public class TargetHitCollector : MonoBehaviour
{
    public Action<GameObject> OnCharacterHit;
    public Action<GameObject> OnCharacterKill;
    public Action<GameObject> OnTbagStanceTriggered;
    [SerializeField] PlayerTeam playerTeam;

    [SerializeField] bool tBagOnKill = true;

    public Action<GameObject, GameObject> OnKill;

    public bool ignoreHit = false;



    public void CharacterHit(DamagePackage damage,GameObject target)
    {
        if (ignoreHit) return;


		if (damage.hasHitMarkerEffect && target.GetComponent<PlayerTeam>().TeamIndex != playerTeam.TeamIndex)
        {
            OnCharacterHit?.Invoke(target);
        }

        
    }

    public void CharacterKill(DamagePackage damage, GameObject target)
    {
        OnCharacterKill?.Invoke(target);

        OnKill?.Invoke(gameObject, target);

        if (target.GetComponent<BodyMindConnection>().Mind != null)
        {
            if (tBagOnKill)
            {
                OnTbagStanceTriggered?.Invoke(target.GetComponent<BodyMindConnection>().GetPlayerHead());
            }

            
        }

       



    }
}
