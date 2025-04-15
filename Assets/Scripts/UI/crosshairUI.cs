using UnityEngine;
using UnityEngine.UI;

public class crosshairUI : InterfaceItem
{
    Color baseColor;

    [SerializeField] Color onTargetColor;
    [SerializeField] RawImage crosshairImage;

    protected override void Subscribe(PlayerBody body)
    {
        var bulletSpawner = body.BulletSpawner;
        var rightArm = body.PlayerArms.RightArm;

        bulletSpawner.OnTargetAcquired += OnTargetAcquired;
        bulletSpawner.OnTargetLost += OnTargetLost;
        rightArm.OnWeaponEquipStarted += (weapon, time) => ChangeSprite(weapon.CrosshairUI, weapon.CrosshairSizeUI);

        var weapon = rightArm.CurrentWeapon;
        ChangeSprite(weapon.CrosshairUI, weapon.CrosshairSizeUI);
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var bulletSpawner = body.BulletSpawner;
        var rightArm = body.PlayerArms.RightArm;

        bulletSpawner.OnTargetAcquired -= OnTargetAcquired;
        bulletSpawner.OnTargetLost -= OnTargetLost;
        rightArm.OnWeaponEquipStarted -= (weapon, time) => ChangeSprite(weapon.CrosshairUI, weapon.CrosshairSizeUI);

        OnTargetLost(null);
    }

   


    public void ChangeSprite(Sprite sprite, Vector2 size)
    {
        if (crosshairImage != null)
        {
            if (sprite == null)
            {
                crosshairImage.enabled = false;
                return;
            }
            else 
            {
                crosshairImage.enabled = true;
            }

            crosshairImage.texture = sprite.texture;
            crosshairImage.rectTransform.localScale = size;
        }
    }

    //start
    private void Start()
    {
        baseColor = crosshairImage.color;
    }

    // target acquired
    public void OnTargetAcquired(Transform target)
    {
        crosshairImage.color = onTargetColor;
    }

    // target lost
    public void OnTargetLost(Transform target)
    {
        crosshairImage.color = baseColor;
    }
}
