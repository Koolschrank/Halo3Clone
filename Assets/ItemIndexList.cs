using Fusion;
using System;
using UnityEngine;

public class ItemIndexList : MonoBehaviour
{
    

    public static ItemIndexList Instance { get; private set; }
    [SerializeField] Weapon_Data[] weaponData;
    [SerializeField] Ability_Data[] ability_Data;


    int nextWeaponIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < weaponData.Length; i++)
        {
            if (weaponData[i] != null)
            {
                weaponData[i].WeaponTypeIndex = i;
            }
        }
        for (int i = 0; i < ability_Data.Length; i++)
        {
            if (ability_Data[i] != null)
            {
                ability_Data[i].AbilityTypeIndex = i;
            }
        }
    }


    


    public Weapon_Data GetWeaponViaIndex(int index)
    {
        if (index < 0 || index >= weaponData.Length)
        {
            
            return null;
        }
        return weaponData[index];
    }

    public Ability_Data GetAbilityViaIndex(int index)
    {
        if (index < 0 || index >= ability_Data.Length)
        {
            Debug.LogError("Invalid ability index: " + index);
            return null;
        }
        return ability_Data[index];
    }

    public int GetIndexViaWeapondData(Weapon_Data data)
    {
        for (int i = 0; i < weaponData.Length; i++)
        {
            if (weaponData[i] == data)
            {
                return i;
            }
        }
        Debug.LogError("Weapon data not found in the list: " + data);
        return -1;
    }

    public int GetNextIndex()
    {
        nextWeaponIndex++;
        return nextWeaponIndex;
    }


}


[Serializable]
public struct WeaponNetworkStruct : INetworkStruct
{
    public int index;
    public int weaponTypeIndex;
    public int ammoInMagazine;
    public int ammoInReserve;
}