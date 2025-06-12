using System;
using UnityEngine;

public class TargetHitCollector : MonoBehaviour
{
    public Action<GameObject> OnCharacterHit;
    public Action<GameObject> OnCharacterKill;
    public Action<GameObject> OnTbagStanceTriggered;
    [SerializeField] PlayerTeam playerTeam;

    [SerializeField] bool tBagOnKill = true;



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

        if (tBagOnKill && target.GetComponent<BodyMindConnection>().Mind != null)
        {

            
            OnTbagStanceTriggered?.Invoke(target.GetComponent<BodyMindConnection>().GetPlayerHead());

        }



    }
}
