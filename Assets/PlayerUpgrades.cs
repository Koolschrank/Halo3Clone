using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerUpgrades : MonoBehaviour
{
    [SerializeField] bool autoUpgradeEverything = false;

    GameObject playerBody;


    bool[] upgradeBools = new bool[50];
    [SerializeField] Upgrade[] upgrades;
    public void Upgrade(int index)
    {
        if (index < 0 || index >= upgradeBools.Length)
        {
            Debug.LogError("Upgrade index out of range: " + index);
            return;
        }
        upgradeBools[index] = true;
        ApplyUpgradeOnBody(index, playerBody);

    }

    public bool[] GetUpgrades()
    {
        return upgradeBools;
    }

    public void AssignBody(GameObject body)
    {
        playerBody = body;
        
    }

    public void ApplyAllUpgradesOnBody(GameObject body)
    {
        for (int i = 0; i < upgradeBools.Length; i++)
        {
            if (upgradeBools[i] || autoUpgradeEverything)
            {
                ApplyUpgradeOnBody(i, body);
            }
        }
    }

    public void ApplyUpgradeOnBody(int upgradeIndex, GameObject body)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
        {
            return;
        }

        var upgrade = upgrades[upgradeIndex];
        if (upgrade == null)
        {
            return;
        }

        upgrade.Apply(body);
    }

    public Upgrade GetUpgrade(int index)
    {
        if (index < 0 || index >= upgrades.Length)
        {
            return null;
        }
        return upgrades[index];
    }

    public int[] GetIndexOfRandomAbiliyNotEarnedYet(int amount)
    {
        List<int> unupgradedIndexes = new List<int>();
        for (int i = 0; i < upgradeBools.Length; i++)
        {
            if (!upgradeBools[i] && upgrades.Length > i && upgrades[i] != null)
            {
                unupgradedIndexes.Add(i);
            }
        }

        int[] randomIndexes = new int[amount];
        for (int i = 0; i < amount; i++)
        {
            if (unupgradedIndexes.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, unupgradedIndexes.Count);
            randomIndexes[i] = unupgradedIndexes[randomIndex];
            unupgradedIndexes.RemoveAt(randomIndex);
        }

        return randomIndexes;
    }





}


