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


    public void UseWeapon1(InputAction.CallbackContext context)
    {
        buttonData[InputButton.UseAbility1] = context.ReadValueAsButton();
    }

    public void ReloadWeapon1(InputAction.CallbackContext context)
    {
        buttonData[InputButton.ReloadWeapon1] = context.ReadValueAsButton();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        buttonData[InputButton.Jump] = context.ReadValueAsButton();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }

    public void Aim(InputAction.CallbackContext context)
    {
        aimVector = context.ReadValue<Vector2>();
    }

    public void UseWeaponAbility(InputAction.CallbackContext context)
    {
        buttonData[InputButton.UseWeaponAbility] = context.ReadValueAsButton();
    }

    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        buttonData[InputButton.SwitchWeapon] = context.ReadValueAsButton();
    }







}
