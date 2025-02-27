using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimationSignals : MonoBehaviour
{
    [SerializeField] HitBox hitBox;

    // unity event fot hitstun end
    public UnityEvent OnHitStunEnd;
    public UnityEvent OnAttackEnd;
    // enable hitbox
    public void EnableHitBox()
    {
        hitBox.EnableCollider();
    }

    // disable hitbox
    public void DisableHitBox()
    {
        hitBox.DisableCollider();
    }

    public void HitStunEnd()
    {
        OnHitStunEnd.Invoke();
    }

    public void AttackEnd()
    {
        OnAttackEnd.Invoke();
    }


}
