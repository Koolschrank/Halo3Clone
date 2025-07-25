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

	bool isArmor = true;


	PlayerBuffs playerBuff;

	Color defaultColor;
	private void Awake()
	{
		defaultColor = armorBarColor.color;

	}


	public void ConnectBuffs(PlayerBuffs playerBuff)
	{
		if (this.playerBuff != null)
		{
			this.playerBuff.OnEnterBuff -= SetBuff;
			this.playerBuff.OnExitBuff -= CancelBuff;
			this.playerBuff.OnUpdateBuff -= UpdateValues;
		}

		this.playerBuff = playerBuff;
		playerBuff.OnEnterBuff += SetBuff;
		playerBuff.OnExitBuff += CancelBuff;
		playerBuff.OnUpdateBuff += UpdateValues;
	}

	public void SetBuff(Buff buff)
	{
		SetIsArmor(false);
		SetColor(buff.buffColor);
	}

	public void CancelBuff()
	{
		SetIsArmor(true);
		SetColor(defaultColor);
	}

	public void SetIsArmor(bool value)
	{
			isArmor = value;
		
	}

	public void SetColor(Color color)
	{
		armorBarColor.color = color;
	}


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
			if (isArmor)
			{
				armorEffect.TriggerEffect();
			}

			if (armorValue <= 0)
			{
				armorGainEffect.Stop(); // stop effect if shild is depleted
				armorEffect.Stop(); // stop effect if shild is depleted
			}

			if (isArmor)
			{
				playerCam.EnterArmorBloom(); // trigger bloom effect on shild change

				if (damageBarTimer <= 0f)
					armorBar_damage.fillAmount = lastArmorValue;

				damageBarTimer = damageBarTime;
			}
			else
			{
				armorBar_damage.fillAmount = 0f;
			}

			
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
