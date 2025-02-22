using UnityEngine;

public class FlyForward : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

}
