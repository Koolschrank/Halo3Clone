using UnityEngine;

public class PlayerUpgrader : MonoBehaviour
{
    [SerializeField] int upgradeCount = 3;
    [SerializeField] PlayerProgression playerProgression;

    private void Start()
    {
        playerProgression.OnLevelUp += EnterUpgradeMenu;
    }

    public void EnterUpgradeMenu()
    {
        Time.timeScale = 0;


        var players = PlayerManager.instance.GetAllPlayers();



        foreach (var player in players)
        {
            if (player.IsDead)
            {
                player.Respawn();
            }


            player.SetUpUpgradeMenu(upgradeCount);
            player.OnUpgradeSelectionFinished += UpgradePlayer;
        }
    }

    int playersUpgraded = 0;

    public void UpgradePlayer()
    {
        playersUpgraded++;
        if (playersUpgraded >= PlayerManager.instance.GetAllPlayers().Count)
        {
            ExitUpgradeMenu();
        }
    }

    public void ExitUpgradeMenu()
    {
        var players = PlayerManager.instance.GetAllPlayers();
        foreach (var player in players)
        {
            player.OnUpgradeSelectionFinished -= UpgradePlayer;
        }

        playersUpgraded = 0;
        Time.timeScale = 1;
    }



}
