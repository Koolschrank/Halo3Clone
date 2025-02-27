using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : MonoBehaviour
{
    
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;


    [SerializeField] float updateTime = 0.3f;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackRate = 1f;
    [SerializeField] bool isRunning = false;
    float lastAttackTime;

    float lastUpdateTime;
    Transform target;
    State state;




    void Update()
    {
        if (target == null)
        {
            target = GetNewTarget();
        }

        if ( IsCloseToTargetToAttack())
        {
            TryAttack();
        }

        if (Time.time - lastUpdateTime > updateTime)
        {
            animator.SetBool("Run", isRunning);

            agent.SetDestination(target.position);
            lastUpdateTime = Time.time;
        }
    }

    public Transform GetNewTarget()
    {
        // find the closest player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        foreach (var player in players)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }
        return closestPlayer;
    }



    // check if close to target
    public bool IsCloseToTargetToAttack()
    {
        if (target == null)
        {
            return false;
        }
        return Vector3.Distance(target.position, transform.position) < attackDistance;
    }

    public void TryAttack()
    {
        if (state != State.Attacking && Time.time - lastAttackTime > attackRate)
        {
            Attack();
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        // stop the agent
        agent.isStopped = true;
        lastAttackTime = Time.time;
        state = State.Attacking;
    }

    public void AttackEnd()
    {
        agent.isStopped = false;
        
        state = State.Idle;
    }

    public void HitStun()
    {
        animator.SetTrigger("Hit");
        agent.isStopped = true;
        state = State.HitStun;

    }

    public void EndHitStun()
    {
        agent.isStopped = false;
        state = State.Idle;
    }

    public void Death()
    {
        animator.SetBool("Dead", true);
        agent.isStopped = true;
        state = State.Dead;
    }


    public enum State
    {
        Idle,
        Moving,
        Attacking,
        HitStun,
        Dead
    }
}

