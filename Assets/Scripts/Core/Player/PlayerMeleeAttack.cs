using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "PlayerMeleeAttack", menuName = "Player/PlayerMeleeAttack")]
public class PlayerMeleeAttack : ScriptableObject
{
    [SerializeField] float damage = 80f;
    [SerializeField] float damageMultiplierVSAI = 1f; // multiplier for damage against AI
	[SerializeField] float damageMultiplierAgainstTeamMates = 1f;
	[SerializeField] float force = 10f;
    [SerializeField] float forceOnPlayers = 5f; // force applied to players hit by the melee attack
	[SerializeField] float forceOffset = 0f;
	[SerializeField] float delay = 0.3f;
    [SerializeField] float meleeTime = 1f;
    [SerializeField] float meleeRadius = 1f;
    [SerializeField] float meleeDistance = 1f;
    [SerializeField] LayerMask enemyLayer;


	[Header("Launch")]
	public bool hasLaunch = false;
	public float launchDistance = 5f;
	public float launchAngle = 45f;
	public float launchTime = 0.5f;
	public float launchStopDistance = 0.1f;
    public bool launchResetsGravity = false;
	public AnimationCurve launchCurve;
	public LayerMask launchTargetLayer;

	[Header("Sound")]
    [SerializeField] EventReference swingSound;
    [SerializeField] EventReference hitSound;



	public float Damage => damage;

    public float DamageMultiplierVSAI => damageMultiplierVSAI;

    public float DamageMultiplierAgainstTeamMates => damageMultiplierAgainstTeamMates;
	public float Force => force;

    public float ForceOnPlayers => forceOnPlayers;

    public float ForceOffset => forceOffset;

	public float Delay => delay;

    public float MeleeTime => meleeTime;

    public float MeleeRadius => meleeRadius;

    public float MeleeDistance => meleeDistance;

    public LayerMask EnemyLayer => enemyLayer;

    public EventReference SwingSound => swingSound;
    public EventReference HitSound => hitSound;






}
