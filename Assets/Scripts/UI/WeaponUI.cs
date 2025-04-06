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

    [SerializeField] TextMeshProUGUI[] magazinTextToColor;
    [SerializeField] TextMeshProUGUI[] reserveTextToColor;

    int magazinSize = 0;

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
        if (!gameObject.activeSelf && weapon.ShowAmmo)
        {
            gameObject.SetActive(true);
        }
        else if (gameObject.activeSelf && !weapon.ShowAmmo)
        {
            gameObject.SetActive(false);
        }

        magazinSize = weapon.MagazineSize;
        weapon.OnMagazineChange += UpdateMagazin;
        UpdateMagazin(weapon.Magazine);
        UpdateReserve(playerArm.AmmoOfWeaponInReserve);
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
        // if ammo empty change color
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
}





