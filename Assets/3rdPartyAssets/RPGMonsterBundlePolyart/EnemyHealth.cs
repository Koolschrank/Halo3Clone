using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : Health
{
    [SerializeField] Collider hurtBox;
    [SerializeField] float damageForHitStun = 25f;


    // unity event for hit stun
    public UnityEvent OnHitStun;

    public UnityEvent OnDie;
    public UnityEvent OnHit;


    public void TakeDamage(float damage)
    {
        return;
        OnHit?.Invoke();
        //base.TakeDamage(damage);
        if (CurrentHeath > 0 && damage >= damageForHitStun)
        {
            OnHitStun?.Invoke();

        }
    }

    protected override void Die()
    {
        OnDie?.Invoke();
        Destroy(gameObject, 5f);
        hurtBox.enabled = false;
    }

}
