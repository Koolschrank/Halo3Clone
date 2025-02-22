using UnityEngine;

[CreateAssetMenu(fileName = "GranadeStats", menuName = "Granade Stats", order = 1)]
public class GranadeStats : ScriptableObject
{
    [SerializeField] float throwDelay = 0.3f;
    [SerializeField] float throwTime = 1f; // time until player can do another action
    [SerializeField] float throwAngle = 45f;
    [SerializeField] float throwForce = 10f;
    [SerializeField] GameObject granadePrefab = null;
    [SerializeField] GameObject granadeClonePrefab = null;

    public float ThrowDelay => throwDelay;

    public float ThrowTime => throwTime;

    public float ThrowAngle => throwAngle;

    public float ThrowForce => throwForce;

    public GameObject GranadePrefab => granadePrefab;

    public GameObject GranadeClonePrefab => granadeClonePrefab;
}
