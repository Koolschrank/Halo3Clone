using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class BodyMindConnection : MonoBehaviour
{
    [SerializeField] Transform mindParent;
    [SerializeField] GameObject mesh;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerPickUpScan playerPickUpScan;
    [SerializeField] Health health;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform spectatorCameraTarget;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] AbilityInventory abilityInventory;
    [SerializeField] TargetHitCollector targetHitCollector;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] PlayerStartEquipment playerStartEquipment;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] GameObject playerHead;
    [SerializeField] PlayerBodyStatSheet playerBodyStatSheet;
    [SerializeField] PlayerInteractableTrigger playerInteractableTrigger;

    [SerializeField] SkinnedMeshRenderer[] meshes;


    PlayerMind mind;

    public PlayerMind Mind => mind;

    public async Task ConnectMind(PlayerMind mind)
    {
        

        this.mind = mind;
        mind.SetPlayerBody(gameObject);
        mind.transform.SetParent(mindParent);
        mind.SetPlayerModel(mesh);
        mind.SetPlayerMovement(playerMovement);
        mind.SetPlayerAim(playerAim);
        mind.SetPlayerArms(playerArms);
        mind.SetHealth(health);
        mind.SetPickUpScan(playerPickUpScan);
        mind.SetBulletSpawner(bulletSpawner);
        
        mind.SetPlayerInventory(playerInventory);
        mind.SetAbilityInventory(abilityInventory);
        mind.SetInteractable(playerInteractableTrigger);
        mind.transform.localPosition = Vector3.zero;
        mind.transform.localRotation = Quaternion.identity;

        

        mind.ConnectPlayerElimination(targetHitCollector);
        SetPlayTeamIndex();
        playerStartEquipment.GetEquipment(GetStartEquipment());
        mind.ApplyUpgrades();
        mind.SetAlive();

        if (GameModeSelector.gameModeManager.GameModeStats.spawnWithArmor)
        {
            var characterHealth = GetComponent<CharacterHealth>();
            characterHealth.FillArmor();
		}

        // wait for  0.5 seconds before setting the stat sheet

        
        

        if (mind.PlayerMindStatSheet.usePlayerStatsSheet)
            playerBodyStatSheet.SetStatSheet(mind.PlayerMindStatSheet.playerStatSheetInstance);

        health.OnPreDeath += TrySaveEquipment;
        
    }

    public void ApplyUpgradeToMind(StatUpgrader statUpgrader)
    {
        if (mind.PlayerMindStatSheet.usePlayerStatsSheet)
            mind.PlayerMindStatSheet.playerStatSheetInstance.ApplyModifiers(statUpgrader);
    }

    
    public void TrySaveEquipment()
    {
       if (mind == null)
            return;
        var statSheet = mind.PlayerMindStatSheet.playerStatSheetInstance;

        var weaponInHand = playerArms.RightArm.GetWeaponInHand();
        var weaponInLeftHand = playerArms.LeftArm.GetWeaponInHand();
        var weaponInBack = playerInventory.GetWeapon();
        var weaponInBackLeftHand = playerInventory.GetWeapon();

        var ability1 = abilityInventory.GetAbility(0);
        var ability2 = abilityInventory.GetAbility(1);
        var ability3 = abilityInventory.GetAbility(2);

        if (weaponInHand != null)
            statSheet.startingWeapon = weaponInHand.Data;
        if (weaponInLeftHand != null)
            statSheet.startingWeapon_Left = weaponInLeftHand.Data;
        if (weaponInBack != null)
            statSheet.startingWeaponReserve = weaponInBack.Data;
        if (weaponInBackLeftHand != null)
            statSheet.startingWeaponReserve_Left = weaponInBackLeftHand.Data;
        statSheet.startingAbility1 = ability1;
        statSheet.startingAbility2 = ability2;
        statSheet.startingAbility3 = ability3;

        

    }

    public Equipment GetStartEquipment()
    {
        var equipment = new Equipment( GameModeSelector.gameModeManager.GetEquipment());
        if (MapLoader.instance == null)
            return equipment;


        bool isSwat = MapLoader.instance.IsSwat();
        bool dualWieldPlus = MapLoader.instance.IsDualWieldPlus();
        bool noMiniMap = MapLoader.instance.HasNoMiniMap();
        bool randomWeaponSpawn = MapLoader.instance.IsRandomWeaponSpawn();
        float moveSpeedMultiplier = MapLoader.instance.GetMoveSpeedMultiplier();

        if (isSwat)
        {
            equipment.SetWeapons(ItemList.instance.GetPistol(), null, null);
            equipment.SetMagazins(4, 0, 0);
            equipment.SetHasShild(false);
            equipment.SetHeadShotOneShot(false);
        }
        if (dualWieldPlus)
        {
            equipment.SetCanDualWieldEverything(true);
        }
        if (noMiniMap)
        {
            equipment.SetHasMiniMap(false);
        }
        if (randomWeaponSpawn) {
            equipment.SetWeapons(ItemList.instance.GetRandomWeapon(), null, null);
            equipment.SetMagazins(3, 0, 0);
        }

        if (moveSpeedMultiplier != 1)
        {
            equipment.SetMovementSpeedMultiplier(moveSpeedMultiplier);
        }




        return equipment;
    }

    


    public void SetCameras(CinemachineCamera camera, CinemachineCamera spectatorCamera)
    {
        camera.Follow = mindParent.transform;
        camera.LookAt = mindParent.transform;
        mind.SetSpectatorTarget(spectatorCamera);
        spectatorCamera.Follow = transform;
        spectatorCamera.LookAt = spectatorCameraTarget;
    }

    public void SetPlayTeamIndex()
    {
        playerTeam.SetTeamIndex(mind.TeamIndex);
    }

    public void SetPlayTeamIndex(int teamIndex)
    {
        playerTeam.SetTeamIndex(teamIndex);
    }

    public void SetMaterial(Material material)
    {
        foreach (var mesh in meshes)
        {
            mesh.material = material;
        }
    }

    public void SetPlayerColor(Color color)
    {
        playerAnimation.SetPlayerColor(color);
    }

    public GameObject GetPlayerHead()
    {
        return playerHead;
    }

    public PlayerMind GetMind()
    {
        return mind;
    }
}
