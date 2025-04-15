using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HealthUI : InterfaceItem
{
    [SerializeField] Image healthBar;
    [SerializeField] PlayerCamera playerCam;
    [SerializeField] AnimationCurve blodyScreenCurve;

    protected override void Unsubscribe(PlayerBody body)
    {
        var health = body.Health;
        health.OnHealthChanged -= UpdateHealth;
    }

    protected override void Subscribe(PlayerBody body)
    {
        var health = body.Health;
        health.OnHealthChanged += UpdateHealth;
        health.OnDeath += Clear;

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

    public void Clear()
    {
        playerCam.SetVignetteIntensity(0);
    }
}
