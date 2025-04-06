using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] GameObject barObject;
    [SerializeField] Image cooldownBar;

    Color defaultColor;
    [SerializeField] Color filledColor;

    private void Awake()
    {
        defaultColor = cooldownBar.color;
    }

    public void SetActive(bool active)
    {
        barObject.SetActive(active);
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
