using UnityEngine;

public class BallGizmo : MonoBehaviour
{
	// gizmo 
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, 0.5f);
	}



}
