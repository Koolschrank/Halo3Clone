using System;
using UnityEngine;

public class AutoPickUp : MonoBehaviour
{
    public Action OnPickUp;

	public virtual void PickUp(GameObject player)
    {
        OnPickUp?.Invoke();
		Destroy(gameObject);
	}
}
