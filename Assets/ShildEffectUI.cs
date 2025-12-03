
using UnityEngine;
using UnityEngine.UI;

public class ShildEffectUI : MonoBehaviour
{
	[SerializeField] float time;
	[SerializeField] float alphaPower = 1f;
	[SerializeField] AnimationCurve alphaCurve;
	[SerializeField] Image image;
	
	float timer = 0f;

	public bool noUpdate = false;

	public void Stop()
	{
		
		timer = 0f;
		UpdateAlphaValue(0f);

	}

	public void UpdateAlphaValue(float alpha)
	{
		var color = image.color;
		color.a = alpha;
		image.color = color;
	}

	public void TriggerEffect()
	{
		timer = time;
	}

	private void Update()
	{
		if (noUpdate) return;
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			var alpha = alphaCurve.Evaluate(1f - (timer / time)) * alphaPower;
			UpdateAlphaValue(alpha);
		}
		else
		{
			UpdateAlphaValue(0f);

		}
	}
}
