using Fusion;
using System;
using UnityEngine;

public class PlayerArmsInput : NetworkBehaviour
{

    [SerializeField] PlayerBody playerBody;
    [SerializeField] PlayerArms playerArms;

    [SerializeField] float reloadWeaponInputBuffer = 0.2f;
    [SerializeField] float switchWeaponInputBuffer = 0.2f;
    [SerializeField] float meleeInputBuffer = 0.2f;


    [SerializeField] float holdInteractForPickUpTime = 0.2f;
    [SerializeField] float pickUp1InputBuffer = 0.2f;

    [Networked] private NetworkButtons PreviousButtons { get; set; }


    [NonSerialized]
    public bool Weapon1;
    
    public bool Weapon2;
    [NonSerialized]
    public bool Ability1;
    [NonSerialized]
    public bool Ability2;
    public bool Reload1 => reload1WeaponInputBufferCounter > 0;

    public bool Reload2 => reload2WeaponInputBufferCounter > 0;
    public bool SwitchWeapon => switchWeaponInputBufferCounter >0;
    public bool Melee => meleeInputBufferCounter > 0;

    public bool PickUp1 => pickUp1InputBufferCounter > 0;

    public bool PickUp2 => pickUp2InputBufferCounter > 0;

    float reload1WeaponInputBufferCounter = 0;
    float reload2WeaponInputBufferCounter = 0;
    float switchWeaponInputBufferCounter = 0;
    float holdInteract1ForPickUpTimeCounter = 0;
    float holdInteract2ForPickUpTimeCounter = 0;

    float meleeInputBufferCounter = 0;

    float pickUp1InputBufferCounter = 0;
    float pickUp2InputBufferCounter = 0;


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            var buttons = InputSplitter.GetContollerData(input, playerBody.LocalPlayerIndex).buttons;


            Weapon1 = buttons.IsSet(InputButton.Weapon1);
            Weapon2 = buttons.IsSet(InputButton.Weapon2);
            Ability1 = buttons.IsSet(InputButton.Ability1);
            Ability2 = buttons.IsSet(InputButton.Ability2);

            if (buttons.WasPressed(PreviousButtons, InputButton.Interact1))
            {
                reload1WeaponInputBufferCounter = reloadWeaponInputBuffer;
            }
            else if (!buttons.IsSet(InputButton.Interact1))
            {
                reload1WeaponInputBufferCounter -= Runner.DeltaTime;
            }

            if (buttons.WasPressed(PreviousButtons, InputButton.Interact2))
            {
                reload2WeaponInputBufferCounter = reloadWeaponInputBuffer;
            }
            else if (!buttons.IsSet(InputButton.Interact2))
            {
                reload2WeaponInputBufferCounter -= Runner.DeltaTime;
            }

            if (buttons.WasPressed(PreviousButtons, InputButton.SwitchWeapon))
            {
                switchWeaponInputBufferCounter = switchWeaponInputBuffer;
            }
            else if (!buttons.IsSet(InputButton.SwitchWeapon))
            {
                switchWeaponInputBufferCounter -= Runner.DeltaTime;
            }

            if (buttons.WasPressed(PreviousButtons, InputButton.Melee))
            {
                meleeInputBufferCounter = meleeInputBuffer;
            }
            else if (!buttons.IsSet(InputButton.Melee))
            {
                meleeInputBufferCounter -= Runner.DeltaTime;
            }



            if (buttons.IsSet(InputButton.Interact1) && holdInteract1ForPickUpTimeCounter != 0|| buttons.WasPressed(PreviousButtons, InputButton.Interact1))
            {
                holdInteract1ForPickUpTimeCounter += Runner.DeltaTime;
                if (holdInteract1ForPickUpTimeCounter >= holdInteractForPickUpTime)
                {
                    pickUp1InputBufferCounter = pickUp1InputBuffer;
                    holdInteract1ForPickUpTimeCounter = 0;
                }
            }
            else
            {
                holdInteract1ForPickUpTimeCounter = 0;
            }

            if (pickUp1InputBufferCounter > 0)
            {
                pickUp1InputBufferCounter -= Runner.DeltaTime;
            }

            if (buttons.IsSet(InputButton.Interact2) && holdInteract2ForPickUpTimeCounter != 0 || buttons.WasPressed(PreviousButtons, InputButton.Interact2))
            {
                holdInteract2ForPickUpTimeCounter += Runner.DeltaTime;
                if (holdInteract2ForPickUpTimeCounter >= holdInteractForPickUpTime)
                {
                    pickUp2InputBufferCounter = pickUp1InputBuffer;
                    holdInteract2ForPickUpTimeCounter = 0;
                }
            }
            else
            {
                holdInteract2ForPickUpTimeCounter = 0;
            }

            if (pickUp2InputBufferCounter > 0)
            {
                pickUp2InputBufferCounter -= Runner.DeltaTime;
            }



            PreviousButtons = buttons;
        }
    }

    public void ResetReload1Input()
    {
        reload1WeaponInputBufferCounter = 0;
    }

    public void ResetReload2Input()
    {
        reload2WeaponInputBufferCounter = 0;
    }

    public void ResetSwitchInput()
    {
        switchWeaponInputBufferCounter = 0;
    }

    public void ResetPickUp1Input()
    {
        pickUp1InputBufferCounter = 0;
    }

    public void ResetPickUp2Input()
    {
        pickUp2InputBufferCounter = 0;
    }




}
