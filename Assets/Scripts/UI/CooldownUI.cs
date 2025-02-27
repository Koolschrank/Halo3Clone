using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] Image cooldownBar;

    Color defaultColor;
    [SerializeField] Color filledColor;

    private void Start()
    {
        defaultColor = cooldownBar.color;
    }


    public void UpdateCooldown(float value)
    {
        cooldownBar.fillAmount = value;
        if (value == 1)
        {
            cooldownBar.color = filledColor;
        }
        else
        {
            cooldownBar.color = defaultColor;
        }
    }
}
