using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{


    public Teleporter_Door teleporter_Door;
    public Transform targetLocation;

    static List<GameObject> teleportedObjects = new List<GameObject>();
    public float cooldownTime = 0.5f;


    Vector3 teleportOffset;
    bool changedRotation = false;
	Quaternion teleportRotationOffset;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        teleportOffset = targetLocation.position - teleporter_Door.transform.position;
        if (targetLocation.rotation != teleporter_Door.transform.rotation)
            changedRotation = true;
		teleportRotationOffset = targetLocation.rotation * Quaternion.Inverse(teleporter_Door.transform.rotation);
		teleporter_Door.OnTeleporterDoorEntered += TeleportObject;
	}

    private void TeleportObject(GameObject obj)
    {
        if (!teleportedObjects.Contains(obj))
        {
			// if has CharacterController
            CharacterController characterController = obj.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
			}

			obj.transform.position += teleportOffset;
            teleportedObjects.Add(obj);
            Invoke(nameof(RemoveFromTeleportedList), cooldownTime);


            if (changedRotation)
            {
                obj.transform.rotation = teleportRotationOffset * obj.transform.rotation;
                if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearVelocity = teleportRotationOffset * rb.linearVelocity;
                }
                if (obj.TryGetComponent<PlayerMovement>(out PlayerMovement cc))
                {

                    cc.moveVelocity = teleportRotationOffset * cc.moveVelocity;

                }
            }

				if (characterController != null)
            {
                characterController.enabled = true;
            }
		}
	}

    private void RemoveFromTeleportedList()
    {
        if (teleportedObjects.Count > 0)
        {
            teleportedObjects.RemoveAt(0);
        }
	}


	// gizmos to show teleport target
    private void OnDrawGizmos()
    {
        if (teleporter_Door != null && targetLocation != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(teleporter_Door.transform.position, targetLocation.position);
            Gizmos.DrawSphere(targetLocation.position, 0.2f);
        }
	}


}
