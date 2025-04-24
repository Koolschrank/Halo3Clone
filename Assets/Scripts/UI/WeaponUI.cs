using UnityEngine;
using TMPro;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using static UnityEngine.Rendering.GPUSort;

public class WeaponUI : InterfaceItem
{




    [SerializeField] bool leftArm = false;
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;

    [SerializeField] TextMeshProUGUI[] reserveTextToColor;
    [SerializeField] Image weaponSprite;

    WeaponInventoryExtended weaponInventory;


    [Header("BulletUI")]
    [SerializeField] RectTransform bulletUI;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSizeImpactOnPosition = 2f; 
    [SerializeField] Color BulletColor;
    [SerializeField] Color BulletEmptyColor;
    [SerializeField] Color BulletsDepletedColor;
    List<Image> bullets = new List<Image>();

    protected override void Subscribe(PlayerBody body)
    {
        var arms = body.PlayerArms;
        weaponInventory = body.WeaponInventory;

        if (leftArm)
        {

            arms.OnLeftWeaponRemoved += (weapon) => UnequipWeapon();
            arms.OnLeftWeaponEquiped += (weapon) =>
            {
                EquipWeapon(weapon);
                UpdateMagazin(weaponInventory.LeftWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoLeftWeapon());
            };
            weaponInventory.OnLeftWeaponReserveAmmoChanged += UpdateReserve;
            weaponInventory.OnLeftWeaponMagazinChanged += UpdateMagazin;

            if (weaponInventory.LeftWeapon.weaponTypeIndex == -1)
            {
                Disable();
            }
            else
            {
                UpdateMagazin(weaponInventory.LeftWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoLeftWeapon());
                Enable();
                EquipWeapon(arms.Weapon_LeftHand);

            }
        }
        else
        {
            arms.OnRightWeaponEquiped += (weapon) =>
            {
                EquipWeapon(weapon);
                UpdateMagazin(weaponInventory.RightWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoRightWeapon());
            };
            arms.OnRightWeaponRemoved += (weapon) => UnequipWeapon();
            weaponInventory.OnRightWeaponReserveAmmoChanged += UpdateReserve;
            weaponInventory.OnRightWeaponMagazinChanged += UpdateMagazin;

            if (weaponInventory.RightWeapon.weaponTypeIndex == -1)
            {
                Disable();
            }
            else
            {
                UpdateMagazin(weaponInventory.RightWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoRightWeapon());
                Enable();
                EquipWeapon(arms.Weapon_RightHand);

            }
        }
    }

    // subscribe to the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        var arms = body.PlayerArms;

        if (leftArm)
        {
            arms.OnLeftWeaponEquiped -= (weapon) =>
            {
                EquipWeapon(weapon);
                UpdateMagazin(weaponInventory.LeftWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoLeftWeapon());
            };

            arms.OnLeftWeaponRemoved -= (weapon) => UnequipWeapon();
            weaponInventory.OnLeftWeaponReserveAmmoChanged -= UpdateReserve;
            weaponInventory.OnLeftWeaponMagazinChanged -= UpdateMagazin;


        }
        else
        {
            arms.OnRightWeaponEquiped -= (weapon) =>
            {
                EquipWeapon(weapon);
                UpdateMagazin(weaponInventory.RightWeapon.ammoInMagazine);
                UpdateReserve(weaponInventory.GetReserveAmmoRightWeapon());
            };
            arms.OnRightWeaponRemoved -= (weapon) => UnequipWeapon();
            weaponInventory.OnRightWeaponReserveAmmoChanged -= UpdateReserve;
            weaponInventory.OnRightWeaponMagazinChanged -= UpdateMagazin;

        }


        weaponInventory = null;



    }

    


    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }




    void EquipWeapon(Weapon_Arms weapon)
    {
        if (weapon == null) return;


        var weaponData = weapon.Data;

        gameObject.SetActive(true);
        if (weaponData.ShowAmmoUI)
        {
            bulletUI.gameObject.SetActive(true);
            foreach (var text in reserveTextToColor)
            {
                text.gameObject.SetActive(true);
            }
        }
        else if ( !weaponData.ShowAmmoUI)
        {
            bulletUI.gameObject.SetActive(false);
            foreach (var text in reserveTextToColor)
            {
                text.gameObject.SetActive(false);
            }
        }
        Enable();

        //weapon.OnMagazineChange += UpdateMagazin;
       
        
        SetUpBulletUI(weaponData.BulletSpriteUI, weaponData.MagazineSize, weaponData.BulletsPerRowUI, weaponData.BulletSizeUI);
       

        var newSprite = weaponData.GunSpriteUI;
        if (newSprite != null)
        {
            weaponSprite.sprite = newSprite;
            weaponSprite.color = new Color(1, 1, 1, 1);
        }
        else
        {
            weaponSprite.sprite = null;
            weaponSprite.color = new Color(1, 1, 1, 0);
        }

        UpdateSprite();
    }

    void UnequipWeapon()
    {
        //if (weaponStruct.weaponTypeIndex == -1) return;

        gameObject.SetActive(false);

        DeleteBulletsFromUI();
        reserveText.text = "";
    }

    int magazin = 0;
    void UpdateMagazin(int magazin)
    {
        this.magazin = magazin;
        for (int i = 0; i < bullets.Count; i++)
        {
            if (magazin == 0)
            {
                bullets[i].color = BulletsDepletedColor;
                continue;
            }

            if (i < magazin)
            {
                bullets[i].color = BulletColor;
            }
            else
            {
                bullets[i].color = BulletEmptyColor;
            }
        }
        UpdateSprite();
    }

    int reserve = 0;
    void UpdateReserve(int reserve)
    {
        // if ammo empty change color
        if (reserve == 0)
        {
            foreach (var text in reserveTextToColor)
            {
                text.color = emptyColor;
            }
        }
        else
        {
            foreach (var text in reserveTextToColor)
            {
                text.color = baseColor;
            }
        }

        this.reserve = reserve;

        reserveText.text = reserve.ToString();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (reserve <= 0 && magazin <= 0)
        {
            weaponSprite.color = BulletsDepletedColor;
        }
        else
        {
            weaponSprite.color = BulletColor;
        }
    }


    public void SetUpBulletUI(Sprite bulletSprite, int count, int bulletsPerRow, Vector2 bulletSize)
    {
        var bulletUIWidth = bulletUI.rect.width;
        var bulletUIHeight = bulletUI.rect.height;
        if (bulletsPerRow <= 0)
        {
            bulletsPerRow = 20; // default value, not important
        }
        var rows = Mathf.CeilToInt((float)count / bulletsPerRow);
        // delete all bullets in list
        DeleteBulletsFromUI();



        // create new bullets
        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);

            var bulletImage = bullet.GetComponent<Image>();
            if (bulletImage != null)
            {
                bulletImage.sprite = bulletSprite;
            }
            bullets.Add(bulletImage);

            var bulletTransform = bullet.GetComponent<RectTransform>();
            bulletTransform.SetParent(bulletUI.transform);
            bulletTransform.localScale = bulletSize;

            
            int column = bulletsPerRow - (i % bulletsPerRow);
            float x =
                (bulletUIWidth / bulletsPerRow) * (column) - (bulletUIWidth / 2) + (bulletSize.x * bulletSizeImpactOnPosition);

            int row = i / bulletsPerRow;
            float rowHeight = 0;
            if (rows == 2)
            {
                if (row == 0)
                {
                    rowHeight = -0.5f;
                }
                else
                {
                    rowHeight = 0.5f;
                }
            }
            else if (rows == 3)
            {
                if (row == 0)
                {
                    rowHeight = -0.7f;
                }
                else if (row == 1)
                {
                    rowHeight = 0;
                }
                else
                {
                    rowHeight = 0.7f;
                }
            }

            float y =
                rowHeight * bulletUIHeight / 2;//+ (bulletSize / 2);
            bulletTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    private void DeleteBulletsFromUI()
    {
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        bullets.Clear();
    }
}





