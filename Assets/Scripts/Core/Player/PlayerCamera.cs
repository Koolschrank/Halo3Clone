using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCamera : InterfaceItem
{
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Camera playerCamera;
    [SerializeField] float zoomInSpeed = 2f;
    [SerializeField] float zoomOutSpeed = 2f;
    CinemachineCamera cinemachineCamera;
    CinemachineCamera spectatorCamera;
    Volume volume;
    float baseFOV = 75f;
    float zoomedInFOV = 40f;
    bool isZoomedIn = false;

    PlayerBody playerBody;

    protected override void Subscribe(PlayerBody body)
    {
        body.PlayerArms.OnZoomedChanged += UpdateZoom;
        body.PlayerArms.OnRightWeaponEquiped += (weapon) => SetZoomedInFOV(weapon.ZoomFOV);

        playerBody = body;

        if (body.PlayerArms.Weapon_RightHand != null)
            SetZoomedInFOV(body.PlayerArms.Weapon_RightHand.ZoomFOV);
        UpdateZoom(body.PlayerArms.InZoom);

        if (cinemachineCamera != null)
        {
            body.SetCameras(cinemachineCamera, spectatorCamera);
            body.SetVisualLayer(playerInterface.HidenPlayerLayer);
        }
            
        
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        body.PlayerArms.OnZoomedChanged -= UpdateZoom;
        body.PlayerArms.OnRightWeaponEquiped -= (weapon) => SetZoomedInFOV(weapon.ZoomFOV);
        playerBody = null;

    }



    public void SetVignetteIntensity(float power)
    {
        if (volume == null)
            return;

        volume.profile.TryGet(out Vignette vignette);
        vignette.intensity.value = power;
    }


    public void SetCinemachineCamera(CinemachineCamera cam)
    {
        cinemachineCamera = cam;
        volume = cinemachineCamera.GetComponentInChildren<Volume>();
        SetBaseFOV(cinemachineCamera.Lens.FieldOfView);
    }

    public void SetSpectatorCamera(CinemachineCamera cam)
    {
        spectatorCamera = cam;

        if (playerBody != null)
        {
            playerBody.SetCameras(cinemachineCamera, spectatorCamera);
            playerBody.SetVisualLayer(playerInterface.HidenPlayerLayer);
        }
    }

    public void SetBaseFOV(float fov)
    {
        baseFOV = fov;
    }

    public void SetZoomedInFOV(float fov)
    {
        zoomedInFOV = fov;
    }

    public void UpdateZoom(bool val)
    {
        isZoomedIn = val;
    }

    private void Update()
    {
        if (cinemachineCamera == null)
            return;

        // move towards zoomed in FOV, do not lerp
        if (isZoomedIn)
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.MoveTowards(cinemachineCamera.Lens.FieldOfView, zoomedInFOV, zoomInSpeed * Time.deltaTime);
        }
        else
        {
            cinemachineCamera.Lens.FieldOfView = Mathf.MoveTowards(cinemachineCamera.Lens.FieldOfView, baseFOV, zoomOutSpeed * Time.deltaTime);
        }

    }

    public void EnableLayerInCamera(int layer)
    {
        playerCamera.cullingMask |= 1 << layer;
    }

    public void DisableLayerInCamera(int layer)
    {
        playerCamera.cullingMask &= ~(1 << layer);
    }

    public void SetScreenRect(ScreenRectValues screen, int channel)
    {
        cinemachineBrain.ChannelMask = 0;
        playerCamera.rect = new Rect(screen.x, screen.y, screen.width, screen.height);
        cinemachineBrain.ChannelMask += 1 << channel + 1;

        playerCamera.targetDisplay = screen.targetDisplay;

        //playerFOV.SetFOV(screen.FOV);
    }

}
