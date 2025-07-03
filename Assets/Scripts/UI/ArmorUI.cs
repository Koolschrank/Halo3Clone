using UnityEngine;

using UnityEngine.UI;

public class ArmorUI : MonoBehaviour
{
	[SerializeField]
	ShildEffectUI armorEffect;
	[SerializeField]
	ShildEffectUI armorGainEffect;

	[SerializeField] PlayerCamera playerCam;
	[SerializeField] CharacterHealth health;
	[SerializeField] Image armorBar;
	[SerializeField] Image armorBar_damage;

	[SerializeField] Image armorBar_damageColor;
	[SerializeField] Image armorBarColor;

	[SerializeField] float damageBarTime = 0.2f;
	[SerializeField] AnimationCurve damageBarVisibilityCurve;
	float damageBarTimer = 0f;

	public void SetUp(CharacterHealth health)
	{
		if (this.health != null)
		{
			this.health = health;
		}
		this.health = health;
		health.OnArmorChanged += UpdateValues;



		UpdateValues(health.ArmorValue); // update values on start

	}

	float lastArmorValue = 0f;
	public void UpdateValues(float armorValue)
	{
		if (lastArmorValue > armorValue)
		{
			armorEffect.TriggerEffect();

			if (armorValue <= 0)
			{
				armorGainEffect.Stop(); // stop effect if shild is depleted
				armorEffect.Stop(); // stop effect if shild is depleted
			}
				


			playerCam.EnterArmorBloom(); // trigger bloom effect on shild change

			if (damageBarTimer <= 0f)
				armorBar_damage.fillAmount = lastArmorValue;

			damageBarTimer = damageBarTime;
		}
		else if (lastArmorValue < armorValue)
		{
			armorGainEffect.TriggerEffect();
		}


		
		armorBar.fillAmount = armorValue;
		lastArmorValue = armorValue;

		if (armorValue <=0)
		{
			gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (damageBarTimer > 0)
		{
			damageBarTimer -= Time.deltaTime;
			float visibility = damageBarVisibilityCurve.Evaluate(1f - (damageBarTimer / damageBarTime));

			var c = armorBar_damageColor.color;
			armorBar_damageColor.color = new Color(c.r, c.g, c.b, visibility);

		}
		else
		{
			armorBar_damageColor.color = new Color(armorBar_damageColor.color.r, armorBar_damageColor.color.g, armorBar_damageColor.color.b, 0f);
			armorBar_damage.fillAmount = 0f;
		}
	}
}
