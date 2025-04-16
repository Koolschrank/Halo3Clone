using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;
using Fusion;

using Fusion.Addons.KCC;
using UnityEngine.Windows;

public class PlayerMovement : NetworkBehaviour
{

    public Action<Vector3> OnMoveUpdated;

    public Action OnJump;
    public Action OnCrouch;
    public Action OnStandUp;

    // character controller 
    [Header("References")]
    [SerializeField] PlayerBody playerBody;
    [SerializeField] KCC cc;
    [SerializeField] Transform head;
    [SerializeField] Transform head_normalPosition;
    [SerializeField] Transform head_crouchPosition;
    [SerializeField] PlayerArms arms;
    [Header("Settings")]
    // movement speed


    [SerializeField] float jumpPower = 9.8f;
    [SerializeField] float jumpCooldown = 0.5f;
    float jumpCooldownTimer = 0;
    [SerializeField] float cyoteTime = 0.2f;
    bool isGrounded => true; //cc.IsGrounded || Time.time - lastGroundTouch < cyoteTime;
    [SerializeField] float crouchSpeed = 0.5f;


    [Header("Sound")]
    [SerializeField] EventReference walkSound;
    [SerializeField] float distanceForWalkSound = 1f;
    float distanceToWalkSoundLeft = 0;
    [SerializeField] EventReference jumpSound;




    Vector3 moveVelocity = Vector3.zero;
    float gravityVelocity = 0;
    Vector2 moveInput = Vector2.zero;

    bool inCrouch = false;

    public override void Spawned()
    {
        
    }

    // update
    public override void FixedUpdateNetwork()
    {
        GetInput(out NetworkInputData data);
        LocalControllerData localControllerData = InputSplitter.GetContollerData(data, playerBody.LocalPlayerIndex);
        cc.SetInputDirection(cc.Data.TransformRotation * new Vector3(localControllerData.moveVector.x, 0.0f, localControllerData.moveVector.y));


        RPC_UpdateMove(cc.Data.KinematicVelocity);


        if (localControllerData.buttons.IsSet(InputButton.Jump) && cc.Data.IsGrounded)
        {
            cc.Jump(Vector3.up);
            RPC_UpdateJump();
            AudioManager.instance.PlayOneShot(jumpSound, transform.position);
        }


        var moveMagnitude = cc.Data.KinematicVelocity.magnitude;
        if (moveMagnitude > 0 && cc.Data.IsGrounded)
        {
            distanceToWalkSoundLeft -= moveMagnitude * Runner.DeltaTime;
            if (distanceToWalkSoundLeft <= 0)
            {
                AudioManager.instance.PlayOneShot(walkSound, transform.position);
                distanceToWalkSoundLeft = distanceForWalkSound;
            }
        }



        return;
        UpdateCrouch();
        UpdateMove();
        UpdateGravity();

        
        var moveVector = new Vector3(moveVelocity.x, 0, moveVelocity.z);
        //cc.Move(moveVector * Runner.DeltaTime);

        

        //OnMoveUpdated?.Invoke(moveVector);

        //if (moveVelocity.magnitude > 0 && cc.IsGrounded)
        //{
        //    distanceToWalkSoundLeft -= moveVelocity.magnitude * Runner.DeltaTime;
        //    if (distanceToWalkSoundLeft <= 0)
        //    {
        //        AudioManager.instance.PlayOneShot(walkSound, transform.position);
        //        distanceToWalkSoundLeft = distanceForWalkSound;
        //    }
        //}

        //if (cc.IsGrounded)
        //{
        //    // todo : Time.time neess to be replaced with fusion time
        //    lastGroundTouch  = Time.time;
        //}

    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_UpdateMove(Vector3 moveVector)
    {
        OnMoveUpdated?.Invoke(moveVector);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_UpdateJump()
    {
        OnJump?.Invoke();
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
        //GetInput(out NetworkInputData data);
        //Vector2 input = data.controllerData1.moveVector;

        

        GetInput(out NetworkInputData data);
        LocalControllerData localControllerData = InputSplitter.GetContollerData(data, playerBody.LocalPlayerIndex);

        Vector2 input = localControllerData.moveVector;//controller.Player.Move.ReadValue<Vector2>();
        
        Vector3 moveInput = new Vector3(input.x, 0, input.y);
        Vector3 camForward = head.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        
        Vector3 move = camForward * moveInput.z + head.transform.right * moveInput.x;
        /*

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

        if ( localControllerData.buttons.IsSet(InputButton.Jump))
        {
            TryJump();
        }*/



    }

    //public void SetMovementSpeedMultiplier(float multiplier)
    //{
    //    maxMoveSpeedMultiplier = multiplier;
    //}

    private void UpdateGravity()
    {

        //if (cc.IsGrounded && jumpCooldownTimer <= 0)
        //{
        //    gravityVelocity = -0.1f;
        //}
        //else
        //{
        //    gravityVelocity -= gravity * Runner.DeltaTime;
        //}

        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Runner.DeltaTime;
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
        //else if(cc.IsGrounded)
        //{
        //    inCrouch = true;
        //    OnCrouch?.Invoke();
        //}
    }


}