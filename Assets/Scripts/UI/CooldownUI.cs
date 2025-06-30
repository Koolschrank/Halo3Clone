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

    Ability ability;
    public void Setup(Ability ability)
    {
        if (this.ability != null)
        {
            this.ability.OnCooldownChanged -= UpdateCooldown;
            this.ability.OnChargeGained -= SetFilledColor;
		}

		this.ability = ability;
        ability.OnCooldownChanged += UpdateCooldown;
        ability.OnChargeGained += SetFilledColor;


        

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
        foreach (var bar in cooldownBars)
        {
            bar.fillAmount = value;
           
        }
		if (value == 1)
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
