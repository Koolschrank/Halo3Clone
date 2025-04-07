using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpUI : MonoBehaviour
{
    [SerializeField] string pickUpText = "Press E to pick up ";
    [SerializeField] string dualWieldText = "Press E to dual wield ";
    [SerializeField] PlayerPickUpScan pickUpScan;
    [SerializeField] GameObject pickUpTextObject;

    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] Image weaponImage;

    [SerializeField] GameObject dualwieldObject;
    [SerializeField] TextMeshProUGUI dualWieldName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetUp(PlayerPickUpScan pickUpScan)
    {
        if (pickUpScan != null) {
            pickUpScan.OnWeaponPickUpUpdate -= UpdatePickUpUI;
            pickUpScan.OnWeaponPickUp -= ClearPickUpUI;
            pickUpScan.OnWeaponDualWieldUpdate -= UpdateDualWieldText;
        }

        this.pickUpScan = pickUpScan;
        pickUpScan.OnWeaponPickUpUpdate += UpdatePickUpUI;
        pickUpScan.OnWeaponPickUp += ClearPickUpUI;
        pickUpScan.OnWeaponDualWieldUpdate += UpdateDualWieldText;
        ClearPickUpUI();
    }


    void UpdatePickUpUI(Weapon_PickUp weapon_PickUp)
    {
        if (weapon_PickUp == null)
        {
            weaponName.text = "";
            pickUpTextObject.SetActive(false);
            return;
        }
        pickUpTextObject.SetActive(true);
        weaponName.text = pickUpText + weapon_PickUp.WeaponName;

        var sprite = weapon_PickUp.GunSpriteUI;

        if (sprite != null)
        {
            weaponImage.sprite = sprite;
            weaponImage.enabled =true;
        }
        else
        {
            weaponImage.sprite = null;
            weaponImage.enabled = false;
        }
    }

    void ClearPickUpUI()
    {
        weaponName.text = "";
        pickUpTextObject.SetActive(false);
        weaponImage.sprite = null;
        weaponImage.enabled = false;
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
