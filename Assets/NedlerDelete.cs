using UnityEngine;

public class NedlerDelete : MonoBehaviour
{
    public float deletTime = 7f;
	float timer;
	CharacterHealth health;
	public GameObject removePartical;

	public LayerMask rigidBodyLayer;

	public void Activate(CharacterHealth health)
	{

		this.health = health;
		health.OnRemoveNedler += Remove;
		Activate();


		var bodyPart = health.RagdollTrigger.GetRandomBodyPart;

		


		Vector3 position = bodyPart.transform.position + transform.forward * -0.15f;
		transform.position = position;

		transform.SetParent(bodyPart.transform);

	}

	

	public void Activate()
	{
		enabled = true;
		timer = 0f;

	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= deletTime)
		{
			Remove();
		}

		if (health != null)
			transform.localPosition = Vector3.zero;

	}

	public void Remove()
	{
		if (health != null)
		{
			health.OnRemoveNedler -= Remove;
		}

		// spawn partical
		if (removePartical != null)
		{
			var partical = Instantiate(removePartical, transform.position, Quaternion.identity) as GameObject;
			partical.transform.SetParent(null);
		}
		GetComponent<Bullet>().DestroySelf();
	}
}
