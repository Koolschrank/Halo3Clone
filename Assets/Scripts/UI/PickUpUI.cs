using TMPro;
using UnityEngine;

public class PickUpUI : MonoBehaviour
{
    [SerializeField] string pickUpText = "Press E to pick up ";
    [SerializeField] PlayerPickUpScan pickUpScan;
    [SerializeField] TextMeshProUGUI weaponName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetUp(PlayerPickUpScan pickUpScan)
    {
        if (pickUpScan != null) {
            pickUpScan.OnWeaponPickUpUpdate -= UpdateUI;
            pickUpScan.OnWeaponPickUp -= ClearText;
        }

        this.pickUpScan = pickUpScan;
        pickUpScan.OnWeaponPickUpUpdate += UpdateUI;
        pickUpScan.OnWeaponPickUp += ClearText;
        ClearText();
    }


    void UpdateUI(Weapon_PickUp weapon_PickUp)
    {
        if (weapon_PickUp == null)
        {
            weaponName.text = "";
            return;
        }
        weaponName.text = pickUpText + weapon_PickUp.WeaponName;
    }

    void ClearText()
    {
        weaponName.text = "";
    }
}
