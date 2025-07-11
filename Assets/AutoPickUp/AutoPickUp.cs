using System;
using UnityEngine;
using FMODUnity;

public class AutoPickUp : MonoBehaviour
{

	
	public EventReference pickUpSound;
    public Action OnPickUp;

	public virtual void PickUp(GameObject player)
    {
        OnPickUp?.Invoke();
		RuntimeManager.PlayOneShot(pickUpSound, transform.position);
		Destroy(gameObject);
	}
}
