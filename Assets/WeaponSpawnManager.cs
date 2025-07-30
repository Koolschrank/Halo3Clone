using UnityEngine;

public class WeaponSpawnManager : MonoBehaviour
{
	// singelon
	public static WeaponSpawnManager instance;


	[SerializeField] GameObject[] weaponLists;

	private void Awake()
	{
		instance = this;
	}


	public void EnableWeaponList(int index)
	{
		if (weaponLists.Length == 0) return;

		if (index < 0 || index >= weaponLists.Length)
		{
			index = 0;
		}
		for (int i = 0; i < weaponLists.Length; i++)
		{
			if (i == index)
			{
				weaponLists[i].SetActive(true);
			}
			else
			{
				weaponLists[i].SetActive(false);
			}
		}
	}
}
