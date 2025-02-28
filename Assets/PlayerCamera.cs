using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Camera playerCamera;
    [SerializeField] float zoomInSpeed = 2f;
    [SerializeField] float zoomOutSpeed = 2f;
    CinemachineCamera cCam; 
    float baseFOV = 75f;
    float zoomedInFOV = 40f;
    bool isZoomedIn = false;

    private void Awake()
    {
        //baseFOV = playerCamera.fieldOfView;

    }

    public void SetCinemachineCamera(CinemachineCamera cam)
    {
        cCam = cam;
        SetBaseFOV(cCam.Lens.FieldOfView);
    }

    public void SetBaseFOV(float fov)
    {
        baseFOV = fov;
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
        if (cCam == null)
            return;

        // move towards zoomed in FOV, do not lerp
        if (isZoomedIn)
        {
            cCam.Lens.FieldOfView = Mathf.MoveTowards(cCam.Lens.FieldOfView, zoomedInFOV, zoomInSpeed * Time.deltaTime);
        }
        else
        {
            cCam.Lens.FieldOfView = Mathf.MoveTowards(cCam.Lens.FieldOfView, baseFOV, zoomOutSpeed * Time.deltaTime);
        }

    }

    public void EnableLayerInCamera(int layer)
    {
        playerCamera.cullingMask |= 1 << layer;
    }

    public void SetScreenRect(ScreenRectValues screen, int channel)
    {
        cinemachineBrain.ChannelMask = 0;
        playerCamera.rect = new Rect(screen.x, screen.y, screen.width, screen.height);
        cinemachineBrain.ChannelMask += 1 << channel + 1;

        //playerFOV.SetFOV(screen.FOV);
    }

}
