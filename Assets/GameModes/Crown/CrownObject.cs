using System;
using Unity.VisualScripting;
using UnityEngine;

public class CrownObject : MonoBehaviour
{
    public Action<GameObject> OnCollected;


    private void OnTriggerEnter(Collider other)
    {
        // if has player team component
        if (other.GetComponent<PlayerTeam>() != null)
        {
            OnCollected?.Invoke(other.gameObject);
        }
    }
}
