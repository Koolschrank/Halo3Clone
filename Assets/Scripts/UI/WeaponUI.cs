using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Arm playerArm;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI magazinText;
    [SerializeField] TextMeshProUGUI reserveText;

    [SerializeField] Color baseColor;
    [SerializeField] Color emptyColor;
    [SerializeField] TextMeshProUGUI[] textToColor;

    int magazinSize = 0;

    public void SetUp(Arm playerArm)
    {
        if (this.playerArm != null)
        {
            playerArm.OnWeaponEquipStarted -= EquipWeapon;
            playerArm.OnWeaponUnequipFinished -= UnequipWeapon;
            playerArm.OnWeaponDroped -= UnequipWeapon;
        }


        this.playerArm = playerArm;
        playerArm.OnWeaponEquipStarted += EquipWeapon;
        playerArm.OnWeaponUnequipFinished += UnequipWeapon;
        playerArm.OnWeaponDroped += UnequipWeapon;

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




    void EquipWeapon(Weapon_Arms weapon, float timer)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        magazinSize = weapon.MagazineSize;
        weapon.OnAmmoChange += UpdateAmmo;
        UpdateAmmo(weapon.Magazine, weapon.Reserve);
    }

    void UnequipWeapon(Weapon_Arms weapon)
    {
        weapon.OnAmmoChange -= UpdateAmmo;

        ammoText.text = "";
        magazinText.text = "";
        reserveText.text = "";
    }

    void UpdateAmmo(int magazin, int reserve)
    {
        // if ammo empty change color
        if (magazin == 0 && reserve == 0)
        {
            foreach (var text in textToColor)
            {
                text.color = emptyColor;
            }
        }
        else
        {
            foreach (var text in textToColor)
            {
                text.color = baseColor;
            }

            if (magazin == 0)
            {
                ammoText.color = emptyColor;
            }
            else
            {
                ammoText.color = baseColor;
            }

            if (reserve == 0)
            {
                reserveText.color = emptyColor;
            }
            else
            {
                reserveText.color = baseColor;
            }
        }



        ammoText.text = magazin.ToString();
        magazinText.text = "/ " + magazinSize.ToString();
        reserveText.text = reserve.ToString();
    }
}





