using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "GranadeStats", menuName = "Granade Stats", order = 1)]
public class GranadeStats : ScriptableObject
{
    [SerializeField] float throwDelay = 0.3f;
    [SerializeField] float throwTime = 1f; // time until player can do another action
    [SerializeField] float throwAngle = 45f;
    [SerializeField] float throwForce = 10f;
    [SerializeField] GameObject granadePrefab = null;
    [SerializeField] GameObject granadeClonePrefab = null;

	[Header("Sound")]
	[SerializeField] EventReference throwSound;
    [SerializeField] float throwSoundDelay = 0.1f;

	public float ThrowDelay => throwDelay;

    public float ThrowTime => throwTime;

    public float ThrowAngle => throwAngle;

    public float ThrowForce => throwForce;

    public GameObject GranadePrefab => granadePrefab;

    public GameObject GranadeClonePrefab => granadeClonePrefab;

    public EventReference ThrowSound => throwSound;

    public float ThrowSoundDelay => throwSoundDelay;


	public GranadeStats(GranadeStats statsToCopy)
    {
        throwDelay = statsToCopy.throwDelay;
        throwTime = statsToCopy.throwTime;
        throwAngle = statsToCopy.throwAngle;
        throwForce = statsToCopy.throwForce;
        granadePrefab = statsToCopy.granadePrefab;
        granadeClonePrefab = statsToCopy.granadeClonePrefab;
        throwSound = statsToCopy.throwSound;
        throwSoundDelay = statsToCopy.throwSoundDelay;
	}
}
