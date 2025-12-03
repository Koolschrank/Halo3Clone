using UnityEngine;
using UnityEngine.UI;

public class crosshairUI : MonoBehaviour
{
    Color baseColor;

    [SerializeField] Color onTargetColor;
    [SerializeField] RawImage crosshairImage;


    Weapon_Arms currentWeapon;

     

    public void DisableCrosshair()
        {
		crosshairImage.enabled = false;
	}


	public void ChangeSprite(Weapon_Arms weapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.OnBloomUpdate -= UpdateBloom;
            currentWeapon.OnEnterDualWield -= () => UpdateBloom(0);
            currentWeapon.OnExitDualWield -= () => UpdateBloom(0);
		}

		weapon.OnBloomUpdate += UpdateBloom;
        weapon.OnEnterDualWield += () => UpdateBloom(0);
        weapon.OnExitDualWield += () => UpdateBloom(0);

		currentWeapon = weapon;
		if (weapon == null)
        {
            crosshairImage.enabled = false;
            return;
		}
		crosshairImage.enabled = true;


		var sprite = weapon.CrosshairUI;
        var size = weapon.CrosshairSizeUI;

		if (crosshairImage != null)
        {
            if (sprite == null)
            {
                crosshairImage.enabled = false;
                return;
            }
            else 
            {
                crosshairImage.enabled = true;
            }

            crosshairImage.texture = sprite.texture;
            crosshairImage.rectTransform.localScale = size;
        }
    }

    void UpdateBloom(float bloom)
    {
		crosshairImage.rectTransform.localScale = 
            currentWeapon.CrosshairSizeUI 
            * (1f + 
            bloom * currentWeapon.Data.BloomCrosshairsSizeMultiplier);

	}

    //start
    private void Start()
    {
        baseColor = crosshairImage.color;
    }

    // target acquired
    public void OnTargetAcquired(Transform target)
    {
        crosshairImage.color = onTargetColor;
    }

    // target lost
    public void OnTargetLost(Transform target)
    {
        crosshairImage.color = baseColor;
    }
}
