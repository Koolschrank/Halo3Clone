using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventoryUI : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;

    [SerializeField] TextMeshProUGUI[] reserveTextToColor;
    [SerializeField] Image weaponSprite;
    bool showInPercent = false;



	public void SetUp(PlayerInventory playerInventory)
    {
        if (this.playerInventory != null)
        {
            playerInventory.OnWeaponAddedToInventory += (weapon, ammo) =>
            {
                Show();
                UpdateWeaponSprite(weapon.GunSpriteUI);
                UpdateAmmo(ammo);
            };
            
            playerInventory.OnAmmoOfWeaponInInventoryChanged -= UpdateAmmo;
            playerInventory.OnWeaponDrop -= (weapon) => Hide();
        }
        this.playerInventory = playerInventory;
        playerInventory.OnWeaponAddedToInventory += (weapon, ammo) =>
        {
            Show();
            UpdateWeaponSprite(weapon.GunSpriteUI);
            UpdateAmmo(ammo);
        };
        playerInventory.OnAmmoOfWeaponInInventoryChanged += UpdateAmmo;
        playerInventory.OnWeaponDrop += (weapon) => Hide();
        if (playerInventory.HasWeapon)
        {
            Show();
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


        var weapon = playerInventory.GetWeapon();
        if (weapon != null)
        {
            showInPercent = weapon.Data.showAmmoInPercent;
		}
	}

    public void UpdateAmmo(int ammo)
    {
		if (showInPercent)
        {
            var weapon = playerInventory.GetWeapon();
            if (weapon != null && weapon.Data != null && weapon.Data.MagazineSize > 0)
            {
                var percent = (int)(((float)weapon.Magazine / weapon.Data.MagazineSize) * 100);
                reserveText.text = percent.ToString() + "%";

                if (percent == 0)
                {
                    foreach (var text in reserveTextToColor)
                    {
                        text.color = emptyColor;
                    }
                    if (weaponSprite != null)
                    {
                        weaponSprite.color = emptyColor;
                    }
                }
                else
                {
                    foreach (var text in reserveTextToColor)
                    {
                        text.color = baseColor;
                    }
                    if (weaponSprite != null)
                    {
                        weaponSprite.color = baseColor;
                    }
				}
			}
            else
            {
                reserveText.text = "0%";
            }
            return;
		}



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
