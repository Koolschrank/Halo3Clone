using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image healthBar;
    [SerializeField] PlayerCamera playerCam;
    [SerializeField] AnimationCurve blodyScreenCurve;

    [SerializeField] Gradient healthBarColor;
    [SerializeField] bool showHealthBar = true;
    [SerializeField] GameObject[] healthBarObjects;
    [SerializeField] float healthBarDepletMultiplier = 1.2f; // multiplier to adjust the speed of health bar depletion

    [SerializeField] RectTransform healthBarRect;
    [SerializeField] RectTransform healthBarRectBackground;

    [SerializeField] float shortBarWidth = 500f;
    [SerializeField] float longBarWidth = 1000f;


    public void SetWidth(bool lenght)
    {
        return;
        if (lenght)
        {
            healthBarRect.sizeDelta = new Vector2(longBarWidth, healthBarRect.sizeDelta.y);
            healthBarRectBackground.sizeDelta = new Vector2(longBarWidth+25, healthBarRectBackground.sizeDelta.y);

        }
        else
        {
            healthBarRect.sizeDelta = new Vector2(shortBarWidth, healthBarRect.sizeDelta.y);
            healthBarRectBackground.sizeDelta = new Vector2(shortBarWidth+ 25, healthBarRectBackground.sizeDelta.y);
        }

    }

    public void SetUp(Health health)
    {
        if (this.health != null)
        {
            health.OnHealthChanged -= UpdateHealth;
            health.OnShowHealthBar -= ShowHealthBar;
            health.OnMaxHealthChanged -= UpdateMaxHealth;
        }


        this.health = health;
        health.OnHealthChanged += UpdateHealth;
        health.OnDeath += Clear;
        health.OnMaxHealthChanged += UpdateMaxHealth;
        UpdateHealth(health.HealthPercentage);

        health.OnShowHealthBar += ShowHealthBar;

        if (showHealthBar)
        {
            ShowHealthBar();
        }
        else
        {
            foreach (var obj in healthBarObjects)
            {
                obj.SetActive(false);
            }
        }

        UpdateMaxHealth(health.MaxHeath);


    }

    public void UpdateMaxHealth(float maxHealth)
    {
        if (maxHealth < 150f)
        {
            SetWidth(false);
        }
        else
        {
            SetWidth(true);
        }
    }

    public void ShowHealthBar()
    {
        showHealthBar = true;
        foreach (var obj in healthBarObjects)
        {
            obj.SetActive(true);
        }
    }


    public void UpdateHealth(float healthValue)
    {
        var valueLost = 1 - healthValue;
        




        if (healthValue <= 0) // disable on death
        {
            playerCam.SetVignetteIntensity(0);
        }

        
        if (showHealthBar)
        {
            var barValue = Mathf.Clamp(1 - valueLost * healthBarDepletMultiplier, 0.02f, 1);
            healthBar.fillAmount = barValue;
            if (healthValue >= 0.5)
            {
                valueLost = 0;
            }
            else
            {
                valueLost = 1 - (healthValue * 2); // scale to 0-1 range for the curve
            }
            var color = healthBarColor.Evaluate(healthValue);
            healthBar.color = color;
        }
        

        playerCam.SetVignetteIntensity(blodyScreenCurve.Evaluate(valueLost));

        
       
    }

    public void Clear()
    {
        playerCam.SetVignetteIntensity(0);
    }
}
