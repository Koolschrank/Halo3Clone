using UnityEngine;

public class AI_SendInformationToAllies : MonoBehaviour
{
    [SerializeField] AI_StateMachine stateMachine;
    [SerializeField] float radius = 10f;
    [SerializeField] LayerMask allyLayer;

    public void Awake()
    {
        stateMachine.OnTargetFound += SendInformationToAllies;

    }

    private void SendInformationToAllies(Vector3 target)
    {
        Collider[] allies = Physics.OverlapSphere(transform.position, radius, allyLayer);
        foreach (Collider ally in allies)
        {
            if (ally.tag != "AIEnemy") continue;

            AI_StateMachine allyStateMachine = ally.GetComponentInChildren<AI_StateMachine>();
            if (allyStateMachine != null)
            {
                allyStateMachine.SetTarget(target);
            }
        }
    }
}
