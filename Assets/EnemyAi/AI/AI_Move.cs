using UnityEngine;
using UnityEngine.AI;

public class AI_Move : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 0.6f;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] AI_Target target;
    [SerializeField] NavMeshAgent agent;



    private void Update()
    {
        Vector3 targetPosition = target.GetTargetPosition();
        agent.SetDestination(targetPosition);



        Vector3 direction = agent.desiredVelocity.normalized;

        Debug.Log("AI Target Position: " + targetPosition);
        Debug.Log("AI Move Direction: " + direction);

        playerMovement.UpdateMoveInput(new Vector2(direction.x, direction.z) * speedMultiplier);

        agent.transform.localPosition = Vector3.zero;
    }

}
