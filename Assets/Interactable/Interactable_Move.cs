using UnityEngine;

public class Interactable_Move : Interactable
{
    [SerializeField] Transform targetToMove;
    [SerializeField] float moveSpeed = 1f;
    Vector3 targetPosition;
    bool isMoving = false;

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        targetPosition = targetToMove.position;
        isMoving = true;
        isInteractable = false;

    }

    private void Update()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (Vector3.Distance(targetToMove.position, targetPosition) < 0.001f)
            {
                isMoving = false;
            }
        }

    }
}
