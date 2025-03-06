using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] float aliveTime = 5f;

    // start
    public void Start()
    {
        Destroy(gameObject, aliveTime);
    }
}
