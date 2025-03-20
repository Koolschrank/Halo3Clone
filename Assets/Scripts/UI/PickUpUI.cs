using TMPro;
using UnityEngine;

public class PickUpUI : MonoBehaviour
{
    [SerializeField] string pickUpText = "Press E to pick up ";
    [SerializeField] string dualWieldText = "Press E to dual wield ";
    [SerializeField] PlayerPickUpScan pickUpScan;
    [SerializeField] GameObject pickUpTextObject;

    [SerializeField] TextMeshProUGUI weaponName;

    [SerializeField] GameObject dualwieldObject;
    [SerializeField] TextMeshProUGUI dualWieldName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetUp(PlayerPickUpScan pickUpScan)
    {
        if (pickUpScan != null) {
            pickUpScan.OnWeaponPickUpUpdate -= UpdatePickUpText;
            pickUpScan.OnWeaponPickUp -= ClearTextPickUpText;
            pickUpScan.OnWeaponDualWieldUpdate -= UpdateDualWieldText;
        }

        this.pickUpScan = pickUpScan;
        pickUpScan.OnWeaponPickUpUpdate += UpdatePickUpText;
        pickUpScan.OnWeaponPickUp += ClearTextPickUpText;
        pickUpScan.OnWeaponDualWieldUpdate += UpdateDualWieldText;
        ClearTextPickUpText();
    }


    void UpdatePickUpText(Weapon_PickUp weapon_PickUp)
    {
        if (weapon_PickUp == null)
        {
            weaponName.text = "";
            pickUpTextObject.SetActive(false);
            return;
        }
        pickUpTextObject.SetActive(true);
        weaponName.text = pickUpText + weapon_PickUp.WeaponName;
    }

    void ClearTextPickUpText()
    {
        weaponName.text = "";
        pickUpTextObject.SetActive(false);
    }

    void UpdateDualWieldText(Weapon_PickUp weapon_PickUp)
    {
        if (weapon_PickUp == null)
        {
            dualwieldObject.SetActive(false);
            return;
        }
        dualwieldObject.SetActive(true);
        dualWieldName.text = dualWieldText;// + weapon_PickUp.WeaponName;
    }
}
