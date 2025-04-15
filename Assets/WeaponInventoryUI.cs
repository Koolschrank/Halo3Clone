using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryUI : InterfaceItem
{
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;

    [SerializeField] TextMeshProUGUI[] reserveTextToColor;
    [SerializeField] Image weaponSprite;


    // subscribe to the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        var inventory = body.PlayerInventory;
        inventory.OnWeaponAddedToInventory -= (weapon, ammo) =>
        {
            Show();
            UpdateWeaponSprite(weapon.GunSpriteUI);
            UpdateAmmo(ammo);
        };

        inventory.OnAmmoOfWeaponInInventoryChanged -= UpdateAmmo;
        inventory.OnWeaponDrop -= (weapon) => Hide();
    }

    protected override void Subscribe(PlayerBody body)
    {
        var inventory = body.PlayerInventory;
        inventory.OnWeaponAddedToInventory += (weapon, ammo) =>
        {
            Show();
            UpdateWeaponSprite(weapon.GunSpriteUI);
            UpdateAmmo(ammo);
        };
        inventory.OnAmmoOfWeaponInInventoryChanged += UpdateAmmo;
        inventory.OnWeaponDrop += (weapon) => Hide();
        if (inventory.HasWeapon)
        {
            var weapon = inventory.GetWeapon();
            Show();
            UpdateAmmo(weapon.Magazine + inventory.GetAmmo(weapon.Data));
            UpdateWeaponSprite(weapon.GunSpriteUI);
        }
        else
        {
            Hide();
        }
    }

    public void UpdateWeaponSprite(Sprite sprite)
    {
        if (weaponSprite != null)
        {
            weaponSprite.sprite = sprite;
        }
    }

    public void UpdateAmmo(int ammo)
    {
        if (reserveText != null)
        {
            reserveText.text = ammo.ToString();
        }
        if (reserveTextToColor != null)
        {
            foreach (var text in reserveTextToColor)
            {
                text.color = ammo > 0 ? baseColor : emptyColor;
            }
            weaponSprite.color = ammo > 0 ? baseColor : emptyColor;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
