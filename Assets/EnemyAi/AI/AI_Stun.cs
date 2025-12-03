using UnityEngine;

public class AI_Stun : MonoBehaviour
{
    [SerializeField] CharacterHealth health;
    [SerializeField] PlayerAnimation AIanimation;
    [SerializeField] AI_Aim aim;
    [SerializeField] AI_Move movement;
    [SerializeField] AI_Shoot weapon;
    [SerializeField] GranadeThrower granadeThrower;

	[SerializeField] float stunDuration = 0.7f;
    float stunTimer = 0f;

    private void Start()
    {
        health.OnShildDepleted += Stun;
        health.OnMeleeHit += Stun;
        health.OnGranadeStick += Stun;
	}

    public void OnMeleeHit()
    {
        if ((health.ShildPercentage <=0 || health.MaxShild == 0) )
        {
            Stun();
		}
        
	}
	public bool IsStunned()
    {
        return stunTimer > 0;
    }

    private void Stun()
    {
        if (IsStunned()) return;
        weapon.DropLeftWeapon();
		aim.enabled = false;
        movement.enabled = false;
        weapon.enabled = false;
        AIanimation.Stun();
        stunTimer = stunDuration;
        granadeThrower.TryDropGranade();
	}

    private void Update()
    {
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {

                EndStun();
            }
        }
    }

    public void EndStun()
    {
        aim.enabled = true;
        movement.enabled = true;
        weapon.enabled = true;

    }
}
