using UnityEngine;

public class CrownManager : GameModeManager
{
    [SerializeField] GameObject crownPrefab;
    [SerializeField] Transform crownSpawn;


    CrownObject currentCrown;
    GameObject playerWithCrown;


    float timeToScore;
    int crownTeamIndex = -1;

    public override void PlayerJoined(PlayerMind player)
    {
        base.PlayerJoined(player);
        player.OnPlayerElimination += OnPlayerElimination;
        player.OnTeamKill += OnTeamKill;

        //player.EnableObjectiveUIMarker();
    }

    public void OnPlayerElimination(GameObject killedPlayer, PlayerMind player)
    {


        if (playerWithCrown == killedPlayer)
        {
            var isDead = player.Body.Health.IsDead;
            if (isDead)
            {
                playerWithCrown = null;
                SpawnCrown();
            }
            else
            {
                TransferCrownToPlayer(player.Body.gameObject);
            }
            
        }
    }

    public void OnTeamKill(GameObject killedPlayer, PlayerMind player)
    {
        if (playerWithCrown == killedPlayer)
        {
            playerWithCrown = null;
            SpawnCrown();
        }
    }


    public override void ResetGame()
    {
        base.ResetGame();
        var marker = ObjectiveIndicator.instance;
        marker.GetObjective(0).SetActive(true);
        marker.GetObjective(0).SetHideDistance(1);
        SpawnCrown();
    }

    public void SpawnCrown()
    {
        if (currentCrown != null)
        {
            Destroy(currentCrown.gameObject);
        }
        GameObject crown = Instantiate(crownPrefab, crownSpawn.position, crownSpawn.rotation);
        currentCrown = crown.GetComponent<CrownObject>();
        currentCrown.OnCollected += OnCrownCollected;
        ObjectiveIndicator.instance.GetObjective(0).SetPosition(crown.transform.position);
        ObjectiveIndicator.instance.GetObjective(0).SetTeamIndex(-1);
        crownTeamIndex = -1;
    }

    public void OnCrownCollected(GameObject player)
    {
        
        
        Destroy(currentCrown.gameObject);
        TransferCrownToPlayer(player);

    }

    public void TransferCrownToPlayer(GameObject player)
    {
        var gameMode = (GameMode_Crown)gameModeStats;
        timeToScore = gameMode.TimeToScore;
        playerWithCrown = player;
        crownTeamIndex = player.GetComponent<PlayerTeam>().TeamIndex;
        ObjectiveIndicator.instance.GetObjective(0).SetPosition(player.transform.position);
        ObjectiveIndicator.instance.GetObjective(0).SetTeamIndex(crownTeamIndex);
        ObjectiveIndicator.instance.GetObjective(0).SetText(GetPointsLeftForTeamToWin(crownTeamIndex).ToString());

        if (gameMode.SetEquipmentOnCrownPickup)
        {
            player.GetComponent<PlayerStartEquipment>().GetEquipment(gameMode.EquipmentOnCrownPickup);
        }
        
    }

    

    private void Update()
    {

        if ( playerWithCrown != null)
        {
            ObjectiveIndicator.instance.GetObjective(0).SetPosition(playerWithCrown.transform.position);
        }

        if (crownTeamIndex < 0)
        {
            ObjectiveIndicator.instance.GetObjective(0).SetText("");
            return;
        }

        if (timeToScore > 0 )
        {
            timeToScore -= Time.deltaTime;
            if (timeToScore <= 0)
            {
                GainPoints(crownTeamIndex, 1);
                timeToScore = ((GameMode_Crown)gameModeStats).TimeToScore;
                ObjectiveIndicator.instance.GetObjective(0).SetText(GetPointsLeftForTeamToWin(crownTeamIndex).ToString());

            }
            
        }
    }
}
