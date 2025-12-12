using System;
using UnityEngine;

public class Teleporter_Door : MonoBehaviour
{
	public LayerMask teleporterLayerMask;
	public Action<GameObject> OnTeleporterDoorEntered;
	public Action<GameObject> OnTeleporterDoorExited;
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Trigger Entered");
		if (((1 << other.gameObject.layer) & teleporterLayerMask) == 0)
		{
			// debug layer
			Debug.Log("Layer Mismatch: " + LayerMask.LayerToName(other.gameObject.layer));
			return;
		}
		Debug.Log("Layer match: " + LayerMask.LayerToName(other.gameObject.layer));
		OnTeleporterDoorEntered?.Invoke(other.gameObject);

	}

	private void OnTriggerExit(Collider other)
	{
		if (((1 << other.gameObject.layer) & teleporterLayerMask) == 0)
		{
			
			return;
		}
		OnTeleporterDoorExited?.Invoke(other.gameObject);
	}
}
