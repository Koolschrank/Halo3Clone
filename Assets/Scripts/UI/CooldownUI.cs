using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] bool replaceImage = true;
    [SerializeField] GameObject barObject;
    [SerializeField] Image[] cooldownBars;
    [SerializeField]
    Image[] cooldownColorBar;
    [SerializeField] GameObject selectedObject;

    [SerializeField] Image[] sprites;

	[SerializeField] Color defaultColor;
    [SerializeField] Color filledColor;

    [SerializeField] bool updateCharges = false;
    [SerializeField] TextMeshProUGUI chargeText;

	Ability ability;
    public void Setup(Ability ability)
    {
        if (this.ability != null)
        {
            this.ability.OnCooldownChanged -= UpdateCooldown;
            this.ability.OnChargeGained -= SetFilledColor;
            this.ability.OnChargeGained -= UpdateCharge;
            this.ability.OnChargeLost -= UpdateCharge;
		}

		this.ability = ability;
        ability.OnCooldownChanged += UpdateCooldown;
        ability.OnChargeGained += SetFilledColor;
        ability.OnChargeGained += UpdateCharge;
        ability.OnChargeLost += UpdateCharge;

        UpdateCharge(ability.charges);




		barObject.SetActive(true);
        if (replaceImage)
        {
			foreach (Image sprite in sprites)
			{

				sprite.sprite = ability.abilityData.icon;
			}
		}
        

        foreach (var bar in cooldownBars)
        {
            bar.fillAmount = 0;
        }
        foreach (var bar in cooldownColorBar)
        {
            bar.color = defaultColor;
		}

		UpdateCooldown(1);
    }


	public void UpdateCharge(int amont)
        {
        
		if (!updateCharges) return;
		
		if (chargeText != null)
        {
			Debug.Log("UpdateCharge: " + ability.charges.ToString());
			chargeText.text = ability.charges.ToString();

		}
	}

	public void OnDisable()
    {
        if (ability != null)
        {
            ability.OnCooldownChanged -= UpdateCooldown;
            ability.OnChargeGained -= SetFilledColor;
        }
    }

    public void SetActive(bool active)
    {
        barObject.SetActive(active);
    }


    public void UpdateCooldown(float value)
    {
        if (ability.charges == 0)
        {
			foreach (var bar in cooldownBars)
			{
				bar.fillAmount = value;

			}
		}
        else
        {
			foreach (var bar in cooldownBars)
			{
				bar.fillAmount = 1;

			}
		}

        if (value == 1 || ability.charges > 0)
        {
            foreach (var bar in cooldownColorBar)
            {
                bar.color = filledColor;
            }
        }
        else
        {
            foreach (var bar in cooldownColorBar)
            {
                bar.color = defaultColor;
            }
        }
	}

    public void SetFilledColor(int value)
    {
        foreach (var bar in cooldownColorBar)
        {
            bar.color = filledColor;
		}
	}

    public void SetSelected(bool selected)
    {
        selectedObject.SetActive(selected);
    }
}
