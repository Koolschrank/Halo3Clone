using UnityEngine;

[CreateAssetMenu(menuName = "GameModes/GunGame")]
public class GameMode_GunGame : GameMode_Deathmatch
{
	[SerializeField] Equipment[] gunGameEquipment;


	public override Equipment GetEquipmentBasedOnPoints(int points)
	{
		if (points < gunGameEquipment.Length)
		{
			return gunGameEquipment[points];
		}
		else
		{
			return base.GetEquipmentBasedOnPoints(points);
		}
	}


	



}
