using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;


    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
