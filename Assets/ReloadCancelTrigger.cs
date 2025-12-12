using UnityEngine;

public class ReloadCancelTrigger : MonoBehaviour
{
    public Weapon_Visual weaponVisual;
    public void CancelReload()
    {
        weaponVisual.OnReloadWeaponEnd?.Invoke();
	}
}
