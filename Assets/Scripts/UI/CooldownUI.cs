using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : InterfaceItem
{
    [SerializeField] GameObject barObject;
    [SerializeField] Image[] cooldownBars;

    Color defaultColor;
    [SerializeField] Color filledColor;

    AbilityInventory inventory;

    // subscribe to the player body
    protected override void Subscribe(PlayerBody body)
    {
        inventory = body.AbilityInventory;

        

        SetActive(inventory.MaxUses == 0 ? false : true);
        UpdateCooldown(inventory.RechargePercent);
    }

    // unsubscribe from the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        inventory = null;
    }


    protected override void Awake()
    {
        base.Awake();
        foreach (var bar in cooldownBars)
        {
            defaultColor = bar.color;
        }
    }

    public void SetActive(bool active)
    {
        barObject.SetActive(active);
    }

    public void Update()
    {
        if (inventory != null)
        {
            UpdateCooldown(1f - inventory.RechargePercent);
        }
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

        if (inventory.MaxUses ==0)
        {
            barObject.SetActive(false);
        }
        else
        {
            barObject.SetActive(true);
        }


    }
}
