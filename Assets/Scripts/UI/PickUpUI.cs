using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpUI : InterfaceItem
{
    [SerializeField] string pickUpText = "Press E to pick up ";
    [SerializeField] string pickUpTextLeftHand = "Press E to dual wield ";
    [SerializeField] string pickUpTextRightHand = "Press E to pick up ";
    [SerializeField] GameObject pickUpTextObjectRightArm;
    [SerializeField] GameObject pickUpTextObjectLeftArm;

    [SerializeField] TextMeshProUGUI textRightArm;
    [SerializeField] TextMeshProUGUI textLeftArm;

    [SerializeField] Image weaponImage;

    WeaponInventoryExtended weaponInventory;
    Arms arms;

    protected override void Subscribe(PlayerBody body)
    {
        var pickUpScan = body.PlayerPickUpScan;
        weaponInventory = body.WeaponInventory;
        arms = body.PlayerArms;

        pickUpScan.OnIndexOfClosesWeaponChanged += UpdatePickUpUI;

        UpdatePickUpUI(pickUpScan.IndexOfClosestPickUp);
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var pickUpScan = body.PlayerPickUpScan;
        pickUpScan.OnIndexOfClosesWeaponChanged -= UpdatePickUpUI;

        weaponInventory = null;
        arms = null;

        UpdatePickUpUI(-1);
    }


    void UpdatePickUpUI(int weaponIndex)
    {
        if (weaponIndex == -1)
        {
            pickUpTextObjectLeftArm.SetActive(false);
            pickUpTextObjectRightArm.SetActive(false);
            weaponImage.enabled = false;
            return;
        }

        var weaponData = ItemIndexList.Instance.GetWeaponViaIndex(weaponIndex);
        Sprite weaponSprite = weaponData.GunSpriteUI;
        string weaponName = weaponData.WeaponName;
        bool canRightArmPickUp = true;
        bool canLeftArmPickUp =
            arms.CanDualWield2HandedWeapons
            || (arms.Weapon_RightHand != null 
            && arms.Weapon_RightHand.WeaponType == WeaponType.oneHanded
            && weaponData.WeaponType == WeaponType.oneHanded);

        if (canRightArmPickUp && canLeftArmPickUp)
        {
            pickUpTextObjectLeftArm.SetActive(true);
            textLeftArm.text = pickUpTextLeftHand + weaponName;

            pickUpTextObjectRightArm.SetActive(true);
            textRightArm.text = pickUpTextRightHand + weaponName;



        }
        else if (canRightArmPickUp)
        {
            pickUpTextObjectRightArm.SetActive(true);
            textRightArm.text = pickUpText + weaponName;

            pickUpTextObjectLeftArm.SetActive(false);

        }
        else
        {
            pickUpTextObjectRightArm.SetActive(false);
            pickUpTextObjectLeftArm.SetActive(false);
            weaponImage.enabled = false;
            return;
        }




        

        if (weaponSprite != null)
        {
            weaponImage.sprite = weaponSprite;
            weaponImage.enabled =true;
        }
        else
        {
            weaponImage.sprite = null;
            weaponImage.enabled = false;
        }
    }

}
