using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);

        }
    }
}
