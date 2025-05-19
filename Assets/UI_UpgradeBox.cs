using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeBox : MonoBehaviour
{


    [SerializeField] TextMeshProUGUI boxName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image icon;
    [SerializeField] Image buttonIcon;


    public void SetUpgrade(Upgrade upgrade)
    {
        boxName.text = upgrade.UpgradeName;
        description.text = upgrade.Description;
        icon.sprite = upgrade.Icon;
        
    }
}
