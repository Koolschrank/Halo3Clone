using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using Fusion;
using Fusion.Addons.SimpleKCC;

public class PlayerMovement : NetworkBehaviour
{

    public Action OnJump;
    public Action OnCrouch;
    public Action OnStandUp;
    public Action<Vector3> OnMoveUpdated;
    public Action<Vector2> OnAimUpdated;

    // character controller 
    [Header("References")]
    [SerializeField] SimpleKCC cc;
    [SerializeField] Transform head;
    [SerializeField] Transform head_normalPosition;
    [SerializeField] Transform head_crouchPosition;
    [SerializeField] PlayerArms arms;
    [Header("Settings")]
    // movement speed
    [SerializeField] float maxMoveSpeed = 12f;
    [SerializeField] float moveSpeedCrouchMultiplier = 0.4f;
    [SerializeField] float acceleration_ground = 10f;
    [SerializeField] float acceleration_air = 5f;
    [SerializeField] float deceleration_ground = 10f;
    [SerializeField] float deceleration_air = 5f;

    [SerializeField] float jumpPower = 9.8f;
    [SerializeField] float jumpCooldown = 0.5f;
    float jumpCooldownTimer = 0;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float cyoteTime = 0.2f;
    bool isGrounded => cc.IsGrounded || Time.time - lastGroundTouch < cyoteTime;
    float lastGroundTouch;
    [SerializeField] float crouchSpeed = 0.5f;


    float maxMoveSpeedMultiplier = 1f;


    [Header("Sound")]
    [SerializeField] EventReference walkSound;
    [SerializeField] float distanceForWalkSound = 1f;
    float distanceToWalkSoundLeft = 0;
    [SerializeField] EventReference jumpSound;




    Vector3 moveVelocity = Vector3.zero;
    float gravityVelocity = 0;
    Vector2 moveInput = Vector2.zero;

    public float MaxMoveSpeed => maxMoveSpeed * maxMoveSpeedMultiplier;
    bool inCrouch = false;


    // update
    public override void FixedUpdateNetwork()
    {
        UpdateCrouch();
        UpdateMove();
        UpdateGravity();

        var moveVector = new Vector3(moveVelocity.x, gravityVelocity, moveVelocity.z);
        cc.Move(moveVector * Runner.DeltaTime);

        OnMoveUpdated?.Invoke(moveVector);

        if (moveVelocity.magnitude > 0 && cc.IsGrounded)
        {
            distanceToWalkSoundLeft -= moveVelocity.magnitude * Runner.DeltaTime;
            if (distanceToWalkSoundLeft <= 0)
            {
                AudioManager.instance.PlayOneShot(walkSound, transform.position);
                distanceToWalkSoundLeft = distanceForWalkSound;
            }
        }

        if (cc.IsGrounded)
        {
            // todo : Time.time neess to be replaced with fusion time
            lastGroundTouch  = Time.time;
        }

    }

    private void UpdateCrouch()
    {

        
        if ( inCrouch)
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, head_crouchPosition.position, crouchSpeed * Runner.DeltaTime);
        }
        else
        {
            head.transform.position = Vector3.MoveTowards(head.transform.position, head_normalPosition.position, crouchSpeed * Runner.DeltaTime);
        }
    }

    private void FixedUpdate()
    {
        

    }


    private void UpdateMove()
    {
        GetInput(out NetworkInputData data);
        Vector2 input = data.controllerData1.moveVector;

        //Vector2 input = this.moveInput;//controller.Player.Move.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(input.x, 0, input.y);
        Vector3 camForward = head.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        
        Vector3 move = camForward * moveInput.z + head.transform.right * moveInput.x;


        if (move.magnitude == 0)
        {
            var deceleration = cc.IsGrounded ? deceleration_ground : deceleration_air;
            moveVelocity = Vector3.MoveTowards(moveVelocity, Vector2.zero, deceleration * Runner.DeltaTime);
        }
        else
        {
            var acceleration = cc.IsGrounded ? acceleration_ground : acceleration_air;

            var moveSpeedMultiplier = 1f;
            if (arms.IsDualWielding)
            {
                moveSpeedMultiplier = arms.MovementSpeedMultiplier;
            }
            else
            {
                var weapon = arms.RightArm.CurrentWeapon;
                if (weapon != null)
                {
                    moveSpeedMultiplier = weapon.MoveSpeedMultiplier;
                }
            }

           
            
            

            var speedMultiplier = MaxMoveSpeed * moveSpeedMultiplier;
            if (inCrouch)
            {
                speedMultiplier *= moveSpeedCrouchMultiplier;
            }

            moveVelocity = Vector3.MoveTowards(moveVelocity, move * speedMultiplier, acceleration * Runner.DeltaTime);
        }

        
    }

    public void SetMovementSpeedMultiplier(float multiplier)
    {
        maxMoveSpeedMultiplier = multiplier;
    }

    private void UpdateGravity()
    {

        if (cc.IsGrounded && jumpCooldownTimer <= 0)
        {
            gravityVelocity = -0.1f;
        }
        else
        {
            gravityVelocity -= gravity * Runner.DeltaTime;
        }

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }

    public void TryJump()
    {
        if (isGrounded && jumpCooldownTimer <= 0)
        {
            AudioManager.instance.PlayOneShot(jumpSound, transform.position);
            gravityVelocity = jumpPower;
            jumpCooldownTimer = jumpCooldown;
            OnJump?.Invoke();
            if (inCrouch)
            {
                ToggleCrouch();
            }
        }
    }

    // player input funtion
    public void UpdateMoveInput(Vector2 input)
    {
        moveInput = input;
    }


    public void ToggleCrouch()
    {


        
        if (inCrouch)
        {
            inCrouch = false;
            OnStandUp?.Invoke();
            
        }
        else if(cc.IsGrounded)
        {
            inCrouch = true;
            OnCrouch?.Invoke();
        }
    }


}