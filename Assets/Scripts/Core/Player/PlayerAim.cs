using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    
    public Action<Vector2> OnAimUpdated;
    public Action<float, float> OnSensitivityMultiplierChanged;

    [Header("References")]
    [SerializeField] GameObject playerHead;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerTeam playerTeam;

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




    private void Start()
    {
        // hide mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        // x rotates player y rotates camera
        Vector2 input = aimInput; //controller.Player.Aim.ReadValue<Vector2>();
        float rotationX = input.x * aimSpeed_x * sensitivityMultiplier * Time.deltaTime;
        float rotationY = input.y * aimSpeed_y * sensitivityMultiplier * Time.deltaTime;

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
        


        transform.eulerAngles = new Vector3(0, playerXRotation, 0);
        playerHead.transform.eulerAngles = new Vector3(playerYRotation, playerXRotation, 0);

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
