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

    protected override void Subscribe(PlayerBody body)
    {
        var inventory = body.WeaponInventory;
        var arms = body.PlayerArms;

        inventory.OnBackWeaponEquipped += (weaponStruct) =>
        {
            var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex);
            if (weaponData == null)
            {
                Hide();
                return;
            }

            Show();
            UpdateWeaponSprite(weaponData.GunSpriteUI);
            UpdateAmmo(inventory.GetReserveAmmoBackWeapon() + weaponStruct.ammoInMagazine);
        };
        inventory.OnBackWeaponReserveAmmoChanged += UpdateAmmo;
        inventory.OnBackWeaponRemoved += (weapon) => Hide();

        if (inventory.BackWeapon.weaponTypeIndex != -1)
        {
            var weapon = inventory.BackWeapon;
            Show();
            UpdateAmmo(inventory.GetReserveAmmoBackWeapon() + inventory.BackWeapon.ammoInMagazine);
            UpdateWeaponSprite(ItemIndexList.Instance.GetWeaponViaIndex(weapon.weaponTypeIndex).GunSpriteUI);
        }
        else
        {
            Hide();
        }
    }

    // subscribe to the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        var inventory = body.WeaponInventory;
        
        
        inventory.OnBackWeaponEquipped -= (weaponStruct) =>
        {
            var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponStruct.weaponTypeIndex);
            if (weaponData == null)
            {
                Hide();
                return;
            }

            Show();
            UpdateWeaponSprite(weaponData.GunSpriteUI);
            UpdateAmmo(inventory.GetReserveAmmoBackWeapon() + weaponStruct.ammoInMagazine);
        };

        inventory.OnBackWeaponReserveAmmoChanged -= UpdateAmmo;
        inventory.OnBackWeaponRemoved -= (weapon) => Hide();
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
