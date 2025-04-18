using Fusion;
using System;
using UnityEngine;

public class ItemIndexList : MonoBehaviour
{
    

    public static ItemIndexList Instance { get; private set; }
    [SerializeField] Weapon_Data[] weaponData;

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
                weaponData[i].WeaponIndex = i;
            }
        }
    }


    


    public Weapon_Data GetWeaponViaIndex(int index)
    {
        if (index < 0 || index >= weaponData.Length)
        {
            Debug.LogError("Invalid weapon index: " + index);
            return null;
        }
        return weaponData[index];
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
}


[Serializable]
public struct WeaponNetworkStruct : INetworkStruct
{
    public int weaponIndex;
    public int ammoInMagazine;
    public int ammoInReserve;
}