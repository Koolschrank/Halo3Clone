using UnityEngine;


[CreateAssetMenu(fileName = "Skull_ShildGainOnlyOnMelee", menuName = "Skull/FlagMode")]
public class Skull_Flag : Skull
{
	public override void Activate()
	{
		var gameMode = GameModeSelector.gameModeManager as KingOfTheHillManager;

		gameMode.EnterFlagMode();
	}

	public override void Deactivate()
	{

		var gameMode = GameModeSelector.gameModeManager as KingOfTheHillManager;
		gameMode.ExitFlagMode();

	}

}
