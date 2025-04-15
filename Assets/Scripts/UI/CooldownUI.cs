using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : InterfaceItem
{
    [SerializeField] GameObject barObject;
    [SerializeField] Image[] cooldownBars;

    Color defaultColor;
    [SerializeField] Color filledColor;

    // subscribe to the player body
    protected override void Subscribe(PlayerBody body)
    {
        var inventory = body.PlayerInventory;

        inventory.OnMaxGranadeCountChanged += (count) => SetActive(count == 0 ? false : true);
        inventory.OnGranadeChargeChanged += UpdateCooldown;

        SetActive(inventory.GranadeInventorySize == 0 ? false : true);
        UpdateCooldown(inventory.GranadeCharge);
    }

    // unsubscribe from the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        var inventory = body.PlayerInventory;

        inventory.OnGranadeChargeChanged -= UpdateCooldown;
        inventory.OnMaxGranadeCountChanged -= (count) => SetActive(count == 0 ? false : true);
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
}
