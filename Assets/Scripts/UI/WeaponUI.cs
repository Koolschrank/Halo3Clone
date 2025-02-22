using UnityEngine;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] PlayerArms playerArms;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI magazinText;
    [SerializeField] TextMeshProUGUI reserveText;

    int magazinSize = 0;

    public void SetUp(PlayerArms playerArms)
    {
        if (this.playerArms != null)
        {
            playerArms.OnWeaponEquipStarted -= EquipWeapon;
            playerArms.OnWeaponUnequipFinished -= UnequipWeapon;
            playerArms.OnWeaponDroped -= UnequipWeapon;
        }


        this.playerArms = playerArms;
        playerArms.OnWeaponEquipStarted += EquipWeapon;
        playerArms.OnWeaponUnequipFinished += UnequipWeapon;
        playerArms.OnWeaponDroped += UnequipWeapon;
    }


    void Awake()
    {
        

    }



    void EquipWeapon(Weapon_Arms weapon, float timer)
    {
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
        ammoText.text = magazin.ToString();
        magazinText.text = "/ " + magazinSize.ToString();
        reserveText.text = reserve.ToString();
    }
}





