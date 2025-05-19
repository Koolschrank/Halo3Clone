using System;
using UnityEngine;

public class UI_UpgradeMenu : MonoBehaviour
{
    public Action<int> OnUpgradeSelected;


    [SerializeField] PlayerMind playerMind;
    [SerializeField] GameObject upgradeBox;

    [SerializeField] Transform[] boxPlacement;


    public void AddUpgradeBoxes(Upgrade[] upgrades)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            GameObject box = Instantiate(upgradeBox, boxPlacement[i]);
            box.GetComponent<UI_UpgradeBox>().SetUpgrade(upgrades[i]);
        }
    }


    public void ClearUpgradeBoxes()
    {
        foreach (Transform box in boxPlacement)
        {
            if (box.childCount > 0)
            {
                Destroy(box.GetChild(0).gameObject);
            }
        }
        
    }

    public void Select1()
    {
        OnUpgradeSelected?.Invoke(0);
        ClearUpgradeBoxes();
    }

    public void Select2()
    {
        OnUpgradeSelected?.Invoke(1);
        ClearUpgradeBoxes();
    }

    public void Select3()
    {
        OnUpgradeSelected?.Invoke(2);
        ClearUpgradeBoxes();
    }

}
