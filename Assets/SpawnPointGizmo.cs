using UnityEngine;

public class SpawnPointGizmo : MonoBehaviour
{
	// sphere gizmor and forward direction line
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
	}
}
