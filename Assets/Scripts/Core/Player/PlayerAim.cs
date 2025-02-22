using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    
    public Action<Vector2> OnAimUpdated;

    [Header("References")]
    [SerializeField] GameObject playerHead;
    [SerializeField] PlayerArms playerArms;

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
        float rotationX = input.x * aimSpeed_x * Time.deltaTime;
        float rotationY = input.y * aimSpeed_y * Time.deltaTime;

        float playerXRotation = transform.eulerAngles.y;
        float playerYRotation = playerHead.transform.eulerAngles.x;

        if (CheckIfHoverOverEnemy())
        {
            rotationX *= aimSupportSlowDown;
            rotationY *= aimSupportSlowDown;
        }
        if (playerArms.IsInZoom)
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
            if (hit.collider.TryGetComponent<Health>(out Health h))
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
}
