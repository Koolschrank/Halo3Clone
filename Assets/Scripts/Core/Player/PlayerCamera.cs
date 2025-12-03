using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Camera playerCamera;
    [SerializeField] float zoomInSpeed = 2f;
    [SerializeField] float zoomOutSpeed = 2f;

    [SerializeField] Color healthBloomColor;
    [SerializeField] Color shildBloomColor;
    [SerializeField] Color armorBloomColor;
	[SerializeField] float shildBloomIntensity = 0.5f;
    [SerializeField] float shildBloomTime = 0.5f;
    [SerializeField] AnimationCurve shildBloomCurve;


	[SerializeField] float cromaIntensity = 0.5f;
	[SerializeField] float cromaTime = 0.5f;
	[SerializeField] AnimationCurve cromaCurve;
    float cromaTimer = 0f;

	float bloomTimer = 0f;
	CinemachineCamera cCam;
    Volume volume;
    float baseFOV = 75f;
    float zoomedInFOV = 40f;
    bool isZoomedIn = false;
	CinemachineImpulseListener impulseListener;
    public float baseBloomIntensity = 0.2f;



	public void SetVignetteIntensity(float power)
    {
        if (volume == null)
            return;

        volume.profile.TryGet(out Vignette vignette);
        vignette.intensity.value = power;
    }

    public void EnterCroma()
    {
        cromaTimer = cromaTime;
	}

    public void UpdateCroma(float power)
    {
        if (volume == null)
            return;
        volume.profile.TryGet(out ChromaticAberration chromaticAberration);
        chromaticAberration.intensity.value = power;

	}

    public void ExitCroma()
    {
        cromaTimer = 0f;
        UpdateCroma(0f);
	}

	public void ExitBloom()
    {
        bloomTimer = 0f;
        SetBloom(baseBloomIntensity);
		volume.profile.TryGet(out Bloom bloom);
		bloom.tint.value = Color.white;
	}

	public void EnterHealthBloom()
	{
		bloomTimer = shildBloomTime;
		if (volume == null)
			return;
		volume.profile.TryGet(out Bloom bloom);
		bloom.tint.value = healthBloomColor;

	}

	public void EnterShildBloom()
    {
        bloomTimer = shildBloomTime;
		if (volume == null)
			return;
		volume.profile.TryGet(out Bloom bloom);
		bloom.tint.value = shildBloomColor;
	}

	public void EnterArmorBloom()
	{
		bloomTimer = shildBloomTime;
		if (volume == null)
			return;
		volume.profile.TryGet(out Bloom bloom);
		bloom.tint.value = armorBloomColor;
	}


	public void SetBloom(float power)
        {
        if (volume == null)
            return;
        volume.profile.TryGet(out Bloom bloom);
        bloom.intensity.value = power;
	}

	
	public void SetCinemachineCamera(CinemachineCamera cam)
    {
        cCam = cam;
        volume = cCam.GetComponentInChildren<Volume>();
        SetBaseFOV(cCam.Lens.FieldOfView);
		impulseListener = cCam.GetComponent<CinemachineImpulseListener>();
	}

	public void SetGainOnInpulsListiner(float value)
	{
		impulseListener.Gain = value;

	}

	public void SetBaseFOV(float fov)
    {
        baseFOV = fov;
    }

    public void ZoomIn(Weapon_Arms weapon)
    {
        if (weapon.ZoomFOV ==0)
        {
            zoomedInFOV = baseFOV;
		}
        else
        {
			zoomedInFOV = weapon.ZoomFOV;
            if (baseFOV < 60)
            {
				zoomedInFOV -= 10f;

			}
		}

        if (weapon.Data.ReduceScreenShakeWhenAiming)
        {
            SetGainOnInpulsListiner(0.3f);
        }
            
        isZoomedIn = true;
    }

    public void ZoomOut()
    {
        isZoomedIn = false;
		SetGainOnInpulsListiner(1f);
	}

    public void ZoomOut(Weapon_Arms weapon)
    {
        isZoomedIn = false;
		SetGainOnInpulsListiner(1f);
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


		// handle bloom effect for shield
		if (bloomTimer > 0f)
		{
			bloomTimer -= Time.deltaTime;
			float bloomIntensity = shildBloomCurve.Evaluate(1f - (bloomTimer / shildBloomTime)) * shildBloomIntensity;
			SetBloom(baseBloomIntensity +bloomIntensity);
		}
		else
		{
			SetBloom(baseBloomIntensity);
            volume.profile.TryGet(out Bloom bloom);
			bloom.tint.value = Color.white;
		}

		// handle croma effect
        if (cromaTimer > 0f)
        {
            cromaTimer -= Time.deltaTime;
            float cromaIntensityValue = cromaCurve.Evaluate(1f - (cromaTimer / cromaTime)) * cromaIntensity;
            UpdateCroma(cromaIntensityValue);
        }
        else
        {
            UpdateCroma(0f);
        }
        

	}

    int fpsLayer = -1;

    public void SetFPSLayer(int layer)
    {
        fpsLayer = layer;
	}

    public void EnableFPSLayer()
    {
        if (fpsLayer == -1)
            return;
        playerCamera.cullingMask |= 1 << fpsLayer;
	}

    public void DisableFPSLayer()
    {
        if (fpsLayer == -1)
            return;
        playerCamera.cullingMask &= ~(1 << fpsLayer);
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
