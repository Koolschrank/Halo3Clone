using UnityEngine;

public class AutoPickUp : MonoBehaviour
{
    public virtual void PickUp(GameObject player)
    {
       

        Destroy(gameObject);
    }
}
