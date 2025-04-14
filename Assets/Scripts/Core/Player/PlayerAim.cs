using Fusion;
using System;
using UnityEngine;
using Fusion.Addons.SimpleKCC;

public class PlayerAim : NetworkBehaviour
{

    
    public Action<Vector2> OnAimUpdated;
    public Action<float, float> OnSensitivityMultiplierChanged;

    [Header("References")]
    [SerializeField] PlayerBody playerBody;
    [SerializeField] Transform playerHead;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] CharacterHealth playerHealth;
    [SerializeField] SimpleKCC cc;


    [Header("Settings")]
    [SerializeField] float aimSpeed_x = 10f;
    [SerializeField] float aimSpeed_y = 10f;
    [SerializeField] float minAngle = -70f;
    [SerializeField] float maxAngle = 70f;
    [SerializeField] float zoomAimSpeedMultiplier = 0.5f;

    [Header("Aim support Settings")]
    [SerializeField] float aimSupportDistance = 10f;
    [SerializeField] LayerMask aimSupportLayerMask;
    [SerializeField] float aimSupportSlowDown = 0.5f;

    Vector2 aimInput = Vector2.zero;

    
    float sensitivityMultiplier = 1f;
    [Header("Sensitivity Settings")]
    [SerializeField] float minSensitivity = 0.30f;
    [SerializeField] float maxSensitivity = 2f;
    
    [SerializeField] float sensitivityChangeSteps = 0.10f;


    [Networked]
    public Vector2 ViewDirection { get; set; }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        // bug fix so that player stps rotating when dead
        //playerHealth.OnDeath += () =>
        //{
        //    sensitivityMultiplier = 0;
        //};
    }

    public override void FixedUpdateNetwork()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {

        if (HasStateAuthority)
        {

            GetInput(out NetworkInputData data);
            var controller = InputSplitter.GetContollerData(data, playerBody.LocalPlayerIndex);
            // x rotates player y rotates camera
            Vector2 input = controller.aimVector; //controller.Player.Aim.ReadValue<Vector2>();
            float rotationX = input.x * aimSpeed_x * sensitivityMultiplier * Runner.DeltaTime;
            float rotationY = input.y * aimSpeed_y * sensitivityMultiplier * Runner.DeltaTime;

            float playerXRotation = transform.eulerAngles.y;
            float playerYRotation = playerHead.transform.eulerAngles.x;

            if (CheckIfHoverOverEnemy())
            {
                rotationX *= aimSupportSlowDown;
                rotationY *= aimSupportSlowDown;
            }
            if (playerArms.RightArm.IsInZoom)
            {
                rotationX *= zoomAimSpeedMultiplier;
                rotationY *= zoomAimSpeedMultiplier;
            }

            playerXRotation += rotationX;
            playerYRotation -= rotationY;
            //playerYRotation = Mathf.Clamp(playerYRotation, minAngle, maxAngle);


            
            ViewDirection = new Vector2 (playerXRotation, playerYRotation);
           
        }
        cc.SetLookRotation(new Vector3(0, ViewDirection.x, 0));
        playerHead.transform.eulerAngles = new Vector3(ViewDirection.y, ViewDirection.x, 0);


    }

    // render
    public override void Render()
    {
        return;
        if (!HasInputAuthority)
        {
            cc.SetLookRotation(new Vector3(0, ViewDirection.x, 0));
            playerHead.transform.eulerAngles = new Vector3(ViewDirection.y, 0, 0);


        }
        
    }

    public bool CheckIfHoverOverEnemy()
    {
        Ray ray = new Ray(playerHead.transform.position, playerHead.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, aimSupportDistance, aimSupportLayerMask))
        {
            // check if hit has health component
            if (hit.collider.TryGetComponent<PlayerTeam>(out PlayerTeam t) && t.TeamIndex != playerTeam.TeamIndex)
                return true;


            return false;
        }
        return false;
    }

    public void UpdateAimInput(Vector2 input)
    {
        aimInput = input;
        OnAimUpdated?.Invoke(input);
    }

    public void AddSensetivity()
    {
        var sensitivityChange = sensitivityChangeSteps;
        if (sensitivityMultiplier < 0.49999)
        {
            sensitivityChange /= 2;

        }

        AddToSensetivityMultiplier(sensitivityChange);
    }

    public void ReduceSensetivity()
    {
        var sensitivityChange = sensitivityChangeSteps;
        if (sensitivityMultiplier < 0.49999)
        {
            sensitivityChange /= 2;
        }
        AddToSensetivityMultiplier(-sensitivityChange);
    }

    public void SetSensetivityWithNoActionSent(float sensetivity)
    {
        sensitivityMultiplier = sensetivity;
        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, minSensitivity, maxSensitivity);
    }


    public void AddToSensetivityMultiplier(float addedValue)
    {
        sensitivityMultiplier += addedValue;
        sensitivityMultiplier = Mathf.Clamp(sensitivityMultiplier, minSensitivity, maxSensitivity);
        float percentage = (sensitivityMultiplier - minSensitivity) / (maxSensitivity - minSensitivity);
        OnSensitivityMultiplierChanged?.Invoke(sensitivityMultiplier, percentage);
    }
}
