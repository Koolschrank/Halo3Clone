using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] GameObject barObject;
    [SerializeField] Image[] cooldownBars;

    Color defaultColor;
    [SerializeField] Color filledColor;

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
}
