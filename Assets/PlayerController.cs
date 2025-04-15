using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // dictonary to hold the input data
    private Dictionary<InputButton, bool> buttonData = new Dictionary<InputButton, bool>();
    private Vector2 moveVector;
    private Vector2 aimVector;

    public Dictionary<InputButton, bool> ButtonData => buttonData;
    public Vector2 MoveVector => moveVector;
    public Vector2 AimVector => aimVector;


    private void Awake()
    {
        InputCollector.Instance.AddPlayerController(this);
    }

    public void Interact1(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Interact1] = context.ReadValueAsButton();
    }

    public void Interact2(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Interact2] = context.ReadValueAsButton();
    }


    public void Weapon1(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Weapon1] = context.ReadValueAsButton();
    }

    public void Weapon2(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Weapon2] = context.ReadValueAsButton();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Jump] = context.ReadValueAsButton();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void Ability1(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Ability1] = context.ReadValueAsButton();
    }

    public void Ability2(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Ability2] = context.ReadValueAsButton();
    }

    public void Melee(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Melee] = context.ReadValueAsButton();
    }


    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        buttonData[InputButton.SwitchWeapon] = context.ReadValueAsButton();
    }







}
