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
        var arms = body.PlayerArms;

        bulletSpawner.OnTargetAcquired += OnTargetAcquired;
        bulletSpawner.OnTargetLost += OnTargetLost;
        arms.OnRightWeaponEquiped += (weapon) => ChangeSprite(weapon.CrosshairUI, weapon.CrosshairSizeUI);


        if (arms.Weapon_RightHand != null)
        {
            ChangeSprite(arms.Weapon_RightHand.CrosshairUI, arms.Weapon_RightHand.CrosshairSizeUI);
        }
       
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var bulletSpawner = body.BulletSpawner;
        var arms = body.PlayerArms;

        bulletSpawner.OnTargetAcquired -= OnTargetAcquired;
        bulletSpawner.OnTargetLost -= OnTargetLost;
        arms.OnRightWeaponEquiped -= (weapon) => ChangeSprite(weapon.CrosshairUI, weapon.CrosshairSizeUI);

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
