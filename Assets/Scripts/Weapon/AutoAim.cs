using UnityEngine;


[CreateAssetMenu(fileName = "AutoAim", menuName = "Weapon/AutoAim")]
public class AutoAim : ScriptableObject
{
    [SerializeField] float raycastLenght = 10f;
    [SerializeField] float radius = 1f;
    [SerializeField] float aimLerp = 0.5f;

    public float RaycastLenght => raycastLenght;
    public float Radius => radius;
    public float AimLerp => aimLerp;

}
