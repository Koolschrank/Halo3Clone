using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "PlayerMeleeAttack", menuName = "Player/PlayerMeleeAttack")]
public class PlayerMeleeAttack : ScriptableObject
{
    [SerializeField] float damage = 80f;
    [SerializeField] float force = 10f;
    [SerializeField] float delay = 0.3f;
    [SerializeField] float meleeTime = 1f;
    [SerializeField] float meleeRadius = 1f;
    [SerializeField] float meleeDistance = 1f;
    [SerializeField] LayerMask enemyLayer;

    [Header("Sound")]
    [SerializeField] EventReference swingSound;
    [SerializeField] EventReference hitSound;


    public float Damage => damage;

    public float Force => force;

    public float Delay => delay;

    public float MeleeTime => meleeTime;

    public float MeleeRadius => meleeRadius;

    public float MeleeDistance => meleeDistance;

    public LayerMask EnemyLayer => enemyLayer;

    public EventReference SwingSound => swingSound;
    public EventReference HitSound => hitSound;






}
