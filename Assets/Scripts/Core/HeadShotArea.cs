using UnityEngine;

public class HeadShotArea : MonoBehaviour
{
    [SerializeField] float radius = 1f;

    public bool IsHeadShot(Vector3 position)
    {
        // check if position y is in the headshot area
        var lowerBound = transform.position.y - radius;
        //var upperBound = transform.position.y + radius;
        if (position.y >= lowerBound) //&& position.y <= upperBound)
            return true;


        return Vector3.Distance(transform.position, position) < radius;
    }


    // gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
