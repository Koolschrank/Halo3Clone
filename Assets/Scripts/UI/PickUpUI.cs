using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickUpUI : MonoBehaviour
{
    [SerializeField] string pickUpText = "Press E to pick up ";
    [SerializeField] string dualWieldText = "Press E to dual wield ";
    PlayerPickUpScan pickUpScan;
    PlayerInteractableTrigger interactableTrigger;

    [SerializeField] GameObject pickUpTextObject;

    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] Image weaponImage;

    [SerializeField] GameObject dualwieldObject;
    [SerializeField] TextMeshProUGUI dualWieldName;
    [SerializeField] TextMeshProUGUI price;

    [SerializeField] TextMeshProUGUI discription;
    [SerializeField] Color basePriceColor = Color.white;
    [SerializeField] Color notBuyablePriceColor = Color.red;
    [SerializeField] PlayerMind playerMind;

    [SerializeField] GameObject[] keyboardText;


    public void SetKeyboard()
    {
        foreach (var text in keyboardText)
        {
            if (text != null)
            {
                text.SetActive(true);
            }
		}
	}

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
        this.pickUpScan = pickUpScan;

        
    }

    public void SetUp(PlayerInteractableTrigger interactableTrigger)
    {
        if (interactableTrigger != null)
        {
            interactableTrigger.OnNewInteractable -= UpdateInteractable;
            interactableTrigger.OnRemoveInteractable -= ClearInteractableUI;
        }
        
        interactableTrigger.OnNewInteractable += UpdateInteractable;
        interactableTrigger.OnRemoveInteractable += ClearInteractableUI;
        ClearInteractableUI();
        this.interactableTrigger = interactableTrigger;

    }
    bool isOnInteractable = false;

    void UpdateInteractable(Interactable interactable)
    {
        isOnInteractable = true;
        price.gameObject.SetActive(interactable.HasPrice);
        if (interactable.HasPrice)
        {
            
            price.text = "Cost: " + interactable.Price.ToString() + "$";
            if (playerMind != null && !interactable.CanAfford(playerMind.Score))
            {
                price.color = notBuyablePriceColor;
            }
            else
            {
                price.color = basePriceColor;
            }
        }
        else
        {
            price.text = "";

        }

        weaponImage.sprite = null;
        weaponImage.enabled = false;
        pickUpTextObject.SetActive(true);
        weaponName.text = interactable.discription;
        discription.text = interactable.extraDiscription;
    }

    void ClearInteractableUI()
    {
        isOnInteractable = false;
        weaponName.text = "";
        price.text = "";
        pickUpTextObject.SetActive(false);

        discription.text = "";
    }


    void UpdatePickUpUI(Weapon_PickUp weapon_PickUp)
    {
        if (isOnInteractable) return;
        if (weapon_PickUp == null)
        {
            if (isOnInteractable) return;
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
        discription.text = "";
    }

    void ClearPickUpUI()
    {
        weaponName.text = "";
        pickUpTextObject.SetActive(false);
        weaponImage.sprite = null;
        weaponImage.enabled = false;
        discription.text = "";
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
        discription.text = "";
    }
}
