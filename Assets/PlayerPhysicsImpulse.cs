using UnityEngine;

public class PlayerPhysicsImpulse : MonoBehaviour
{
    //Vector3 impulseForce;
    [SerializeField] float horizontalImpulseMultiplayer_ground = 1f;
    [SerializeField] float verticalImpulseMultiplayer_ground = 1f;
	[SerializeField] float horizontalImpulseMultiplayer_air = 1f;
    [SerializeField] float verticalImpulseMultiplayer_air = 1f;

 //   [SerializeField] float horizontalDecay_ground = 0.1f;
	//[SerializeField] float horizontalDecay_air = 0.1f;

    CharacterController cc;
    PlayerMovement playerMovement;

	private void Awake()
	{
		cc = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
	}

    public float GetPlayerGravityForce()
    {
        return playerMovement.gravityVelocity;
	}

	public void AddImpulse(PlayerImpactStruct impulse)
    {
        var grounded = cc.isGrounded;
        var horizontalImpulseMultiplayer = grounded ? horizontalImpulseMultiplayer_ground : horizontalImpulseMultiplayer_air;
        var verticalImpulseMultiplayer = grounded ? verticalImpulseMultiplayer_ground : verticalImpulseMultiplayer_air;
		impulse.impactForce.x *= horizontalImpulseMultiplayer;
		impulse.impactForce.y *= verticalImpulseMultiplayer;
		impulse.impactForce.z *= horizontalImpulseMultiplayer;
		playerMovement.ApplyImpact(impulse);
	}

    public void ChangeGravity(float value)
    {
        if (value != 1 && playerMovement.gravityVelocity <0)
        {
            playerMovement.gravityVelocity *= value;

		}
        playerMovement.gravityMultiplier = value;
    }

	/*
	private void Update()
	{
        var grounded = cc.isGrounded;
		var horizontalDecay = grounded ? horizontalDecay_ground : horizontalDecay_air;

        impulseForce = Vector3.MoveTowards (
            impulseForce,
            new Vector3(0, impulseForce.y, 0),
            horizontalDecay * Time.deltaTime
        );



        if (grounded)
        {
            impulseForce.y = 0;

		}
        if (impulseForce.magnitude > 0.1)
        {
			cc.Move(impulseForce * Time.deltaTime);
		}

		
        

	}*/

}

public struct PlayerImpactStruct
{
    public Vector3 impactForce;
    public bool resetGravity;
}
