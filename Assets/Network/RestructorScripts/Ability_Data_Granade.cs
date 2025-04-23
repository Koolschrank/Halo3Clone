using UnityEngine;


[CreateAssetMenu(fileName = "Ability_Data_Granade", menuName = "ScriptableObjects/Ability/Granade")]
public class Ability_Data_Granade : Ability_Data
{
    [SerializeField] GranadeStats granade;

    public GranadeStats Granade => granade;
}
