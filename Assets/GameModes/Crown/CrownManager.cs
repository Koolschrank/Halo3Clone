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

        player.EnableObjectiveUIMarker();
    }

    public void OnPlayerElimination(GameObject killedPlayer, PlayerMind player)
    {


        if (playerWithCrown == killedPlayer)
        {
            var isDead = player.PlayerBody.GetComponent<CharacterHealth>().IsDead;
            if (isDead)
            {
                playerWithCrown = null;
                SpawnCrown();
            }
            else
            {
                TransferCrownToPlayer(player.PlayerBody);
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
        marker.SetActive(true);
        marker.SetHideDistance(1);
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
        ObjectiveIndicator.instance.SetPosition(crown.transform.position);
        ObjectiveIndicator.instance.SetTeamIndex(-1);
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
        ObjectiveIndicator.instance.SetPosition(player.transform.position);
        ObjectiveIndicator.instance.SetTeamIndex(crownTeamIndex);
        ObjectiveIndicator.instance.SetTimer(GetPointsLeftForTeamToWin(crownTeamIndex));

        if (gameMode.SetEquipmentOnCrownPickup)
        {
            player.GetComponent<PlayerStartEquipment>().GetEquipment(gameMode.EquipmentOnCrownPickup);
        }
        
    }

    

    private void Update()
    {

        if ( playerWithCrown != null)
        {
            ObjectiveIndicator.instance.SetPosition(playerWithCrown.transform.position);
        }

        if (crownTeamIndex < 0)
        {
            ObjectiveIndicator.instance.SetTimer(-1);
            return;
        }

        if (timeToScore > 0 )
        {
            timeToScore -= Time.deltaTime;
            if (timeToScore <= 0)
            {
                GainPoints(crownTeamIndex, 1);
                timeToScore = ((GameMode_Crown)gameModeStats).TimeToScore;
                ObjectiveIndicator.instance.SetTimer(GetPointsLeftForTeamToWin(crownTeamIndex));

            }
            
        }
    }
}
