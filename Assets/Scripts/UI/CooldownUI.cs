using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] GameObject barObject;
    [SerializeField] Image[] cooldownBars;
    [SerializeField] GameObject selectedObject;

    [SerializeField] Image[] sprites;

    Color defaultColor;
    [SerializeField] Color filledColor;

    Ability ability;
    public void Setup(Ability ability)
    {
        this.ability = ability;
        ability.OnCooldownChanged += UpdateCooldown;
        ability.OnChargeGained += SetFilledColor;


        

        barObject.SetActive(true);
        foreach (Image sprite in sprites)
        {
            sprite.sprite = ability.abilityData.icon;
        }

        foreach (var bar in cooldownBars)
        {
            

            bar.fillAmount = 0;
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


    private void Awake()
    {
        foreach (var bar in cooldownBars)
        {
            defaultColor = bar.color;
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
            if (value == 1)
            {
                bar.color = filledColor;
            }
            else
            {
                bar.color = defaultColor;
            }
        }
    }

    public void SetFilledColor(int value)
    {
        foreach (var bar in cooldownBars)
            bar.color = filledColor;
    }

    public void SetSelected(bool selected)
    {
        selectedObject.SetActive(selected);
    }
}
