using TMPro;
using UnityEngine;
// ui
using UnityEngine.UI;
using MultiplayerGameModes;

public class GameModeSelect : MonoBehaviour
{
    [SerializeField] GameMode[] gameModesToSelectFrom;
    [SerializeField] GameObject gameModeButtonPrefab;
    [SerializeField] Transform gameModeButtonParent;
    [SerializeField] float buttonSpacing = 10;

    private void Start()
    {

        // make maouse visible and unlock
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        for (int i = 0; i < gameModesToSelectFrom.Length; i++)
        {
            var gameMode = gameModesToSelectFrom[i];
            var button = Instantiate(gameModeButtonPrefab, gameModeButtonParent).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = gameMode.GameModeName;
            button.onClick.AddListener(() => SelectGameMode(gameMode));
            button.transform.localPosition += new Vector3(0, buttonSpacing * i, 0);
        }

    }

    private void SelectGameMode(GameMode gameMode)
    {
        var mapLoader = MapLoader.instance;
        if (mapLoader != null)
        {
            mapLoader.SetGameMode(gameMode);
        }
    }


}
