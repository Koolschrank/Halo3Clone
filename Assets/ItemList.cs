using UnityEngine;

public class ItemList : MonoBehaviour
{
    // singelton
    public static ItemList instance;

    public Weapon_Data[] weapons;


    // awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Weapon_Data GetPistol()
    {
        return weapons[0];
    }

    public Weapon_Data GetRandomWeapon()
    {
        int randomIndex = Random.Range(0, weapons.Length);
        return weapons[randomIndex];
    }
}
