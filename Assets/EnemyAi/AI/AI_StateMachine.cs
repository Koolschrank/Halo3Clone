using System;
using UnityEngine;

public class AI_StateMachine : MonoBehaviour
{
    [SerializeField] CharacterHealth characterHealth;
    [SerializeField] AI_LookForPlayer lookForPlayer;

    public Action<AIState> OnStateChange;
    public Action<Vector3> OnTargetFound;


    [SerializeField] AIState currentState;

    public AIState CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState != value)
            {
                currentState = value;
                OnStateChange?.Invoke(currentState);
            }
        }
    }
    void Start()
    {
        CurrentState = AIState.Patrol;

        characterHealth.OnDamageTaken += OnDamageTaken;

        lookForPlayer.OnTargetDetected += OnTargetDetected;
        lookForPlayer.OnTargetLost += OnTargetLost;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDamageTaken(DamagePackage damage)
    {
        CurrentState = AIState.Chase;
        OnTargetFound?.Invoke(damage.owner.transform.position);
    }

    private void OnTargetDetected(Transform target)
    {
        CurrentState = AIState.Attack;
        OnTargetFound?.Invoke(target.position);

    }

    public void SetTarget(Vector3 target)
    {
        if (currentState == AIState.Chase || currentState == AIState.Attack) return;

        CurrentState = AIState.Chase;
        OnTargetFound?.Invoke(target);
    }

    private void OnTargetLost(Vector3 position)
    {
        CurrentState = AIState.Chase;
        OnTargetFound?.Invoke(position);
    }
}


public enum AIState
{
    Patrol,
    Chase,
    Attack,
    Flee
}
