using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] float zoomInSpeed = 2f;
    [SerializeField] float zoomOutSpeed = 2f;
    [SerializeField] float baseFOV = 75f;
    float zoomedInFOV = 40f;
    bool isZoomedIn = false;

    private void Awake()
    {
        baseFOV = playerCamera.fieldOfView;

    }

    public void ZoomIn(Weapon_Arms weapon)
    {
        zoomedInFOV = weapon.ZoomFOV;
        isZoomedIn = true;
    }

    public void ZoomOut(Weapon_Arms weapon)
    {
        isZoomedIn = false;
    }

    private void Update()
    {
        // move towards zoomed in FOV, do not lerp
        if (isZoomedIn)
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, zoomedInFOV, zoomInSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, baseFOV, zoomOutSpeed * Time.deltaTime);
        }

    }

    public void EnableLayerInCamera(int layer)
    {
        playerCamera.cullingMask |= 1 << layer;
    }
}
