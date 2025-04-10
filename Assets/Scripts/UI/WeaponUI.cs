using UnityEngine;
using TMPro;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponUI : MonoBehaviour
{

    [SerializeField] Arm playerArm;
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;

    [SerializeField] TextMeshProUGUI[] reserveTextToColor;
    [SerializeField] Image weaponSprite;


    [Header("BulletUI")]
    [SerializeField] RectTransform bulletUI;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSizeImpactOnPosition = 2f; 
    [SerializeField] Color BulletColor;
    [SerializeField] Color BulletEmptyColor;
    [SerializeField] Color BulletsDepletedColor;
    List<Image> bullets = new List<Image>();


    public void SetUp(Arm playerArm)
    {
        if (this.playerArm != null)
        {
            playerArm.OnWeaponEquipStarted -= EquipWeapon;
            playerArm.OnWeaponUnequipFinished -= UnequipWeapon;
            playerArm.OnWeaponDroped -= (weapon, pickUp) => UnequipWeapon(weapon);
            playerArm.OnReserveAmmoChanged -= UpdateReserve;
        }


        this.playerArm = playerArm;
        playerArm.OnWeaponEquipStarted += EquipWeapon;
        playerArm.OnWeaponUnequipFinished += UnequipWeapon;
        playerArm.OnWeaponDroped -= (weapon, pickUp) => UnequipWeapon(weapon);
        playerArm.OnReserveAmmoChanged += UpdateReserve;

        



        if ( playerArm.CurrentWeapon == null)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }


    void Awake()
    {
        

    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }




    void EquipWeapon(Weapon_Arms weapon, float timer)
    {
        if ( weapon.ShowAmmoUI)
        {
            bulletUI.gameObject.SetActive(true);
            foreach (var text in reserveTextToColor)
            {
                text.gameObject.SetActive(true);
            }
        }
        else if ( !weapon.ShowAmmoUI)
        {
            bulletUI.gameObject.SetActive(false);
            foreach (var text in reserveTextToColor)
            {
                text.gameObject.SetActive(false);
            }
        }
        Enable();

        weapon.OnMagazineChange += UpdateMagazin;
       
        UpdateReserve(playerArm.AmmoOfWeaponInReserve);
        SetUpBulletUI(weapon.BulletSpriteUI, weapon.MagazineSize, weapon.BulletsPerRowUI, weapon.BulletSizeUI);
        UpdateMagazin(weapon.Magazine);

        var newSprite = weapon.GunSpriteUI;
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

    void UnequipWeapon(Weapon_Arms weapon)
    {
        weapon.OnMagazineChange -= UpdateMagazin;

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





