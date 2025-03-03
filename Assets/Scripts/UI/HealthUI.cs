using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image healthBar;
    [SerializeField] PlayerCamera playerCam;
    [SerializeField] AnimationCurve blodyScreenCurve;

    public void SetUp(Health health)
    {
        if (this.health != null)
        {
            health.OnHealthChanged -= UpdateHealth;
        }


        this.health = health;
        health.OnHealthChanged += UpdateHealth;
        UpdateHealth(health.HealthPercentage);


    }

    public void UpdateHealth(float healthValue)
    {
        healthBar.fillAmount = healthValue;

        

        if (healthValue <= 0) // disable on death
        {
            playerCam.SetVignetteIntensity(0);
        }

        playerCam.SetVignetteIntensity(blodyScreenCurve.Evaluate(1 - healthValue));
    }
}
