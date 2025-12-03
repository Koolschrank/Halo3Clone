using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    [SerializeField] GameObject objectToThrow;
    [SerializeField] Transform throwPoint;
    [SerializeField] float throwForce = 10f;
    [SerializeField] Vector3 throwRotation;
    [SerializeField] Transform baseRotation;

    [SerializeField] bool useScriptToCallTrow = false;
    [SerializeField] Weapon_Visual visual;
    [SerializeField] float delay = 0.05f;

	private void Start()
	{
		if (useScriptToCallTrow && visual != null)
        {
            visual.OnShootAction += ThrowStart;

            
        }
	}

	private void OnDestroy()
	{
		if (useScriptToCallTrow && visual != null)
		{
			visual.OnShootAction -= ThrowStart;


		}
	}

	public void ThrowStart()
    {
		StartCoroutine(ThrowWithDelay(delay));
	}

    IEnumerator ThrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject != null)
        {
            Throw();

		}
    }

	public void Throw()
    {
        if (objectToThrow == null || throwPoint == null)
        {
            return;
        }

        GameObject thrownObject = Instantiate(objectToThrow, throwPoint.position, baseRotation.rotation);
        Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        rb.AddTorque(throwRotation, ForceMode.Impulse);
    }
}
