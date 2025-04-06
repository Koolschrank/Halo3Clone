using UnityEngine;
using TMPro;
using NUnit.Framework;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponUI : MonoBehaviour
{

    [SerializeField] Arm playerArm;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI magazinText;
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;

    [SerializeField] bool showMagazinText = true;
    [SerializeField] TextMeshProUGUI[] magazinTextToColor;
    [SerializeField] TextMeshProUGUI[] reserveTextToColor;
    [SerializeField] Image weaponSprite;

    int magazinSize = 0;

    [Header("BulletUI")]
    [SerializeField] RectTransform bulletUI;
    [SerializeField] GameObject bulletPrefab;
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
        if (!gameObject.activeSelf && weapon.ShowAmmoUI)
        {
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf && !weapon.ShowAmmoUI)
        {
            gameObject.SetActive(false);
        }

        magazinSize = weapon.MagazineSize;
        weapon.OnMagazineChange += UpdateMagazin;
        UpdateMagazin(weapon.Magazine);
        UpdateReserve(playerArm.AmmoOfWeaponInReserve);
        SetUpBulletUI(weapon.BulletSpriteUI, weapon.MagazineSize, weapon.BulletsPerRowUI, weapon.BulletSizeUI);

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
    }

    void UnequipWeapon(Weapon_Arms weapon)
    {
        weapon.OnMagazineChange -= UpdateMagazin;

        ammoText.text = "";
        magazinText.text = "";
        reserveText.text = "";
    }

    void UpdateMagazin(int magazin)
    {

        if (showMagazinText)
        {
            if (magazin == 0)
            {
                foreach (var text in magazinTextToColor)
                {
                    text.color = emptyColor;
                }
            }
            else
            {
                foreach (var text in magazinTextToColor)
                {
                    text.color = baseColor;
                }
            }

            ammoText.text = magazin.ToString();
            magazinText.text = "/ " + magazinSize.ToString();
        }
        else
        {
            foreach (var text in magazinTextToColor)
            {
                // 100% transparent
                text.color = new Color(1, 1, 1, 0);
            }
        }
            UpdateBulletUI(magazin);
    }

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
        reserveText.text = reserve.ToString();
    }


    public void SetUpBulletUI(Sprite bulletSprite, int count, int bulletsPerRow, float bulletSize)
    {
        var bulletUIWidth = bulletUI.rect.width;
        var bulletUIHeight = bulletUI.rect.height;
        if (bulletsPerRow <= 0)
        {
            bulletsPerRow = 20; // default value, not important
        }
        var rows = Mathf.CeilToInt((float)count / bulletsPerRow);
        // delete all bullets in list
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
        bullets.Clear();

        

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
            bulletTransform.localScale = new Vector2(bulletSize, bulletSize);
            
            int row = i / bulletsPerRow;
            row++;
            Debug.Log(row);
            int column = bulletsPerRow -  (i % bulletsPerRow);
            float x =
                (bulletUIWidth / bulletsPerRow) * column - (bulletUIWidth / 2) + (bulletSize / 2);

            float y = 0;
            if (rows != 1)
            {
                y =
                    (bulletUIHeight / rows) * row - (bulletUIHeight / 2) + (bulletSize / 2);
            }
            bulletTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    public void UpdateBulletUI(int count)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (count == 0)
            {
                bullets[i].color = BulletsDepletedColor;
                continue;
            }

            if (i < count)
            {
                bullets[i].color = BulletColor;
            }
            else
            {
                bullets[i].color = BulletEmptyColor;
            }
        }
    }

}





