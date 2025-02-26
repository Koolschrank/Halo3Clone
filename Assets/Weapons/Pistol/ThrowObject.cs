using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    [SerializeField] GameObject objectToThrow;
    [SerializeField] Transform throwPoint;
    [SerializeField] float throwForce = 10f;
    [SerializeField] Vector3 throwRotation;

    
    public void Throw()
    {
        GameObject thrownObject = Instantiate(objectToThrow, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        rb.AddTorque(throwRotation, ForceMode.Impulse);
    }
}
