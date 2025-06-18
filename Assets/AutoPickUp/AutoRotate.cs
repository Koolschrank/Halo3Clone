using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 10f; // Speed of rotation in degrees per second
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // rotate Y axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);


    }
}
