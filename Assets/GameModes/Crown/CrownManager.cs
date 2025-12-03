using System.Collections;
using UnityEngine;

public class CrownManager : GameModeManager
{
    [SerializeField] GameObject crownPrefab;
    [SerializeField] Transform crownSpawn;


    CrownObject currentCrown;
    GameObject playerWithCrown;


    float timeToScore;
    int crownTeamIndex = -1;

    public override void AISpawned(GameObject aiCharacter)
    {
        base.AISpawned(aiCharacter);

        aiCharacter.GetComponent<TargetHitCollector>().OnKill += OnPlayerKill;
    }

    public override void PlayerSpawned(PlayerMind player)
    {
        base.PlayerSpawned(player);
        var body = player.PlayerBody;
        if (body == null)
        {
            Debug.LogError("Player body is null for player: " + player.name);
            return;
        }
        body.GetComponent<TargetHitCollector>().OnKill += OnPlayerKill;
    }

    public override void PlayerJoined(PlayerMind player)
    {
        base.PlayerJoined(player);
        

        player.EnableObjectiveUIMarker();
    }

    public void OnPlayerKill( PlayerMind killer, GameObject killedPlayer)
    {
        OnPlayerKill(killedPlayer, killer.PlayerBody);
    }

    public void OnPlayerKill( GameObject killer, GameObject killedPlayer)
    {

        Debug.Log("OnPlayerKill: " + killedPlayer.name + " killed by " + killer.name);
        if (playerWithCrown == killedPlayer)
        {
            var isDead = killer.GetComponent<CharacterHealth>().IsDead;
            var sameTeam = killer.GetComponent<PlayerTeam>().TeamIndex == killedPlayer.GetComponent<PlayerTeam>().TeamIndex;
            if (isDead || sameTeam)
            {
                playerWithCrown = null;
                SpawnCrown();
            }
            else
            {
                TransferCrownToPlayer(killer);
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

        var playerMind = player.GetComponent<BodyMindConnection>().Mind;

        if (playerMind != null)
        {
            playerMind.CrownCollected();
        }
        else
        {

        }
            



        if (gameMode.SetEquipmentOnCrownPickup)
        {
            StartCoroutine(CrownWeaponEquip(player, gameMode.EquipmentOnCrownPickup, 0.35f));
            //player.GetComponent<PlayerStartEquipment>().GetEquipment(gameMode.EquipmentOnCrownPickup);
        }
        
    }

    IEnumerator CrownWeaponEquip(GameObject player, Equipment equipment, float delay)
    {
        yield return new WaitForSeconds(delay);
        var playerHealth = player.GetComponent<CharacterHealth>();
		if (!playerHealth.IsDead)
		{
			player.GetComponent<PlayerStartEquipment>().GetEquipment(equipment);
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
