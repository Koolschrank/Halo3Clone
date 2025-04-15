using Fusion;
using UnityEngine;

public class PlayerArmsInput : NetworkBehaviour
{

    [SerializeField] PlayerBody playerBody;
    [SerializeField] PlayerArms playerArms;

    [Networked] private NetworkButtons PreviousButtons { get; set; }
    

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            var buttons = InputSplitter.GetContollerData(input, playerBody.LocalPlayerIndex).buttons;


            if (buttons.IsSet(InputButton.Weapon1))
            {
                // shoot 1
            }
            if (buttons.IsSet(InputButton.Weapon2))
            {
                // shoot 2
            }

            if (buttons.WasPressed(PreviousButtons, InputButton.SwitchWeapon))
            {
                // switch
            }
            if (buttons.WasPressed(PreviousButtons, InputButton.Interact1))
            {
                // reload
            }



            PreviousButtons = buttons;
        }
    }


     

}
