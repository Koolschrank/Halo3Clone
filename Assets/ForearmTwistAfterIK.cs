using UnityEngine;

public class ForearmTwistAfterIK : MonoBehaviour
{
	public Transform hand;
	public Transform forearm;

	[Range(0f, 1f)]
	public float twistWeight = 0.5f;

	public enum TwistAxis
	{
		X,
		Y,
		Z
	}

	public TwistAxis twistAxis = TwistAxis.Z;

	void LateUpdate()
	{
		if (!hand || !forearm) return;

		// 1. Cache original hand world rotation (IK result)
		Quaternion originalHandWorldRotation = hand.rotation;

		// 2. Determine local axis
		Vector3 localAxis =
			twistAxis == TwistAxis.X ? Vector3.right :
			twistAxis == TwistAxis.Y ? Vector3.up :
									  Vector3.forward;

		// Convert to world axis
		Vector3 worldAxis = forearm.TransformDirection(localAxis);

		// 3. Calculate relative rotation from forearm to hand
		Quaternion relative = Quaternion.Inverse(forearm.rotation) * hand.rotation;

		// Extract angle
		relative.ToAngleAxis(out float angle, out Vector3 axis);

		// Make sure twist direction is consistent
		if (Vector3.Dot(axis, worldAxis) < 0f)
			angle = -angle;

		// 4. Apply weighted twist to forearm
		Quaternion twist = Quaternion.AngleAxis(angle * twistWeight, worldAxis);
		forearm.rotation = twist * forearm.rotation;

		// 5. Restore hand world rotation
		hand.rotation = originalHandWorldRotation;
	}
}
