using UnityEngine;

public class AI_AssignWeapon : MonoBehaviour
{
    [SerializeField] Equipment equipment;
    [SerializeField] PlayerStartEquipment playerStartEquipment;

    private void Start()
    {
        if (equipment != null)
        {
            playerStartEquipment.GetEquipment(equipment);
        }
        else
        {
            Debug.LogWarning("Weapon data is not assigned.");
        }
    }



}
